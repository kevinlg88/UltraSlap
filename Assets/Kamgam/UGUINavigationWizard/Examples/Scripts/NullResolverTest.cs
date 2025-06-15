using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.UGUINavigationWizard.Examples
{
    public class NullResolverTest : MonoBehaviour
    {
        public GameObject ContentA;
        public GameObject ContentB;
        public GameObject ContentC;

        IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(3);
                ContentA.SetActive(false);

                yield return new WaitForSecondsRealtime(3);
                ContentB.SetActive(false);

                yield return new WaitForSecondsRealtime(3);
                ContentC.SetActive(false);

                yield return new WaitForSecondsRealtime(3);
                ContentA.SetActive(true);
                ContentB.SetActive(true);
                ContentC.SetActive(true);
            }
        }
    }
}
