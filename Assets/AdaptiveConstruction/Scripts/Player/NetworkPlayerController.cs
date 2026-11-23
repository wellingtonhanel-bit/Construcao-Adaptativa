using Unity.Netcode;
using UnityEngine;

namespace AdaptiveConstruction
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class NetworkPlayerController : NetworkBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform cameraPitchPivot;
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float jumpHeight = 1.4f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float mouseSensitivity = 2f;

        private CharacterController characterController;
        private float verticalVelocity;
        private float cameraPitch;
        private Vector2 pendingMove;
        private float pendingYaw;
        private bool pendingJump;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        public override void OnNetworkSpawn()
        {
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(IsOwner);

            if (IsOwner)
                Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (!IsSpawned)
                return;

            if (IsOwner)
            {
                ReadOwnerInput();
                SubmitInputServerRpc(pendingMove, pendingYaw, pendingJump);
                pendingJump = false;
            }

            if (IsServer)
                SimulateMovement();
        }

        private void ReadOwnerInput()
        {
            pendingMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            pendingMove = Vector2.ClampMagnitude(pendingMove, 1f);

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            pendingYaw = transform.eulerAngles.y + mouseX;
            cameraPitch = Mathf.Clamp(cameraPitch - mouseY, -75f, 75f);

            if (cameraPitchPivot != null)
                cameraPitchPivot.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);

            pendingJump = Input.GetKeyDown(KeyCode.Space);
        }

        [ServerRpc]
        private void SubmitInputServerRpc(Vector2 move, float yaw, bool jump)
        {
            pendingMove = Vector2.ClampMagnitude(move, 1f);
            pendingYaw = yaw;
            pendingJump = jump;
        }

        private void SimulateMovement()
        {
            transform.rotation = Quaternion.Euler(0f, pendingYaw, 0f);

            if (characterController.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (pendingJump && characterController.isGrounded)
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            verticalVelocity += gravity * Time.deltaTime;
            Vector3 move = transform.right * pendingMove.x + transform.forward * pendingMove.y;
            move *= moveSpeed;
            move.y = verticalVelocity;
            characterController.Move(move * Time.deltaTime);
            pendingJump = false;
        }
    }
}
