# Muck FPS [booster]
A mod for Muck that provides settings to change rendering distance and disable post-processing, which greatly improves performance on low-end devices.

### [Support me! ❤️ ☕ (Ko-Fi)](https://ko-fi.com/karmy)
[[FpsBooster thunderstore page link]]()

# Building from source
This mod requires serveral libraries that it depends on.<br>
<br>
You will need to create `Libraries` folder next to the `FpsBooster.csproj` file and fill it with assemblies:<br>
1. [`MuckSettings.dll`](https://muck.thunderstore.io/package/Terrain/SettingsApi/), [`BepInEx.dll`, `0Harmony.dll`](https://muck.thunderstore.io/package/BepInEx/BepInExPack_Muck/),<br>
2. `Assembly-CSharp.dll`, `UnityEngine.CoreModule.dll`, `UnityEngine.dll`, `Unity.Postprocessing.Runtime.dll`<br>
<br>
While first ones are easy to obtain from thunderstore (You just have to click manual download and unpack .dlls from the .zip files).<br>
<i>(<b>BepInEx.dll</b> and <b>0Harmony.dll</b> are in <code>/BepInExPack_Muck/BepInEx/core/</code>)</i><br>
<br>
Second ones require you to download muck and in the Muck's local folder (<code>Steam/steamapps/common/Muck</code>)<br>
you have to go to the <code>Muck_Data/Managed</code> folder and copy assemblies listed from 2nd line above.

# Contact
If you find any bug you can [open a new Issue on this mod's github Issue tab](https://github.com/KarmyDev/MuckFpsBooster/issues).

# This mod needs
[**BepInEx**](https://muck.thunderstore.io/package/BepInEx/BepInExPack_Muck/)<br>
[**SettingsApi**](https://muck.thunderstore.io/package/Terrain/SettingsApi/)<br>
