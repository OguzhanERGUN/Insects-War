using Unity.Netcode;
using UnityEngine;

public class MouseandAimController : NetworkBehaviour
{
	[Header("Movement Settings")]
	public float mouseSensitivity = 100f;

	[Header("References")]
	public Transform turretTransform;
	public Transform gunTransform;
	public Transform cameraTransform;

	[Header("Rotation Limits")]
	public float minGunAngle = -10f;
	public float maxGunAngle = 30f;

	private float xRotation = 0f;
	private Quaternion lastGunRotation;
	private Quaternion lastTurretRotation;

	// NetworkVariables for turret and gun rotations
	private NetworkVariable<Quaternion> gunRotation = new NetworkVariable<Quaternion>(
		Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

	private NetworkVariable<Quaternion> turretRotation = new NetworkVariable<Quaternion>(
		Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

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

		float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
		float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

		// Turret rotation (Y-axis)
		turretTransform.Rotate(Vector3.up * mouseX);
		Quaternion newTurretRotation = turretTransform.rotation;

		// Update turret rotation only if it changed significantly
		if (Quaternion.Angle(lastTurretRotation, newTurretRotation) > 0.5f)
		{
			lastTurretRotation = newTurretRotation;
			UpdateTurretRotationServerRpc(newTurretRotation);
		}

		// Gun rotation (X-axis)
		xRotation = Mathf.Clamp(xRotation - mouseY, minGunAngle, maxGunAngle);
		Quaternion newGunRotation = Quaternion.Euler(xRotation, 0f, 0f);

		if (Quaternion.Angle(lastGunRotation, newGunRotation) > 0.5f)
		{
			lastGunRotation = newGunRotation;
			UpdateGunRotationServerRpc(newGunRotation);
		}

		// Update camera position based on gun
		cameraTransform.position = gunTransform.position - gunTransform.forward * 4f + Vector3.up * 1.5f;
		cameraTransform.rotation = Quaternion.LookRotation(gunTransform.forward);
	}

	private void FixedUpdate()
	{
		// Synchronize turret and gun rotations
		if (IsClient || IsServer)
		{
			turretTransform.rotation = Quaternion.Slerp(turretTransform.rotation, turretRotation.Value, Time.deltaTime * 5f);
			gunTransform.localRotation = Quaternion.Slerp(gunTransform.localRotation, gunRotation.Value, Time.deltaTime * 20f);

		}
	}

	[ServerRpc]
	private void UpdateTurretRotationServerRpc(Quaternion rotation)
	{
		turretRotation.Value = rotation;
	}

	[ServerRpc]
	private void UpdateGunRotationServerRpc(Quaternion rotation)
	{
		gunRotation.Value = rotation;
	}
}
