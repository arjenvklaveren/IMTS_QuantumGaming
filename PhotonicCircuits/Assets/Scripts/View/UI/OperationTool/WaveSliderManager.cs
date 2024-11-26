using Game.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveSliderManager : MonoBehaviour
{
    [SerializeField] Slider wavelengthSlider;
    [SerializeField] Slider amplitudeSlider;
    [SerializeField] Slider phaseSlider;
    [SerializeField] TMP_InputField wavelengthInput;
    [SerializeField] TMP_InputField amplitudeInput;
    [SerializeField] TMP_InputField phaseInput;

    private void Start()
    {
        InitEventListeners();
    }

    void InitEventListeners()
    {
        Photon photon = WaveOperationTool.Instance.GetPhoton();
        wavelengthSlider.onValueChanged.AddListener((float arg0) => { photon.SetWavelength(wavelengthSlider.value); WaveOperationTool.Instance.UpdateVisuals(); });
        amplitudeSlider.onValueChanged.AddListener((float arg0) => { photon.SetAmplitude(amplitudeSlider.value); WaveOperationTool.Instance.UpdateVisuals(); });
        phaseSlider.onValueChanged.AddListener((float arg0) => { photon.SetPhase(phaseSlider.value); WaveOperationTool.Instance.UpdateVisuals(); });
        wavelengthInput.onValueChanged.AddListener((string arg0) => { photon.SetWavelength(float.Parse(wavelengthInput.text)); WaveOperationTool.Instance.UpdateVisuals(); });
        amplitudeInput.onValueChanged.AddListener((string arg0) => { photon.SetAmplitude(float.Parse(amplitudeInput.text)); WaveOperationTool.Instance.UpdateVisuals(); });
        phaseInput.onValueChanged.AddListener((string arg0) => { photon.SetPhase(float.Parse(phaseInput.text)); WaveOperationTool.Instance.UpdateVisuals(); });
    }

    public void UpdateSliderVisuals(Photon photon)
    {
        wavelengthSlider.value = photon.GetWaveLength();
        amplitudeSlider.value = photon.GetAmplitude();
        phaseSlider.value = photon.GetPhase();
        wavelengthInput.text = photon.GetWaveLength().ToString();
        amplitudeInput.text = photon.GetAmplitude().ToString();
        phaseInput.text = photon.GetPhase().ToString();
    }
}
