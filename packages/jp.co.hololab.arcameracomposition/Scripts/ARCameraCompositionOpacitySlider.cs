using UnityEngine;
using UnityEngine.UI;

namespace HoloLab.ARCameraComposition
{
    public class ARCameraCompositionOpacitySlider : MonoBehaviour
    {
        [SerializeField]
        private ARCameraCompositionOpacityConfigurator configurator;

        [SerializeField]
        private Slider slider;

        private void Start()
        {
            slider.value = configurator.GetOpacity();
            slider.onValueChanged.AddListener(Slider_OnValueChanged);
        }

        private void OnDestroy()
        {
            if (slider != null)
            {
                slider.onValueChanged.RemoveListener(Slider_OnValueChanged);
            }
        }

        private void Slider_OnValueChanged(float value)
        {
            configurator.SetOpacity(value);
        }
    }
}