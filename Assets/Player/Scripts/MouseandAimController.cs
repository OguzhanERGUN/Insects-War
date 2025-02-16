using Unity.Netcode;
using UnityEngine;

public class MouseandAimController : NetworkBehaviour
{
	[Header("Movement Settings")]
	public float mouseSensitivity = 100f;

	[Header("References")]
	public Transform turretTransform; // Kafa k�sm�
	public Transform gunTransform;    // Silah k�sm�
	public Transform cameraTransform; // Kamera

	[Header("Rotation Limits")]
	public float minGunAngle = -10f;  // Namlu a�a�� limit
	public float maxGunAngle = 30f;   // Namlu yukar� limit

	private float xRotation = 0f;

	private void Start()
	{
		if (!IsOwner)
		{
			cameraTransform.gameObject.SetActive(false);
			return;
		}

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	private void Update()
	{
		if (!IsOwner) return;

		// Mouse girdilerini al
		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

		// Turret sa�-sol hareketi (Mouse X)
		turretTransform.Rotate(Vector3.up * mouseX);

		// Gun yukar�-a�a�� hareketi (Mouse Y)
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, minGunAngle, maxGunAngle);
		gunTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

		// Kamera g�ncelle (her zaman silah� takip eder)
		cameraTransform.position = gunTransform.position - gunTransform.forward * 4f + Vector3.up * 1.5f;
		cameraTransform.rotation = Quaternion.LookRotation(gunTransform.forward);
	}
}
