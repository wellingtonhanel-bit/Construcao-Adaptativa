using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace AdaptiveConstruction
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class BasicBotController : NetworkBehaviour
    {
        [SerializeField] private float retargetInterval = 1f;
        private NavMeshAgent agent;
        private float nextRetargetTime;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (!IsServer || !IsSpawned || Time.time < nextRetargetTime)
                return;

            nextRetargetTime = Time.time + retargetInterval;
            NetworkPlayerController[] players = FindObjectsByType<NetworkPlayerController>(FindObjectsSortMode.None);
            NetworkPlayerController nearest = null;
            float nearestDistance = float.PositiveInfinity;

            foreach (NetworkPlayerController player in players)
            {
                if (!player.IsSpawned)
                    continue;

                float distance = (player.transform.position - transform.position).sqrMagnitude;
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = player;
                }
            }

            if (nearest != null && agent.isOnNavMesh)
                agent.SetDestination(nearest.transform.position);
        }
    }
}
