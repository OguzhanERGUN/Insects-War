using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;

public class PlayerCameraController : NetworkBehaviour
{
	[SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
	[SerializeField] private AudioListener audioListener;



	public override void OnNetworkSpawn()
	{
		if (IsOwner)
		{
			cinemachineVirtualCamera.Priority = 1;
			audioListener.enabled = true;
		}
		else
		{
			cinemachineVirtualCamera.Priority=0;
		}
	}
}
