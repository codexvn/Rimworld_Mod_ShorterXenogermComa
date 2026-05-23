# 异种胚芽昏迷缩短 (Shorter Xenogerm Coma)

移除 RimWorld 异种胚芽植入后的昏迷时间，殖民者植入后立即恢复。

## 功能

将 `XenogerminationComa` hediff 的 `disappearsAfterTicks` 从原版的 1.5~3 天改为 1 tick，昏迷即刻消失。

## 兼容

- 游戏版本：1.6
- 纯 XML patch，无 C# 代码，兼容所有不含 `XenogerminationComa` 改动的 mod

## 安装

将 `publish/` 复制到 RimWorld 的 `Mods/ShorterXenogermComa/` 文件夹。

推荐创建目录链接：

```
mklink /D F:\SteamLibrary\steamapps\common\RimWorld\Mods\ShorterXenogermComa F:\RiderProjects\ShorterXenogermComa\publish
```
