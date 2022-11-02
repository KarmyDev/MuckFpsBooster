using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;
using MuckSetting = MuckSettings.Settings;
using UnityEngine.Rendering.PostProcessing;

namespace FpsBooster
{
    [BepInPlugin(Guid, Name, Version)]
    public class FpsBoosterMain : BaseUnityPlugin
    {
        public const string
            Name = "FpsBooster",
            Author = "KarmyDev",
            Guid = Author + "." + Name,
            Version = "1.0.0";

        internal readonly Harmony harmony;
        internal readonly Assembly assembly;

        public static ConfigFile config = new ConfigFile(Path.Combine(Paths.ConfigPath, "example.cfg"), true);
        public static ConfigEntry<bool> isPostProcessingEnabled = config.Bind<bool>("Enable Post Processing", "enablePostProcessing", true, "Enables complex rendering that may lower fps. Disable it if you have potato pc.");
		public static ConfigEntry<int> renderDistance = config.Bind<int>("Render Distance", "renderDistance", 3000, "Sets how far will you see. 3000 is the default. 250 is best for potato pc.");
		
        public FpsBoosterMain()
        {
            harmony = new Harmony(Guid);
            assembly = Assembly.GetExecutingAssembly();

            // this line is very important, anyone using this as an example shouldn't forget to copy-paste this as well!
            config.SaveOnConfigSet = true;
			
			isPostProcessingEnabled.SettingChanged += (sender, e) => Patches.ReloadSettings();
			renderDistance.SettingChanged += (sender, e) => Patches.ReloadSettings();
			
            harmony.PatchAll(assembly);
        }
    }

    [HarmonyPatch]
    class Patches
    {
        [HarmonyPatch(typeof(MuckSetting), "Graphics"), HarmonyPrefix]
        static void Graphics(MuckSetting.Page page)
        {
			page.AddSliderSetting("Render Distance", FpsBoosterMain.renderDistance, 100, 3000);
            page.AddBoolSetting("Enable Post Processing", FpsBoosterMain.isPostProcessingEnabled);
			
        }
		
		[HarmonyPatch(typeof(MoveCamera), "Start"), HarmonyPostfix]
		static void Start()
		{
			ReloadSettings();
		}
		
		public static void ReloadSettings()
		{
			// Get the cam when the cam is Initialised (A player loaded game)
			Camera cam = MoveCamera.Instance.mainCam;
			
			// Get the post processing component thats ruining all the performace on the potato
			PostProcessLayer layer;
			if (cam.TryGetComponent<PostProcessLayer>(out layer))
			{
				// If the post processing is attached to the camera, apply our settings
				layer.enabled = FpsBoosterMain.isPostProcessingEnabled.Value;
			}
			
			// Load our settings for render distance
			cam.farClipPlane = FpsBoosterMain.renderDistance.Value;
		}
    }
}
