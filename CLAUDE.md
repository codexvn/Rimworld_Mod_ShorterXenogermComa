# CLAUDE.md

给 Claude Code 的代码库指南。

## 概述

ShorterXenogermComa（异种胚芽昏迷缩短）是一个 RimWorld 1.6 mod，将异种胚芽植入后的昏迷时间缩短为 1 tick（即刻苏醒）。

## 原始代码流程

异种胚芽植入昏迷的完整链路涉及 3 个类、2 次 `ticksToDisappear` 赋值：

### 步骤一：hediff 创建 — 第一次赋值（HediffComp_Disappears.cs:98-104）

当 `HediffMaker.MakeHediff(XenogerminationComa)` 被调用时，hediff 上的 `HediffComp_Disappears` 组件通过 `CompPostMake` 初始化：

```csharp
// HediffComp_Disappears.cs:98-104
public override void CompPostMake()
{
    base.CompPostMake();
    this.disappearsAfterTicks = this.Props.disappearsAfterTicks.RandomInRange;  // 从 def 取范围随机
    this.seed = Rand.Int;
    this.ticksToDisappear = this.disappearsAfterTicks;  // 第一次赋值
}
```

此时 `this.Props` 来自 def 的 `HediffCompProperties_Disappears`。**这就是 XML patch / `StaticConstructorOnStartup` 改 def 能影响到的唯一位置。** 修改 def 的 `disappearsAfterTicks` 在这里会生效。

### 步骤二：手术完成 — 第二次赋值（SurgeryOutcome_HediffWithDuration.cs:11-18）

`ImplantXenogerm` 手术的 `surgeryOutcomeEffect` 包含 `SurgeryOutcome_HediffWithDuration` 配置，hediff 字段指向 `XenogerminationComa`。手术完成后调用 `Apply`：

```csharp
// SurgeryOutcome_HediffWithDuration.cs:11-18
public override bool Apply(float quality, RecipeDef recipe, Pawn surgeon, Pawn patient, BodyPartRecord part)
{
    if (Rand.Chance(this.chance))
    {
        Hediff hd = HediffMaker.MakeHediff(this.hediff, patient, ...);  // 步骤一发生在这里
        hd.TryGetComp<HediffComp_Disappears>().ticksToDisappear =
            Mathf.RoundToInt(this.qualityToDurationDaysCurve.Evaluate(quality) * 60000f);  // 第二次赋值，覆写！
        patient.health.AddHediff(hd, null, null, null);
        return true;
    }
    return false;
}
```

`qualityToDurationDaysCurve` 是在 Biotech DLC 的 XML defs 中定义的曲线，输入手术质量（quality，0~1），输出天数。`XenogerminationComa` 的曲线约输出 2 天，乘以 60000 ticks/天 → ~120000 ticks。

### 为什么 XML patch / def 修改不生效

```
CompPostMake()            → ticksToDisappear = def.disappearsAfterTicks.RandomInRange  (比如 1)
    ↓
Apply() 立即覆写          → ticksToDisappear = qualityCurve(quality) * 60000             (约 120000)
```

**第二次赋值无条件覆写了第一次赋值。** 步骤一无论设什么值，步骤二都会用曲线计算结果覆盖。这就是为什么：

- 改 XML `<disappearsAfterTicks>1~1</disappearsAfterTicks>` → 无效，Apply() 覆盖
- `StaticConstructorOnStartup` 中改 `comp.disappearsAfterTicks = IntRange(1,1)` → 同样无效

### 顺便提一句：`GeneUtility.ReimplantXenogerm` 血族异种植入能力

血族能力走的是另一条路径：

```csharp
// GeneUtility.cs:121
recipient.health.AddHediff(HediffDefOf.XenogerminationComa, null, null, null);
```

这里直接 `AddHediff`，不经过 `SurgeryOutcome_HediffWithDuration.Apply()`。如果用 def 修改方案，这一路径能生效——但手术路径仍然不行。已用 Harmony patch 一并覆盖。

## 解决方案

两条路径各用一个 Harmony postfix：

| 路径 | Patch 类 | 目标方法 |
|------|----------|----------|
| 手术 `ImplantXenogerm` | `Patch_SurgeryOutcome_XenogermComa` | `SurgeryOutcome_HediffWithDuration.Apply()` |
| 血族能力 `ReimplantXenogerm` | `Patch_ReimplantXenogerm_Coma` | `GeneUtility.ReimplantXenogerm()` |

两者都是 postfix，通过 `GetFirstHediffOfDef(XenogerminationComa)` 找到昏迷 hediff 后强制 `ticksToDisappear = 1`。

## 项目结构

```
ShorterXenogermComa/
  ShorterXenogermComa.csproj    — net472，引用 Assembly-CSharp.dll + 0Harmony.dll
  ShorterXenogermComa_Mod.cs    — Mod 子类 + Harmony patch
  Properties/AssemblyInfo.cs    — 程序集元数据
  publish/
    About/About.xml             — mod 元数据
    1.6/Assemblies/             — 编译输出
```

## 构建

```
dotnet build
```

输出：`publish/1.6/Assemblies/ShorterXenogermComa.dll`
所有 DLL 引用设 `<Private>False</Private>` 避免复制游戏 DLL。

## 目录链接

```
mklink /D F:\SteamLibrary\steamapps\common\RimWorld\Mods\ShorterXenogermComa F:\RiderProjects\ShorterXenogermComa\publish
```
