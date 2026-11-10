using UnityEngine;
using UnityEngine.UI;

// Haengt an einem UI Slider (Vorlesung 6). Der Button-/Slider-Event wird hier
// per Code abonniert (AddListener), statt im Inspector.
[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    Slider slider;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = PlayerPrefs.GetFloat(AudioManager.VolumeKey, 0.8f);
        slider.onValueChanged.AddListener(OnChanged);
    }

    void OnDestroy()
    {
        if (slider != null) slider.onValueChanged.RemoveListener(OnChanged);
    }

    void OnChanged(float value)
    {
        AudioManager.SetMasterVolume(value);
    }
}
