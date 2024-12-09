using Game.Data;
using UnityEngine;

public class BlochSphereVisualiser : MonoBehaviour
{
    [SerializeField] Transform blochArrow;

    public void UpdateBlochVisual(Photon photon)
    {
        float amplitudeRotation = photon.GetAmplitude() * 180.0f;
        Vector3 sideAmplitudeVector = Vector3.down;
        sideAmplitudeVector = Quaternion.AngleAxis(amplitudeRotation, -Vector3.forward) * sideAmplitudeVector;

        float phaseRotation = Mathf.Rad2Deg * photon.GetPhase();
        Vector3 blochVector = Quaternion.AngleAxis(phaseRotation, Vector3.up) * sideAmplitudeVector;

        Quaternion correction = Quaternion.Euler(90, 0, 0);
        blochArrow.rotation = Quaternion.LookRotation(blochVector) * correction;
    }
}
