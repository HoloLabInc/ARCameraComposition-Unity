using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoloLab.ARCameraComposition
{
    [RequireComponent(typeof(Camera))]
    public class ARCameraCompositionCameraConfigurator : MonoBehaviour
    {
        private Camera cameraComponent;

        private void Start()
        {
            cameraComponent = GetComponent<Camera>();
        }

        private void LateUpdate()
        {
            cameraComponent.clearFlags = CameraClearFlags.SolidColor;
            cameraComponent.backgroundColor = Color.clear;
        }
    }
}
