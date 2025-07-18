using OWML.Common;
using OWML.ModHelper;
using NewHorizons.Utility;
using UnityEngine;
using OnARail.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using NewHorizons.Components.Orbital;

namespace OnARail
{
    public class OnARail : ModBehaviour
    {
        public static OnARail Instance;
        public Material porcelain, silver, black;

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
            GlobalMessenger.AddListener("EnterMapView", OnEnterMapView);
            GlobalMessenger<DeathType>.AddListener("PlayerDeath", OnPlayerDeath);
            GlobalMessenger.AddListener("TriggerSupernova", OnTriggerSupernova);
        }

        private void OnStarSystemLoaded(string systemName)
        {
            var newHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");

            GameObject star = newHorizons.GetPlanet("Railway System");
            if (star != null)
            {
                VisorRainEffectVolume visorRainEffectVolume = SearchUtilities.Find("RailwaySystem_Body/Sector/WetEffectVolume2").GetComponent<VisorRainEffectVolume>();
                visorRainEffectVolume._rainDirection = VisorRainEffectVolume.RainDirection.Linear;
            }

            GameObject trainPlanet = newHorizons.GetPlanet("Stellar Express");
            if (trainPlanet != null)
            {
                GameObject trainInterface = SearchUtilities.Find("StellarExpress_Body/Sector/TrainInterface");
                if (trainInterface != null)
                {
                    trainInterface.AddComponent<CoordInterfaceController>();
                }
                else{ModHelper.Console.WriteLine("Can't find TrainInterface!", MessageType.Error);}

                GameObject warpReceiverMain = SearchUtilities.Find("StellarExpress_Body/Sector/Warp_Train_Main");
                if (warpReceiverMain != null)
                {
                    warpReceiverMain.transform.localScale = new Vector3(0.92f, 0.95f, 0.92f);
                }
                else{ModHelper.Console.WriteLine("Can't find Warp_Train_Main!", MessageType.Error);}

                GameObject fireAttachPoint = SearchUtilities.Find("StellarExpress_Body/Sector/Props/Fire/AttachPoint");
                if (fireAttachPoint != null)
                {
                    fireAttachPoint.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
                }
                else{ModHelper.Console.WriteLine("Can't find AttachPoint!", MessageType.Error);}

                porcelain = Resources.FindObjectsOfTypeAll<Material>().First(x => x.name.Contains("Structure_NOM_PorcelainClean_mat"));
                silver = Resources.FindObjectsOfTypeAll<Material>().First(x => x.name.Contains("Structure_NOM_Silver_mat"));
                black = Resources.FindObjectsOfTypeAll<Material>().First(x => x.name.Contains("Structure_NOM_SilverPorcelain_mat"));

                var stellarExpress = newHorizons.GetPlanet("Stellar Express");
                ReplaceMaterials(stellarExpress);
            }
            else
            {
                ModHelper.Console.WriteLine("Couldn't locate planet: Stellar Express!", MessageType.Error);
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

        //NHOrbitLine can't be accessed in OnStarSystemLoaded
        private void OnEnterMapView()
        {
            var newHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");

            GameObject trainOrbit = SearchUtilities.Find("StellarExpress_Body/Orbit");
            if (trainOrbit != null)
            {
                trainOrbit.GetComponent<NHOrbitLine>()._lineWidth = 1;
                LineRenderer lineRenderer = trainOrbit.GetComponent<LineRenderer>();
                lineRenderer.material.renderQueue = 0;
                lineRenderer.endColor = new Color(0, 0, 0, 0);
                lineRenderer.endWidth = 0;
                lineRenderer.numPositions = 128;
                lineRenderer.positionCount = 128;
            }
            else{ModHelper.Console.WriteLine("Can't find Orbit!", MessageType.Error);}
        }

        public static void SolvedCoords()
        {
            var newHorizons = Instance.ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            //Temporary: Set stuff up once train is done!
            GameObject warpSwitch = SearchUtilities.Find("StellarExpress_Body/Sector/TrainInterface/WarpSwitch");
            GameObject pillarRoot = SearchUtilities.Find("StellarExpress_Body/Sector/TrainInterface/PillarPivot/PillarRoot");
            
            warpSwitch.transform.SetParent(pillarRoot.transform);
            warpSwitch.transform.localPosition = new Vector3(0, 0.4f, 0);

            //Disable for Final Voyage, but re-enable the loop later to prevent game over!
            TimeLoop.SetTimeLoopEnabled(false);
            GlobalMessenger<OWRigidbody>.FireEvent("ExitTimeLoopCentral", Locator.GetPlayerBody());
        }

        private static void OnPlayerDeath(DeathType deathType)
        {
            TimeLoop.SetTimeLoopEnabled(true);
        }

        private void OnTriggerSupernova()
        {
            TimeLoop.SetTimeLoopEnabled(true);
        }

        public static void DebugLog(string line, MessageType type)
        {
            Instance.ModHelper.Console.WriteLine($"DEBUG: {line}", MessageType.Info);
        }

        public void ReplaceMaterials(GameObject go)
        {
            foreach (var renderer in go.GetComponentsInChildren<Renderer>())
            {
                renderer.materials = renderer.materials.Select(GetReplacementMaterial).ToArray();
            }
        }

        private Material GetReplacementMaterial(Material material)
        {
            if (material.name.Contains("Structure_NOM_SandStone_mat") ||
                material.name.Contains("Structure_NOM_SandStone_Dark_mat")
                )
            {
                return porcelain;
            }
            else if (material.name.Contains("Structure_NOM_CopperOld_mat") ||
                material.name.Contains("Structure_NOM_TrimPattern_mat") ||
                material.name.Contains("Structure_NOM_CopperOld_Dark_mat")
                )
            {
                return silver;
            }
            else if (material.name.Contains("Structure_NOM_PropTile_Color_mat") ||
                material.name.Contains("Structure_NOM_SandStone_Darker_mat") ||
                material.name.Contains("Structure_NOM_WarpReceiver_mat")
                )
            {
                return black;
            }

            return material;
        }
    }
}