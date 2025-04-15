using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
	public float moveSpeed = 5f;
	public float bodyRotationSpeed = 5f;

	[SerializeField] private Transform BodyTransform;
	[SerializeField] private Transform CameraTransform;

	private NetworkVariable<Quaternion> bodyRotation = new NetworkVariable<Quaternion>(
		Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	public float jumpForce = 5f;
	public float runMultiplier = 2f;

	private bool isGrounded;
	private Rigidbody rb;


	private void Start()
	{
		if (IsOwner)
		{
			rb = GetComponent<Rigidbody>();
		}
	}
	private void Update()
	{
		if (!IsOwner) return;

		GetMovementInput();

		if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
		{
			JumpServerRpc();
		}
	}

	private void LateUpdate()
	{
		// Tüm istemciler için akýcý dönüþ
		if (IsServer || IsClient)
		{
			BodyTransform.rotation = Quaternion.Slerp(BodyTransform.rotation, bodyRotation.Value, bodyRotationSpeed * Time.deltaTime);
		}
	}

	private void GetMovementInput()
	{
		// Kamera yönü hesaplamalarý
		Vector3 cameraForward = CameraTransform.forward;
		cameraForward.y = 0;
		cameraForward.Normalize();

		Vector3 cameraRight = CameraTransform.right;
		cameraRight.y = 0;
		cameraRight.Normalize();

		// Hareket yönü
		float moveX = Input.GetAxis("Horizontal");
		float moveZ = Input.GetAxis("Vertical");

		Vector3 moveDirection = cameraForward * moveZ + cameraRight * moveX;
		moveDirection.Normalize();

		if (moveDirection != Vector3.zero)
		{
			if (CanMoveForward(transform.forward))
			{
				float speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? runMultiplier : 1f;
				MoveServerRpc(moveDirection, speedMultiplier);
				RotateBodyServerRpc(moveDirection);
			}
		}
	}

	[ServerRpc]
	private void MoveServerRpc(Vector3 moveDirection, float speedMultiplier)
	{
		transform.position += moveDirection * moveSpeed * speedMultiplier * Time.deltaTime;
	}

	[ServerRpc]
	private void RotateBodyServerRpc(Vector3 moveDirection)
	{
		Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
		bodyRotation.Value = targetRotation;
	}

	[ServerRpc]
	private void JumpServerRpc()
	{
		if (rb != null && isGrounded)
		{
			rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
			isGrounded = false;
		}
	}


	private void OnCollisionStay(Collision collision)
	{
		if (IsOwner && collision.gameObject.CompareTag("Ground"))
		{
			isGrounded = true;
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (IsOwner && collision.gameObject.CompareTag("Ground"))
		{
			isGrounded = false;
		}
	}

	private bool CanMoveForward(Vector3 direction)
	{
		Ray ray = new Ray(transform.position + Vector3.up * 0.5f, direction);
		RaycastHit hit;
		float rayDistance = 1.5f; // Engel kontrol mesafesi

		if (Physics.Raycast(ray, out hit, rayDistance))
		{
			// Ýsteðe baðlý olarak engel tag kontrolü
			// if (hit.collider.CompareTag("Obstacle")) return false;
			return false; // Engel var, hareket etme
		}

		return true; // Engel yok, hareket serbest
	}
}
