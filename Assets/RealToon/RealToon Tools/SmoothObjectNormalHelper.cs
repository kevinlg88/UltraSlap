//RealToon - SmoothObjectNomal [Helper]
//MJQStudioWorks
//©2024

using UnityEngine;

namespace RealToon.Script
{
    [ExecuteAlways]
    [AddComponentMenu("RealToon/Tools/SmoothObjectNormal - Helper")]
    public class SmoothObjectNormalHelper : MonoBehaviour
    {
        [Header("[RealToon - Smooth Object Normal - Helper]")]

        [Space(25)]

        [Space(10)]
        [SerializeField]
        [Tooltip("A material that uses 'RealToon - Smooth Object Normal' feature.")]
        public Material Material = null;

        [SerializeField]
        [Tooltip("An object to follow the position.")]
        public Transform ObjectToFollow = null;

        [HideInInspector]
        [SerializeField]
        bool checkstart = true;

        string RT_Sha_Nam_URP = "Universal Render Pipeline/RealToon/Version 5/Default/Default";
        string RT_Sha_Nam_HDRP = "HDRP/RealToon/Version 5/Default";

        string RT_Sha_Nam_BiRP_DD = "RealToon/Version 5/Default/Default";
        string RT_Sha_Nam_BiRP_DFT = "RealToon/Version 5/Default/Fade Transparency";
        string RT_Sha_Nam_BiRP_DR = "RealToon/Version 5/Default/Refraction";
        string RT_Sha_Nam_BiRP_TDD = "RealToon/Version 5/Tessellation/Default";
        string RT_Sha_Nam_BiRP_TDFT = "RealToon/Version 5/Tessellation/Fade Transparency";
        string RT_Sha_Nam_BiRP_TDR = "RealToon/Version 5/Tessellation/Refraction";

        void LateUpdate()
        {

            if (Material == null || ObjectToFollow == null)
            { }
            else
            {
                if (Material.shader.name == RT_Sha_Nam_URP || 
                    Material.shader.name == RT_Sha_Nam_HDRP ||
                    Material.shader.name == RT_Sha_Nam_BiRP_DD ||
                    Material.shader.name == RT_Sha_Nam_BiRP_DFT ||
                    Material.shader.name == RT_Sha_Nam_BiRP_DR ||
                    Material.shader.name == RT_Sha_Nam_BiRP_TDD ||
                    Material.shader.name == RT_Sha_Nam_BiRP_TDFT ||
                    Material.shader.name == RT_Sha_Nam_BiRP_TDR)
                {
                    Vector3 ObjPos = ObjectToFollow.gameObject.transform.position;
                    Material.SetVector("_XYZPosition", ObjPos);
                }
            }

        }

        void OnValidate()
        {
            if (Material == null)
            {
                checkstart = true;
            }
            else if (Material != null)
            {
                if (Material.shader.name == RT_Sha_Nam_URP ||
                    Material.shader.name == RT_Sha_Nam_HDRP ||
                    Material.shader.name == RT_Sha_Nam_BiRP_DD ||
                    Material.shader.name == RT_Sha_Nam_BiRP_DFT ||
                    Material.shader.name == RT_Sha_Nam_BiRP_DR ||
                    Material.shader.name == RT_Sha_Nam_BiRP_TDD ||
                    Material.shader.name == RT_Sha_Nam_BiRP_TDFT ||
                    Material.shader.name == RT_Sha_Nam_BiRP_TDR)
                {
                    if (checkstart == true)
                    {
                        if (Material.IsKeywordEnabled("N_F_SON_ON") == false)
                        {
                            Material.EnableKeyword("N_F_SON_ON");
                            Material.SetFloat("_N_F_SON", 1.0f);
                            checkstart = false;
                        }
                    }
                }
            }
        }

        void Reset()
        {
            Material = null;
            ObjectToFollow = null;
            checkstart = true;
        }

    }
}
