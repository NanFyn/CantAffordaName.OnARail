using NewHorizons.Utility;
using UnityEngine;

namespace OnARail.Components
{
    internal class GravityAssistTrigger : MonoBehaviour
    {
        GameObject assistVolume;

        private void Awake()
        {
            assistVolume = SearchUtilities.Find("Locomocean_Body/Sector/Fish_Puzzle/Fish/FishCollider/WarpScale/WarpAssist");
        }

        public virtual void OnTriggerEnter(Collider hitCollider)
        {
            if (hitCollider.attachedRigidbody == Locator.GetPlayerBody()._rigidbody)
            {
                assistVolume.SetActive(false);
            }
        }

        public static GravityAssistTrigger CreateGravityAssistTrigger(GameObject planet, Vector3 pos, float radius)
        {
            var volume = new GameObject("GravityAssistTrigger");
            volume.transform.parent = planet.transform;
            volume.transform.localPosition = pos;
            volume.layer = LayerMask.NameToLayer("BasicEffectVolume");

            var sphere = volume.AddComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = radius;

            var triggerVolume = volume.AddComponent<GravityAssistTrigger>();

            return triggerVolume;
        }
    }
}