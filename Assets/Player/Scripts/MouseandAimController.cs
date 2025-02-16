using Unity.Netcode;
using UnityEngine;

public class MouseandAimController : NetworkBehaviour
{
	[Header("Movement Settings")]
	public float mouseSensitivity = 100f;

	[Header("References")]
	public Transform turretTransform; // Kafa kýsmý
	public Transform gunTransform;    // Silah kýsmý
	public Transform cameraTransform; // Kamera

	[Header("Rotation Limits")]
	public float minGunAngle = -10f;  // Namlu aþaðý limit
	public float maxGunAngle = 30f;   // Namlu yukarý limit

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

		// Turret sað-sol hareketi (Mouse X)
		turretTransform.Rotate(Vector3.up * mouseX);

		// Gun yukarý-aþaðý hareketi (Mouse Y)
		xRotation -= mouseY;
		xRotation = Mathf.Clamp(xRotation, minGunAngle, maxGunAngle);
		gunTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

		// Kamera güncelle (her zaman silahý takip eder)
		cameraTransform.position = gunTransform.position - gunTransform.forward * 4f + Vector3.up * 1.5f;
		cameraTransform.rotation = Quaternion.LookRotation(gunTransform.forward);
	}
}
