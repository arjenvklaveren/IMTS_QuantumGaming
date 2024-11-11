using Game.Data;
using SadUtils;
using UnityEngine;

public class WaveOperationTool : Singleton<WaveOperationTool>
{
    [SerializeField] PhotonWaveVisualiser photonWaveVisualiser;
    [SerializeField] WaveSliderManager waveSliderManager;
    [SerializeField] BlochSphereVisualiser blochSphereVisualiser;
    [SerializeField] DiracNotationVisualiser diracNotationVisualiser;

    Photon photon;

    protected override void Awake()
    {
        SetInstance(this);
        photon = new Photon(Vector2Int.zero, Orientation.Up);
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        photonWaveVisualiser.UpdateWaveVisual(photon);
        waveSliderManager.UpdateSliderVisuals(photon);
        blochSphereVisualiser.UpdateBlochVisual(photon);
        diracNotationVisualiser.UpdateDiracVisuals(photon);
    }

    public Photon GetPhoton() { return photon; }
}
