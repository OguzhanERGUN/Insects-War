using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

public class GunInput : NetworkBehaviour
{
	[Header("Silah Ayarlar�")]
	public float fireRange = 100f;
	public int damage = 20;
	public LayerMask hitLayers;

	[Header("Referanslar")]
	[SerializeField] private Camera playerCamera; // Oyuncu kameras�
	[SerializeField] private GameObject missilePrefab;


	[SerializeField] private float missileFlightTime = 1.5f;
	[SerializeField] private float missileArcHeight = 5f;

	private void Update()
	{
		if (!IsOwner) return;

		if (Input.GetMouseButtonDown(0)) // Sol t�k ile ate� et
		{
			Fire();
		}

		if (Input.GetMouseButtonDown(1)) // Sa� t�k ile f�ze at
			LaunchMissile();
	}

	private void Fire()
	{
		Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Ekran�n ortas�ndan ray at
		if (Physics.Raycast(ray, out RaycastHit hit, fireRange, hitLayers))
		{
			if (hit.transform.parent.parent.TryGetComponent(out HealthController targetPlayer))
			{
				Debug.Log($"Vuruldu: {targetPlayer.name}");
				targetPlayer.TakeDamage(damage);
			}
		}
	}
	private void LaunchMissile()
	{
		Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out RaycastHit hit, 100f))
		{
			GameObject missileObj = Instantiate(missilePrefab, transform.position, Quaternion.identity);
			missileObj.GetComponent<Missile>().Launch(hit.point, missileFlightTime, missileArcHeight);
		}
	}
}
