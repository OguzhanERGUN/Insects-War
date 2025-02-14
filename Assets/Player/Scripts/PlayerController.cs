using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
	public float moveSpeed = 5f;
	public float bodyRotationSpeed = 5f;

	[SerializeField] private Transform BodyTransform;

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
		float moveX = Input.GetAxis("Horizontal");
		float moveZ = Input.GetAxis("Vertical");

		Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
		if (moveDirection != Vector3.zero)
		{
			MoveServerRpc(moveDirection);
			RotateBodyServerRpc(moveDirection);
		}
	}

	[ServerRpc]
	private void MoveServerRpc(Vector3 movedirection)
	{
		transform.position += movedirection * moveSpeed * Time.deltaTime;
	}

	[ServerRpc]
	private void RotateBodyServerRpc(Vector3 movedirection)
	{
		Quaternion targetRotation = Quaternion.LookRotation(movedirection, Vector3.up);
		bodyRotation.Value = targetRotation; // Dönüþü NetworkVariable ile ayarla
	}


}
