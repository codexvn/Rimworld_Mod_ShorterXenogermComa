# 异种胚芽昏迷缩短 (Shorter Xenogerm Coma)

移除 RimWorld 异种胚芽植入后的昏迷时间，殖民者植入后立即恢复。

## 原理

游戏在手术完成时通过 `SurgeryOutcome_HediffWithDuration.Apply()` 覆写 coma hediff 的 `ticksToDisappear`，基于手术质量曲线计算——原版约 2 天。

本 mod 通过 Harmony patch 拦截该方法，在 `XenogerminationComa` 被添加后立即将 `ticksToDisappear` 强制设为 1，昏迷即刻结束。

## 兼容

- 游戏版本：1.6
- 需要 Biotech DLC
- 纯 Harmony patch，无 XML 覆盖，与其他 mod 兼容

## 安装

将 `publish/` 复制到 RimWorld 的 `Mods/ShorterXenogermComa/` 文件夹。

推荐创建目录链接：

```
mklink /D F:\SteamLibrary\steamapps\common\RimWorld\Mods\ShorterXenogermComa F:\RiderProjects\ShorterXenogermComa\publish
```
