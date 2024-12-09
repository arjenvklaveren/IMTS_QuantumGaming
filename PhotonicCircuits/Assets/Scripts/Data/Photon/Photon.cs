using System;
using UnityEngine;

namespace Game.Data
{
    [System.Serializable]
    public class Photon
    {
        public event Action<OpticComponent> OnEnterComponent;
        public event Action<OpticComponent> OnExitComponent;

        public event Action OnDestroy;

        public GridData currentGrid;

        const float MIN_PHOTON_WAVELENGTH = 380;
        const float MAX_PHOTON_WAVELENGTH = 750;
        const float MAX_PHOTON_PHASE = 2f * Mathf.PI;
        const float MAX_PHOTON_POLARISATION = 90;

        float wavelength;
        float amplitude;
        float phase;
        float polarization;
        Vector2Int position;
        Orientation propagation;

        public Photon(
            GridData currentGrid,
            Vector2Int position,
            Orientation propagation,
            float wavelength = 666f,
            float amplitude = 1.0f,
            float phase = 0f,
            float polarization = 0f)
        {
            this.currentGrid = currentGrid;
            this.position = position;
            this.propagation = propagation;
            this.wavelength = wavelength;
            this.amplitude = amplitude;
            this.phase = phase;
            this.polarization = polarization;
        }

        public Photon Clone()
        {
            return new Photon(currentGrid, this.position, this.propagation, this.wavelength, this.amplitude, this.phase, this.polarization);
        }

        public void SetWavelength(float waveLength) { this.wavelength = Mathf.Clamp(waveLength, MIN_PHOTON_WAVELENGTH, MAX_PHOTON_WAVELENGTH); }
        public void SetAmplitude(float amplitude) { this.amplitude = Mathf.Clamp(amplitude, 0, 1.0f); }
        public void SetPhase(float phase) { this.phase = Mathf.Clamp(phase, 0, MAX_PHOTON_PHASE); }
        public void SetPolarisation(float polarisation) { this.polarization = Mathf.Clamp(polarisation, 0, MAX_PHOTON_POLARISATION); }
        public void SetPosition(Vector2Int position) { this.position = position; }
        public void SetPropagation(Orientation propagation) { this.propagation = propagation; }

        public float GetWaveLength() { return wavelength; }
        public float GetAmplitude() { return amplitude; }
        public float GetPhase() { return phase; }
        public float GetPolarisation() { return polarization; }
        public Vector2Int GetPosition() { return position; }

        public Orientation GetPropagation() => propagation;
        public Vector2 GetPropagationVector() => propagation.ToVector2();
        public Vector2Int GetPropagationIntVector() => propagation.ToVector2Int();

        public bool IsIndistinguishable(Photon photon)
        {
            return(
                position == photon.position &&
                propagation == photon.propagation &&
                wavelength == photon.wavelength &&
                amplitude == photon.amplitude &&
                phase == photon.phase &&
                polarization == photon.polarization);
        }

        public float GetWavelengthNormalized()
        {
            return (wavelength - MIN_PHOTON_WAVELENGTH) / (MAX_PHOTON_WAVELENGTH - MIN_PHOTON_WAVELENGTH);
        }
        public Color GetColor()
        {
            //academo.org/demos/wavelength-to-colour-relationship (converted to C# and adjusted using ChatGPT)
            float r = 0f, g = 0f, b = 0f;

            if (wavelength >= MIN_PHOTON_WAVELENGTH && wavelength <= MAX_PHOTON_WAVELENGTH)
            {
                if (wavelength < 440)
                {
                    r = Mathf.Lerp(1f, 0f, (wavelength - 380) / (440 - 380));
                    g = 0f;
                    b = 1f;
                }
                else if (wavelength < 490)
                {
                    r = 0f;
                    g = Mathf.Lerp(0f, 1f, (wavelength - 440) / (490 - 440));
                    b = 1f;
                }
                else if (wavelength < 510)
                {
                    r = 0f;
                    g = 1f;
                    b = Mathf.Lerp(1f, 0f, (wavelength - 490) / (510 - 490));
                }
                else if (wavelength < 580)
                {
                    r = Mathf.Lerp(0f, 1f, (wavelength - 510) / (580 - 510));
                    g = 1f;
                    b = 0f;
                }
                else if (wavelength < 645)
                {
                    r = 1f;
                    g = Mathf.Lerp(1f, 0f, (wavelength - 580) / (645 - 580));
                    b = 0f;
                }
                else
                {
                    r = 1f;
                    g = 0f;
                    b = 0f;
                }
            }
            else
            {
                r = 0f;
                g = 0f;
                b = 0f;
            }

            return new Color(r, g, b, GetAmplitude());
        }

        public void Move()
        {
            this.position += propagation.ToVector2Int();
        }

        public void RotateClockwise(int increments)
        {
            propagation = propagation.RotateClockwise(increments);
        }
        public void RotateCounterClockwise(int increments)
        {
            propagation = propagation.RotateCounterClockwise(increments);
        }

        public void RotatePolarisation(float angleDeg)
        {
            float newAngle = polarization + angleDeg;
            SetPolarisation(newAngle);
        }
        public void RotatePhase(float radians)
        {
            if(radians > MAX_PHOTON_PHASE)
            {
                radians = radians % MAX_PHOTON_PHASE;
            }
            float outRadians = phase + radians;
            if (outRadians > MAX_PHOTON_PHASE)
            {
                outRadians = (phase + radians) - MAX_PHOTON_PHASE;
            }
            SetPhase(outRadians);
        }

        // Event Triggering
        public void TriggerEnterComponent(OpticComponent component)
        {
            OnEnterComponent?.Invoke(component);
        }

        public void TriggerExitComponent(OpticComponent component)
        {
            OnExitComponent?.Invoke(component);
        }

        public void Destroy()
        {
            OnDestroy?.Invoke();
        }
    }
}
