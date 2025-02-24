using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class GunInput : NetworkBehaviour
{
	[Header("Silah Ayarlarý")]
	public float fireRange = 100f;
	public int damage = 20;
	public LayerMask hitLayers;

	[Header("Referanslar")]
	[SerializeField] private Camera playerCamera; // Oyuncu kamerasý

	private void Update()
	{
		if (!IsOwner) return;

		if (Input.GetMouseButtonDown(0)) // Sol týk ile ateþ et
		{
			Fire();
		}
	}

	private void Fire()
	{
		Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Ekranýn ortasýndan ray at
		if (Physics.Raycast(ray, out RaycastHit hit, fireRange, hitLayers))
		{
			if (hit.transform.parent.parent.TryGetComponent(out HealthController targetPlayer))
			{
				Debug.Log($"Vuruldu: {targetPlayer.name}");
				targetPlayer.TakeDamage(damage);
			}
		}
	}
}
