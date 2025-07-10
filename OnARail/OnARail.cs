using OWML.Common;
using OWML.ModHelper;
using NewHorizons.Utility;
using UnityEngine;
using NewHorizons;
using OWML.ModHelper.Events;
using OnARail.Components;
using System.Linq;
using System.Collections.Generic;

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
                    //water.layer = LayerMask.NameToLayer("IgnoreSun");
                    //ModHelper.Console.WriteLine("Set layer to IgnoreSun in Ocean_Body/Sector/Water!", MessageType.Info);

                    Material[] waterMaterial = water.GetComponent<TessellatedSphereRenderer>().GetComponent<TessellatedRenderer>()._materials;
                    waterMaterial[1].color = new Color(0.4f, 0.8f, 1f, 0f);
                }
                else{ModHelper.Console.WriteLine("Can't find Ocean_Body/Sector/Water!", MessageType.Error);}

                GameObject fishStandardRoot = SearchUtilities.Find("Ocean_Body/Sector/Fish_Standard");
                if (fishStandardRoot != null)
                {
                    List<GameObject> fishList = SearchUtilities.GetAllChildren(fishStandardRoot);
                    GameObject[] fish = fishList.ToArray();
                    
                    foreach (GameObject currentFish in fishList)
                    {
                        _ = currentFish.AddComponent<FishController>();
                        ModHelper.Console.WriteLine("Added FishController to " + currentFish, MessageType.Info);
                    }
                }
                else{ModHelper.Console.WriteLine("Can't find Fish_Standard!", MessageType.Error);}
            }
            else
            {
                ModHelper.Console.WriteLine("Couldn't locate planet: Ocean!", MessageType.Error);
            }
        }
    }
}