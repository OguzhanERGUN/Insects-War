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

	private void Update()
	{
		if (!IsOwner) return;
		GetMovementInput();
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
		// Kameranýn yönünü al ve yatay düzlemde projeksiyon yap
		Vector3 cameraForward = CameraTransform.forward;
		cameraForward.y = 0;
		cameraForward.Normalize();

		Vector3 cameraRight = CameraTransform.right;
		cameraRight.y = 0;
		cameraRight.Normalize();

		// Hareket yönünü kamera eksenine göre hesapla
		float moveX = Input.GetAxis("Horizontal");
		float moveZ = Input.GetAxis("Vertical");

		Vector3 moveDirection = cameraForward * moveZ + cameraRight * moveX;
		moveDirection.Normalize();

		if (moveDirection != Vector3.zero)
		{
			MoveServerRpc(moveDirection);
			RotateBodyServerRpc(moveDirection);
		}
	}

	[ServerRpc]
	private void MoveServerRpc(Vector3 moveDirection)
	{
		transform.position += moveDirection * moveSpeed * Time.deltaTime;
	}

	[ServerRpc]
	private void RotateBodyServerRpc(Vector3 moveDirection)
	{
		Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
		bodyRotation.Value = targetRotation;
	}
}
