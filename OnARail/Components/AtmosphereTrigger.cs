using NewHorizons.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace OnARail.Components
{
    internal class AtmosphereTrigger : MonoBehaviour
    {
        private INewHorizons newHorizons = OnARail.Instance.newHorizons;
        public GameObject[] ghostMatterArray;
        public GameObject visorEffects;

        void Awake()
        {
            GameObject star = newHorizons.GetPlanet("Railway System");
            if (star != null)
            {
                GameObject visorRoot = SearchUtilities.Find("RailwaySystem_Body/Sector/VisorEffects");
                if (visorRoot != null)
                {
                    visorEffects = visorRoot;
                }
                else {OnARail.DebugLog("Can't find VisorEffects!", OWML.Common.MessageType.Error);}
            }

            List<GameObject> ghostMatterList = new List<GameObject>();

            GameObject frozenPlanet = newHorizons.GetPlanet("Frost Car");
            if (frozenPlanet != null)
            {
                GameObject ghostMatterFrozen = SearchUtilities.Find("FrostCar_Body/Sector/FrozenInterior/GM_Root_Main");
                if (ghostMatterFrozen != null)
                {
                    ghostMatterList.Add(ghostMatterFrozen);
                }
                else {OnARail.DebugLog("Can't find GM_Root_Main!", OWML.Common.MessageType.Error);}
            }

            GameObject sprucePlanet = newHorizons.GetPlanet("Spruce Caboose");
            if (frozenPlanet != null)
            {
                GameObject ghostMatterFrozen = SearchUtilities.Find("SpruceCaboose_Body/Sector/SpruceBase/GM_Root_Main");
                if (ghostMatterFrozen != null)
                {
                    ghostMatterList.Add(ghostMatterFrozen);
                }
                else { OnARail.DebugLog("Can't find GM_Root_Main!", OWML.Common.MessageType.Error); }
            }

            ghostMatterArray = ghostMatterList.ToArray();
        }

        public virtual void OnTriggerEnter(Collider hitCollider)
        {
            if (hitCollider.attachedRigidbody == Locator.GetPlayerBody()._rigidbody)
            {
                //OnARail.DebugLog("Player exiting atmosphere, re-enable ghost matter!", OWML.Common.MessageType.Info);
                visorEffects.SetActive(false);

                foreach (GameObject obj in ghostMatterArray)
                {
                    obj.SetActive(true);
                }
            }
        }

        public static AtmosphereTrigger CreateAtmosphereTrigger(GameObject planet, Vector3 pos, Vector3 rot)
        {
            var volume = new GameObject("AtmosphereTrigger");
            volume.transform.parent = planet.transform;
            volume.transform.localPosition = pos;
            volume.transform.eulerAngles = rot;
            volume.transform.localScale = new Vector3(2f, 200f, 200f);
            volume.layer = LayerMask.NameToLayer("BasicEffectVolume");

            var cube = volume.AddComponent<BoxCollider>();
            cube.isTrigger = true;

            var triggerVolume = volume.AddComponent<AtmosphereTrigger>();

            return triggerVolume;
        }
    }
}