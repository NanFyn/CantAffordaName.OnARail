using OWML.Common;
using OWML.ModHelper;
using NewHorizons.Utility;
using UnityEngine;
using OnARail.Components;
using System.Collections.Generic;
using UnityEngine.Events;

namespace OnARail
{
    public class OnARail : ModBehaviour
    {
        public static OnARail Instance;

        private void Awake()
        {
            Instance = this;
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

            GameObject trainPlanet = newHorizons.GetPlanet("Locomocean");
            if (trainPlanet != null)
            {
                GameObject trainInterface = SearchUtilities.Find("StellarExpress_Body/Sector/TrainInterface");
                if (trainInterface != null)
                {
                    trainInterface.AddComponent<CoordInterfaceController>();
                }
                else { ModHelper.Console.WriteLine("Can't find TrainInterface!", MessageType.Error); }
            }

            GameObject oceanPlanet = newHorizons.GetPlanet("Locomocean");
            if (oceanPlanet != null)
            {
                SphereCollider sphereCollider = SearchUtilities.Find("Locomocean_Body/Sector/Air").GetComponent<SphereCollider>();
                if(sphereCollider != null)
                {
                    sphereCollider.radius = 110f;
                }
                
                GameObject water = SearchUtilities.Find("Locomocean_Body/Sector/Water");
                if (water != null)
                {
                    //water.layer = LayerMask.NameToLayer("IgnoreSun");
                    //ModHelper.Console.WriteLine("Set layer to IgnoreSun in Locomocean_Body/Sector/Water!", MessageType.Info);

                    Material[] waterMaterial = water.GetComponent<TessellatedSphereRenderer>().GetComponent<TessellatedRenderer>()._materials;
                    waterMaterial[1].color = new Color(0.4f, 0.8f, 1f, 0f);
                }
                else{ModHelper.Console.WriteLine("Can't find Locomocean_Body/Sector/Water!", MessageType.Error);}

                GameObject fishStandardRoot = SearchUtilities.Find("Locomocean_Body/Sector/Fish_Standard");
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
                ModHelper.Console.WriteLine("Couldn't locate planet: Locomocean!", MessageType.Error);
            }
        }

        public static void SolvedCoords()
        {
            var newHorizons = Instance.ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            //Temporary: Set stuff up once train is done!
            GameObject warpSwitch = SearchUtilities.Find("StellarExpress_Body/Sector/TrainInterface/WarpSwitch");
            GameObject pillarRoot = SearchUtilities.Find("StellarExpress_Body/Sector/TrainInterface/PillarPivot/PillarRoot");
            
            warpSwitch.transform.SetParent(pillarRoot.transform);
            warpSwitch.transform.localPosition = new Vector3(0, 0.4f, 0);
        }

        public static void DebugLog(string line, MessageType type)
        {
            Instance.ModHelper.Console.WriteLine($"DEBUG: {line}", MessageType.Info);
        }
    }
}