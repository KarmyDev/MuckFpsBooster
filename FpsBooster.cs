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
            Version = "1.2.0";

        internal readonly Harmony harmony;
        internal readonly Assembly assembly;

        public static ConfigFile config = new ConfigFile(Path.Combine(Paths.ConfigPath, "example.cfg"), true);
		public static ConfigEntry<int> renderDistance = config.Bind<int>("Render Distance", "renderDistance", 3000, "Sets how far will you see. 3000 is the default. 250 is best for potato pc.");
        public static ConfigEntry<bool> isPostProcessingEnabled = config.Bind<bool>("Enable Post Processing", "enablePostProcessing", true, "Enables complex rendering that may lower fps. Disable it if you have potato pc.");
        public static ConfigEntry<bool> areWorldMeshShadowsEnabled = config.Bind<bool>("Enable World Mesh Shadows", "enableWorldMeshShadows", true, "Enables the shadows on the world mesh.");
		public static ConfigEntry<bool> areLeafParticlesEnabled = config.Bind<bool>("Enable Leaf Particles", "enableLeafParticles", true, "Enables floating particles that may impact fps.");
		
        public FpsBoosterMain()
        {
            harmony = new Harmony(Guid);
            assembly = Assembly.GetExecutingAssembly();

            // this line is very important, anyone using this as an example shouldn't forget to copy-paste this as well!
            config.SaveOnConfigSet = true;
			
			renderDistance.SettingChanged += (sender, e) => Patches.UpdateRenderDistanceSettings();
			isPostProcessingEnabled.SettingChanged += (sender, e) => Patches.UpdatePostProcessingSettings();
			areWorldMeshShadowsEnabled.SettingChanged += (sender, e) => Patches.UpdateWorldMeshShadowsSettings();
			areLeafParticlesEnabled.SettingChanged += (sender, e) => Patches.UpdateLeafParticlesSettings();
			
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
			page.AddBoolSetting("Enable World Mesh Shadows", FpsBoosterMain.areWorldMeshShadowsEnabled);
			page.AddBoolSetting("Enable Leaf Particles", FpsBoosterMain.areLeafParticlesEnabled);
			
        }
		
		[HarmonyPatch(typeof(MoveCamera), "Start"), HarmonyPostfix]
		static void Start()
		{
			LoadCamera();
			ReloadSettings();
		}
		
		// -{ v1.2.0 }- -> Added more settings!
		
		public static Camera camRef;
		public static PostProcessLayer layerRef;
		public static GameObject leafParticlesRef;
		public static MeshRenderer worldMeshRef;
		
		public static void LoadCamera()
		{
			// Get the cam when the cam is Initialised (A player loaded game)
			camRef = MoveCamera.Instance.mainCam;
			
			// Get the post processing component thats ruining all the performace on the potato
			if (layerRef == null) camRef.TryGetComponent<PostProcessLayer>(out layerRef);
			
			if (leafParticlesRef == null) leafParticlesRef = PlayerMovement.Instance.transform.Find("LeafParticles").gameObject;
			
			if (worldMeshRef == null) worldMeshRef = World.Instance.GetComponent<MeshRenderer>();
		}
		
		public static void ReloadSettings()
		{
			if (camRef == null) return;
			// Update all the settings once loaded
			UpdateRenderDistanceSettings();
			UpdatePostProcessingSettings();
			UpdateLeafParticlesSettings();
			UpdateWorldMeshShadowsSettings();
		}
		
		public static void UpdateWorldMeshShadowsSettings()
		{
			if (worldMeshRef == null) return;
			worldMeshRef.shadowCastingMode = FpsBoosterMain.areWorldMeshShadowsEnabled.Value ? UnityEngine.Rendering.ShadowCastingMode.On : UnityEngine.Rendering.ShadowCastingMode.Off;
		}
		
		public static void UpdateLeafParticlesSettings()
		{
			if (leafParticlesRef == null) return;
			
			leafParticlesRef.SetActive(FpsBoosterMain.areLeafParticlesEnabled.Value);
		}
		
		public static void UpdateRenderDistanceSettings()
		{
			if (camRef == null) return;
			
			// Load our settings for render distance
			camRef.farClipPlane = FpsBoosterMain.renderDistance.Value;
		}
		
		public static void UpdatePostProcessingSettings()
		{
			if (camRef == null) return;
			
			// Get the post processing component thats ruining all the performace on the potato
			if (layerRef == null) camRef.TryGetComponent<PostProcessLayer>(out layerRef);
			
			// If the post processing is attached to the camera, apply our settings
			layerRef.enabled = FpsBoosterMain.isPostProcessingEnabled.Value;
		}
    }
}
