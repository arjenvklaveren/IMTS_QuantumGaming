public class Photon
{
    float waveLength;
    float amplitude;
    float phase;

    public Photon(float waveLength, float amplitude, float phase)
    {
        this.waveLength = waveLength;
        this.amplitude = amplitude;
        this.phase = phase;
    }
    public Photon() { }

    public void SetWaveLength(float waveLength) { this.waveLength = waveLength; }
    public void SetAmplitude(float amplitude) { this.amplitude = amplitude; }
    public void SetPhase(float phase) { this.phase = phase; }
    public float GetWaveLength() { return waveLength; }
    public float GetAmplitude() { return amplitude; }
    public float GetPhase() { return phase; }
}
