using System;
using UnityEngine;

namespace AdaptiveConstruction
{
    public sealed class SafeZoneHudData : MonoBehaviour
    {
        [SerializeField] private SafeZoneController zone;
        public float Radius => zone != null ? zone.CurrentRadius.Value : 0f;
        public event Action<float> RadiusChanged;

        private void OnEnable()
        {
            if (zone != null)
                zone.CurrentRadius.OnValueChanged += OnRadiusChanged;
        }

        private void OnDisable()
        {
            if (zone != null)
                zone.CurrentRadius.OnValueChanged -= OnRadiusChanged;
        }

        private void OnRadiusChanged(float previous, float current)
        {
            RadiusChanged?.Invoke(current);
        }
    }
}
