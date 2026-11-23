using Unity.Netcode;
using UnityEngine;

namespace AdaptiveConstruction
{
    public sealed class NetworkBuildSystem : NetworkBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private NetworkObject[] buildPrefabs;
        [SerializeField] private LayerMask placementSurfaceMask = ~0;
        [SerializeField] private float buildRange = 12f;
        [SerializeField] private float gridSize = 2f;

        private void Update()
        {
            if (!IsSpawned || !IsOwner || playerCamera == null || buildPrefabs == null || buildPrefabs.Length == 0)
                return;

            if (Input.GetMouseButtonDown(0) && Physics.Raycast(playerCamera.transform.position,
                    playerCamera.transform.forward, out RaycastHit hit, buildRange, placementSurfaceMask))
            {
                Vector3 snapped = new Vector3(
                    Mathf.Round(hit.point.x / gridSize) * gridSize,
                    Mathf.Round(hit.point.y / gridSize) * gridSize,
                    Mathf.Round(hit.point.z / gridSize) * gridSize);
                PlaceBuildServerRpc(snapped, 0);
            }
        }

        [ServerRpc]
        private void PlaceBuildServerRpc(Vector3 position, int prefabIndex)
        {
            if (buildPrefabs == null || prefabIndex < 0 || prefabIndex >= buildPrefabs.Length)
                return;

            // Validate range on the server; clients cannot place structures arbitrarily far away.
            if (Vector3.Distance(transform.position, position) > buildRange + gridSize)
                return;

            NetworkObject piece = Instantiate(buildPrefabs[prefabIndex], position, Quaternion.identity);
            piece.Spawn(true);
        }
    }
}
