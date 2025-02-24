using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerUIController : NetworkBehaviour
{
	[SerializeField] private Canvas playerCanvas;

	public override void OnNetworkSpawn()
	{
		if (!IsOwner)
		{
			playerCanvas.enabled = false;
		}
	}
}
