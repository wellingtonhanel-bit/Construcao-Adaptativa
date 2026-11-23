using Unity.Netcode;
using UnityEngine;

namespace AdaptiveConstruction
{
    public sealed class NetworkHealth : NetworkBehaviour
    {
        [SerializeField, Min(1)] private int maxHealth = 100;

        public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>(
            100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        public override void OnNetworkSpawn()
        {
            if (IsServer)
                CurrentHealth.Value = maxHealth;
        }

        public void ApplyDamage(int amount)
        {
            if (!IsServer || amount <= 0 || CurrentHealth.Value <= 0)
                return;

            CurrentHealth.Value = Mathf.Max(0, CurrentHealth.Value - amount);
            if (CurrentHealth.Value == 0 && NetworkObject != null && NetworkObject.IsSpawned)
                NetworkObject.Despawn(true);
        }
    }
}
