using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class HealthController : NetworkBehaviour
{

	private NetworkVariable<int> health = new NetworkVariable<int>(100
		, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
	[SerializeField] private TextMeshProUGUI healthBar;



	private void Start()
	{
		health.OnValueChanged += OnHealthChanged;
		UpdateHealthBar();
	}

	private void OnHealthChanged(int oldValue, int newValue)
	{
		if (oldValue > newValue)
		{

		}
		else
		{

		}
		UpdateHealthBar();
	}


	public void TakeDamage(int damage)
	{
		TakeDamageServerRpc(damage);
	}

	[ServerRpc(RequireOwnership = false)]
	private void TakeDamageServerRpc(int damage)
	{
		health.Value -= damage;
	}


	private void UpdateHealthBar()
	{
		healthBar.text = health.Value.ToString();
	}
}
