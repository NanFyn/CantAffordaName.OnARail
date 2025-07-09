using OWML.Common;
using OWML.ModHelper;
using NewHorizons.Utility;
using UnityEngine;
using NewHorizons;
using OWML.ModHelper.Events;

namespace OnARail
{
    public class OnARail : ModBehaviour
    {
        private void Awake()
        {
            // You won't be able to access OWML's mod helper in Awake. Use Start() instead.
            // Harmony Patches can go here.
        }

        internal void Start()
        {
            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"My mod {nameof(OnARail)} is loaded!", MessageType.Success);

            // Get the New Horizons API and load configs
            var newHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            newHorizons.LoadConfigs(this);

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene != OWScene.SolarSystem) return;
                ModHelper.Console.WriteLine("Loaded into solar system with On A Rail!", MessageType.Success);
            };

            newHorizons.GetStarSystemLoadedEvent().AddListener(OnStarSystemLoaded);
        }

        private void OnStarSystemLoaded(string systemName)
        {
            var newHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");

            GameObject oceanPlanet = newHorizons.GetPlanet("Ocean");
            if (oceanPlanet != null)
            {
                GameObject water = SearchUtilities.Find("Ocean_Body/Sector/Water");
                if (water != null)
                {
                    water.layer = LayerMask.NameToLayer("IgnoreSun");
                    ModHelper.Console.WriteLine("Set layer to IgnoreSun in Ocean_Body/Sector/Water!", MessageType.Info);

                    Material[] waterMaterial = water.GetComponent<TessellatedSphereRenderer>().GetComponent<TessellatedRenderer>()._materials;
                    waterMaterial[1].color = new Color(0.4f, 0.8f, 1f, 0f);
                    //Makes atmosphere render infront as it should, but not worth the hassle of reconfiguring renderQueue for everything
                    //waterMaterial[1].renderQueue = 999;
                }
                else{ModHelper.Console.WriteLine("Can't find Ocean_Body/Sector/Water!", MessageType.Error);}
                /*TessSphereSectorToggle oceanGDTessSectorToggle = SearchUtilities.Find("Ocean_Body/Sector/Ocean_GD").GetComponent<TessSphereSectorToggle>();
                if (oceanGDTessSectorToggle != null)
                {
                    Destroy(oceanGDTessSectorToggle);
                    ModHelper.Console.WriteLine("Destroying TessSphereSectorToggle in Ocean_GD!", MessageType.Info);
                }
                else
                {
                    ModHelper.Console.WriteLine("Can't find TessSphereSectorToggle in Ocean_GD!", MessageType.Error);
                }
                GameObject waterVolume = SearchUtilities.Find("Ocean_Body/Sector/WaterVolume");
                if (waterVolume != null)
                {
                    RadialFluidVolume radialFluidVolume = waterVolume.GetComponent<RadialFluidVolume>();
                    radialFluidVolume._buoyancyDensity = 0.9f;
                }
                else
                {
                    ModHelper.Console.WriteLine("Can't find WaterVolume!", MessageType.Error);
                }*/
            }
            else
            {
                ModHelper.Console.WriteLine("Couldn't locate planet: Ocean!", MessageType.Error);
            }
        }
    }
}