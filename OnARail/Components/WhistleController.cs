using System.Collections;
using UnityEngine;

namespace OnARail.Components
{
    public class WhistleController : MonoBehaviour
    {
        public GameObject whistler;
        public GameObject ghostMatter;
        public bool justWhistled = false;

        void Awake()
        {
            whistler = this.gameObject.transform.GetChild(0).gameObject;
            ghostMatter = this.gameObject.transform.GetChild(1).gameObject;
        }

        private void Update()
        {
            var currentTime = TimeLoop.GetMinutesElapsed();

            //GetMinutesElapsed: whole numbers are minutes, decimals are seconds
            //i.e. 20 minutes left: currentTime = 2  ||  19.5 minutes left: currentTime = 2.5
            if (currentTime % 2 >= 0 && currentTime % 2 <= 0.01 && !justWhistled)
            {
                //OnARail.DebugLog("Time to whistle!", OWML.Common.MessageType.Success);
                justWhistled = true;
                StartCoroutine(PlayWhistle());
            }
        }

        private IEnumerator PlayWhistle()
        {
            whistler.SetActive(true);
            ghostMatter.SetActive(false);
            yield return new WaitForSeconds(26);
            //OnARail.DebugLog("The whistle has stopped!", OWML.Common.MessageType.Success);
            justWhistled = false;
            whistler.SetActive(false);
            ghostMatter.SetActive(true);
        }
    }
}
