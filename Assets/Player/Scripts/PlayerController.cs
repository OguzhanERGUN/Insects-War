using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
	public float moveSpeed = 5f; 
	public float headRotationSpeed = 40f; 
	[SerializeField] private Transform BodyTransform;
	[SerializeField] private Transform HeadTransform;
	[SerializeField] private Transform GunTransform;

	private void Start()
	{
	}

	private void Update()
	{
		if (!IsOwner) return;
		GetMovementInput();
		GetRotationInput();
	}

	private void GetRotationInput()
	{
		if (Input.GetKey(KeyCode.E))
		{
			RotateItemServerRpc();
		}
	}

	private void GetMovementInput()
	{

		float moveX = Input.GetAxis("Horizontal"); // A (-1) ve D (+1)
		float moveZ = Input.GetAxis("Vertical");   // W (+1) ve S (-1)

		Vector3 move = new Vector3(moveX, 0, moveZ) * moveSpeed * Time.deltaTime;
		MoveServerRpc(move);
		Debug.Log(NetworkBehaviourId + " " + move);
	}


	[ServerRpc]
	private void MoveServerRpc(Vector3 move)
	{
		transform.position = transform.position + move;
	}


	[ServerRpc]
	private void RotateItemServerRpc()
	{
		RotateHeadClientRpc();
	}

	[ClientRpc]
	private void RotateHeadClientRpc()
	{
		HeadTransform.Rotate(Vector3.up * headRotationSpeed * Time.deltaTime);
	}
}
