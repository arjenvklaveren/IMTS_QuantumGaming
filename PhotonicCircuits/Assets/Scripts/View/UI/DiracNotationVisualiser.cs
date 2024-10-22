using TMPro;
using UnityEngine;

public class DiracNotationVisualiser : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI diracText;

    public void UpdateDiracVisuals(Photon photon)
    {
        float amplitude = photon.GetAmplitude();
        float phase = photon.GetPhase();
        float zeroStateProb = (float)System.Math.Round(1.0f - amplitude, 2);
        float oneStateProb = (float)System.Math.Round(1.0f - zeroStateProb, 2);
        string operationString = " + ";

        if (amplitude != 0 && amplitude != 1.0f)
        {
            if (phase > (Mathf.PI / 2) && phase < (Mathf.PI * 1.5f))
            {
                operationString = " - ";
            }
        }

        diracText.text = "∣Ψ⟩ = <color=red>" + zeroStateProb + "</color> ∣0⟩" + operationString + "<color=red>" + oneStateProb + "</color> ∣1⟩";
    }
}
