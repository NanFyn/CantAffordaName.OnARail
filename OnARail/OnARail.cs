using OWML.Common;
using OWML.ModHelper;
using NewHorizons.Utility;
using UnityEngine;
using OnARail.Components;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.Events;
using NewHorizons.Components.Orbital;

namespace OnARail
{
    public class OnARail : ModBehaviour
    {
        public INewHorizons newHorizons;
        public static OnARail Instance;
        public Material porcelain, silver, black;

        private void Awake()
        {
            Instance = this;
        }

        internal void Start()
        {
            //ModHelper.Console.WriteLine($"My mod {nameof(OnARail)} is loaded!", MessageType.Success);

            // Get the New Horizons API and load configs
            newHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            newHorizons.LoadConfigs(this);

            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene != OWScene.SolarSystem) return;
                //ModHelper.Console.WriteLine("Loaded into solar system with On A Rail!", MessageType.Success);
            };

            newHorizons.GetStarSystemLoadedEvent().AddListener(OnStarSystemLoaded);
            GlobalMessenger.AddListener("EnterMapView", OnEnterMapView);
            GlobalMessenger<DeathType>.AddListener("PlayerDeath", OnPlayerDeath);
            GlobalMessenger.AddListener("TriggerSupernova", OnTriggerSupernova);
        }

        private void OnStarSystemLoaded(string systemName)
        {
            newHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");

            GameObject star = newHorizons.GetPlanet("Railway System");
            if (star != null)
            {
                GameObject visorEffects = SearchUtilities.Find("RailwaySystem_Body/Sector/VisorEffects");
                if (visorEffects != null)
                {
                    VisorRainEffectVolume visorRainEffectVolume = SearchUtilities.Find("RailwaySystem_Body/Sector/VisorEffects/WetEffectVolume2").GetComponent<VisorRainEffectVolume>();
                    visorRainEffectVolume._rainDirection = VisorRainEffectVolume.RainDirection.Linear;
                    visorEffects.SetActive(false);
                }
                else { ModHelper.Console.WriteLine("Can't find VisorEffects!", MessageType.Error); }
            }
            else
            {
                ModHelper.Console.WriteLine("Couldn't locate planet: Railway System!", MessageType.Error);
            }

            GameObject trainPlanet = newHorizons.GetPlanet("Stellar Express");
            if (trainPlanet != null)
            {
                GameObject trainWhistle = SearchUtilities.Find("StellarExpress_Body/Sector/TrainBase/WhistleController");
                if (trainWhistle != null)
                {
                    trainWhistle.AddComponent<WhistleController>();
                }
                else { ModHelper.Console.WriteLine("Can't find WhistleController!", MessageType.Error); }

                GameObject trainInterface = SearchUtilities.Find("StellarExpress_Body/Sector/TrainInterface");
                if (trainInterface != null)
                {
                    trainInterface.AddComponent<CoordInterfaceController>();
                }
                else { ModHelper.Console.WriteLine("Can't find TrainInterface!", MessageType.Error); }

                GameObject warpReceiverMain = SearchUtilities.Find("StellarExpress_Body/Sector/Warp_Train_Main");
                if (warpReceiverMain != null)
                {
                    warpReceiverMain.transform.localScale = new Vector3(0.92f, 0.95f, 0.92f);
                }
                else { ModHelper.Console.WriteLine("Can't find Warp_Train_Main!", MessageType.Error); }

                GameObject endingGM = SearchUtilities.Find("StellarExpress_Body/Sector/EndingGM");
                if (endingGM != null)
                {
                    endingGM.SetActive(false);
                }
                else { ModHelper.Console.WriteLine("Can't find EndingGM!", MessageType.Error); }

                GameObject creditsVolume = SearchUtilities.Find("StellarExpress_Body/Sector/LoadCreditsVolume");
                if (creditsVolume != null)
                {
                    creditsVolume.SetActive(false);
                }
                else { ModHelper.Console.WriteLine("Can't find LoadCreditsVolume!", MessageType.Error); }

                GameObject revealVolume = SearchUtilities.Find("StellarExpress_Body/Sector/Reveal Volume (Enter)");
                if (revealVolume != null)
                {
                    revealVolume.SetActive(false);
                }
                else { ModHelper.Console.WriteLine("Can't find Reveal Volume (Enter)!", MessageType.Error); }

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

            GameObject frozenPlanet = newHorizons.GetPlanet("Frost Car");
            if (frozenPlanet != null)
            {
                SphereShape sectorSphere = SearchUtilities.Find("FrostCar_Body/Sector").GetComponent<SphereShape>();
                if (sectorSphere != null)
                {
                    sectorSphere.radius = 600f;
                }

                SphereCollider sphereCollider = SearchUtilities.Find("FrostCar_Body/Sector/Air").GetComponent<SphereCollider>();
                if (sphereCollider != null)
                {
                    sphereCollider.radius = 60f;
                }

                AtmosphereTrigger.CreateAtmosphereTrigger(frozenPlanet, new Vector3(-60f, 0f, 0f), new Vector3(0f, 0f, 0f));
            }
            else
            {
                ModHelper.Console.WriteLine("Couldn't locate planet: Frost Car!", MessageType.Error);
            }

            GameObject oceanPlanet = newHorizons.GetPlanet("Locomocean");
            if (oceanPlanet != null)
            {
                SphereShape sectorSphere = SearchUtilities.Find("Locomocean_Body/Sector").GetComponent<SphereShape>();
                if (sectorSphere != null)
                {
                    sectorSphere.radius = 600f;
                }

                SphereCollider sphereCollider = SearchUtilities.Find("Locomocean_Body/Sector/Air").GetComponent<SphereCollider>();
                if (sphereCollider != null)
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
                else { ModHelper.Console.WriteLine("Can't find Locomocean_Body/Sector/Water!", MessageType.Error); }

                GameObject fishStandardRoot = SearchUtilities.Find("Locomocean_Body/Sector/Fish_Standard");
                if (fishStandardRoot != null)
                {
                    GameObject fish = SearchUtilities.Find("Locomocean_Body/Sector/Fish_Standard/Fish");
                    if (fish != null)
                    {
                        fish.AddComponent<FishController>();
                        for (int i = 0; i < 24; i++)
                        {
                            GameObject fishClone = Instantiate(fish, fishStandardRoot.transform);
                        }
                        fish.transform.rotation = new Quaternion(180f, 0f, 0f, 0f);
                        for (int i = 0; i < 23; i++)
                        {
                            GameObject fishClone = Instantiate(fish, fishStandardRoot.transform);
                        }
                    }
                    else { ModHelper.Console.WriteLine("Can't find Fish!", MessageType.Error); }
                }
                else { ModHelper.Console.WriteLine("Can't find Fish_Standard!", MessageType.Error); }

                GameObject fishPuzzleRoot = SearchUtilities.Find("Locomocean_Body/Sector/Fish_Standard");
                if (fishPuzzleRoot != null)
                {
                    GameObject fish = SearchUtilities.Find("Locomocean_Body/Sector/Fish_Puzzle/Fish");
                    if (fish != null)
                    {
                        fish.AddComponent<FishController>();
                    }
                    else { ModHelper.Console.WriteLine("Can't find Fish!", MessageType.Error); }
                }
                else { ModHelper.Console.WriteLine("Can't find Fish_Puzzle!", MessageType.Error); }
            }
            else
            {
                ModHelper.Console.WriteLine("Couldn't locate planet: Locomocean!", MessageType.Error);
            }

            GameObject sprucePlanet = newHorizons.GetPlanet("Spruce Caboose");
            if (sprucePlanet != null)
            {
                SphereShape sectorSphere = SearchUtilities.Find("SpruceCaboose_Body/Sector").GetComponent<SphereShape>();
                if (sectorSphere != null)
                {
                    sectorSphere.radius = 1000f;
                }

                SphereCollider sphereCollider = SearchUtilities.Find("SpruceCaboose_Body/Sector/Air").GetComponent<SphereCollider>();
                if (sphereCollider != null)
                {
                    sphereCollider.radius = 110f;
                }

                WaterTrigger.CreateWaterTrigger(sprucePlanet, 29.6f);
                AtmosphereTrigger.CreateAtmosphereTrigger(sprucePlanet, new Vector3(0f, 0f, 100f), new Vector3(0f, 90f, 0f));
                AtmosphereTrigger.CreateAtmosphereTrigger(sprucePlanet, new Vector3(0f, 0f, -100f), new Vector3(90f, 270f, 0f));
                AtmosphereTrigger.CreateAtmosphereTrigger(sprucePlanet, new Vector3(100f, 0f, 0f), new Vector3(0f, 0f, 0f));
                AtmosphereTrigger.CreateAtmosphereTrigger(sprucePlanet, new Vector3(-100f, 0f, 0f), new Vector3(0f, 0f, 0f));
                AtmosphereTrigger.CreateAtmosphereTrigger(sprucePlanet, new Vector3(0f, 100f, 0f), new Vector3(0f, 0f, 90f));
                AtmosphereTrigger.CreateAtmosphereTrigger(sprucePlanet, new Vector3(0f, -100f, 0f), new Vector3(0f, 0f, 90f));
            }
            else
            {
                ModHelper.Console.WriteLine("Couldn't locate planet: Spruce Caboose!", MessageType.Error);
            }

            GameObject beaconPlanet = newHorizons.GetPlanet("Beacon Blocker");
            if (sprucePlanet != null)
            {
                SphereShape sectorSphere = SearchUtilities.Find("BeaconBlocker_Body/Sector").GetComponent<SphereShape>();
                if (sectorSphere != null)
                {
                    sectorSphere.radius = 60f;
                }
            }
            else
            {
                ModHelper.Console.WriteLine("Couldn't locate planet: Spruce Caboose!", MessageType.Error);
            }

            GameObject startPlatform = newHorizons.GetPlanet("CantAffordaName_OnARail_Platform");
            if (startPlatform != null)
            {
                porcelain = Resources.FindObjectsOfTypeAll<Material>().First(x => x.name.Contains("Structure_NOM_PorcelainClean_mat"));
                silver = Resources.FindObjectsOfTypeAll<Material>().First(x => x.name.Contains("Structure_NOM_Silver_mat"));
                black = Resources.FindObjectsOfTypeAll<Material>().First(x => x.name.Contains("Structure_NOM_SilverPorcelain_mat"));

                ReplaceMaterials(startPlatform);
            }
            else
            {
                ModHelper.Console.WriteLine("Couldn't locate planet: CantAffordaName_OnARail_Platform!", MessageType.Error);
            }
        }

        //NHOrbitLine can't be accessed in OnStarSystemLoaded
        private void OnEnterMapView()
        {
            newHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");

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
            //Instead of running here, do it inside CoordInterfaceController
            /*var newHorizons = Instance.ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            Temporary: Set stuff up once train is done!
            GameObject warpSwitch = SearchUtilities.Find("StellarExpress_Body/Sector/TrainInterface/WarpSwitch");
            GameObject pillarRoot = SearchUtilities.Find("StellarExpress_Body/Sector/TrainInterface/PillarPivot/PillarRoot");
            
            warpSwitch.transform.SetParent(pillarRoot.transform);
            warpSwitch.transform.localPosition = new Vector3(0, 0.4f, 0);

            //Disable for Final Voyage, but re-enable the loop later to prevent game over!
            TimeLoop.SetTimeLoopEnabled(false);
            GlobalMessenger<OWRigidbody>.FireEvent("ExitTimeLoopCentral", Locator.GetPlayerBody());

            GameObject endingGM = SearchUtilities.Find("StellarExpress_Body/Sector/EndingGM");
            GameObject trainWhistle = SearchUtilities.Find("StellarExpress_Body/Sector/TrainBase/WhistleController");
            endingGM.SetActive(true);
            Destroy(trainWhistle);
            StartCoroutine(EndingCoroutine());*/
        }

        /*private IEnumerator EndingCoroutine()
        {
            yield return new WaitForSeconds(8);
            GameObject creditsVolume = SearchUtilities.Find("StellarExpress_Body/Sector/LoadCreditsVolume");
            creditsVolume.SetActive(true);
        }*/

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