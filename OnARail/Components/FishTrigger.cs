using NewHorizons.Utility;
using UnityEngine;

namespace OnARail.Components
{
    internal class FishTrigger : MonoBehaviour
    {
        GameObject munchVolume;

        private void Awake()
        {
            munchVolume = SearchUtilities.Find("Locomocean_Body/Sector/MunchVolume");
        }

        public virtual void OnTriggerEnter(Collider hitCollider)
        {
            if (hitCollider.attachedRigidbody == Locator.GetPlayerBody()._rigidbody)
            {
                munchVolume.SetActive(true);
                //OnARail.DebugLog("Player in Angler! Enabling MunchVolume!", OWML.Common.MessageType.Info);
            }
        }

        public virtual void OnTriggerExit(Collider hitCollider)
        {
            if (hitCollider.attachedRigidbody == Locator.GetPlayerBody()._rigidbody)
            {
                //Need this incase player touches while inside the ship! Otherwise destruction volume won't work until exiting and re-entering the volume!
                munchVolume.SetActive(false);
                //OnARail.DebugLog("Player left Angler! Disabling MunchVolume!", OWML.Common.MessageType.Info);
            }
        }
    }
}