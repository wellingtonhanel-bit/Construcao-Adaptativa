using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace AdaptiveConstruction
{
    public sealed class SafeZoneController : NetworkBehaviour
    {
        [SerializeField] private Vector3 zoneCenter;
        [SerializeField] private float startingRadius = 250f;
        [SerializeField] private float finalRadius = 15f;
        [SerializeField] private float shrinkDuration = 600f;

        public NetworkVariable<float> CurrentRadius = new NetworkVariable<float>(
            250f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<Vector3> Center = new NetworkVariable<Vector3>(
            Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
                return;

            Center.Value = zoneCenter;
            CurrentRadius.Value = startingRadius;
            StartCoroutine(ShrinkZone());
        }

        private IEnumerator ShrinkZone()
        {
            float elapsed = 0f;
            while (elapsed < shrinkDuration)
            {
                elapsed += Time.deltaTime;
                CurrentRadius.Value = Mathf.Lerp(startingRadius, finalRadius, elapsed / shrinkDuration);
                yield return null;
            }

            CurrentRadius.Value = finalRadius;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(Application.isPlaying ? Center.Value : zoneCenter,
                Application.isPlaying ? CurrentRadius.Value : startingRadius);
        }
    }
}
