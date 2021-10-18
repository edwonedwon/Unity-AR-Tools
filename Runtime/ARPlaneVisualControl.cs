using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Edwon.ARTools
{
    public class ARPlaneVisualControl : MonoBehaviour
    {
        ARPlaneManager arPlaneManager;
        public Material textureMaterial;
        Material textureMaterialShared;
        public string textureColorParameter;
        public string planeColorParameter;
        Color textureColorStart;
        Color planeColorStart;

        void Awake()
        {
            if (Application.isPlaying && !Application.isEditor)
            {
                arPlaneManager = FindObjectOfType<ARPlaneManager>();
                Material[] sharedMaterials = arPlaneManager.planePrefab.GetComponent<Renderer>().sharedMaterials;
                foreach(Material m in sharedMaterials)
                {
                    if (m.name == textureMaterial.name)
                    {
                        textureMaterialShared = m;
                        break;
                    }
                }
            }
            else
            {
                textureMaterialShared = textureMaterial;
            }
            
            textureColorStart = textureMaterialShared.GetColor(textureColorParameter);
            planeColorStart = textureMaterialShared.GetColor(planeColorParameter);
        }

        [InspectorButton("ShowPlaneTexture")]
        public bool showPlaneTexture;
        public void ShowPlaneTexture()
        {
            textureMaterialShared.SetColor(textureColorParameter, textureColorStart);
            textureMaterialShared.SetColor(planeColorParameter, planeColorStart);
        }

        [InspectorButton("HidePlaneTexture")]
        public bool hidePlaneTexture;
        public void HidePlaneTexture()
        {
            float alpha = 0;
            Color textureColor = new Color(
                textureColorStart.r,
                textureColorStart.g,
                textureColorStart.b,
                alpha
            );
            Color planeColor = new Color(
                planeColorStart.r,
                planeColorStart.g,
                planeColorStart.b,
                alpha
            );
            textureMaterialShared.SetColor(textureColorParameter, textureColor);
            textureMaterialShared.SetColor(planeColorParameter, textureColor);
        }
    }
}