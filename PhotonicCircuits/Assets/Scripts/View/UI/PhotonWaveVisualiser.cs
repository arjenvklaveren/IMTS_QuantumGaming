using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonWaveVisualiser : MonoBehaviour
{
    [SerializeField] Image photonVisual;
    Material matCopy;

    public void UpdateWaveVisual(Photon photon)
    {
        if (matCopy == null) matCopy = new Material(photonVisual.material);
        matCopy.SetFloat("_Wavelength", photon.GetWaveLength());
        matCopy.SetFloat("_Amplitude", photon.GetAmplitude());
        matCopy.SetFloat("_Phase", photon.GetPhase());
        photonVisual.material = matCopy;
    }
}
