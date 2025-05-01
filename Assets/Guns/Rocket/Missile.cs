using UnityEngine;

public class Missile : MonoBehaviour
{
	private Vector3 startPoint;
	private Vector3 endPoint;
	private float height;
	private float duration;
	private float timeElapsed;
	[SerializeField] private GameObject explosionEffectPrefab;


	private Vector3 previousPosition;

	public void Launch(Vector3 target, float speed = 20f, float arcHeight = 5f)
	{
		startPoint = transform.position;
		endPoint = target;
		timeElapsed = 0f;
		previousPosition = startPoint;

		// Hedefe olan uzakl��a g�re s�reyi hesapla (sabit h�zla)
		float distance = Vector3.Distance(startPoint, endPoint);
		height = Mathf.Clamp(distance * 0.25f, 1f, 10f); // mesafe * katsay�, min/max ile s�n�rlama
		duration = distance / speed; // s�re = mesafe / h�z
	}


	private void Update()
	{
		if (timeElapsed > duration) return;

		timeElapsed += Time.deltaTime;
		float t = Mathf.Clamp01(timeElapsed / duration);

		// Yatay pozisyon
		Vector3 currentPos = Vector3.Lerp(startPoint, endPoint, t);

		// Dikey parabolik y�kselme
		float arc = -4 * height * t * t + 4 * height * t;
		currentPos.y += arc;

		// Y�n� g�ncelle
		Vector3 direction = currentPos - previousPosition;
		if (direction != Vector3.zero)
			transform.rotation = Quaternion.LookRotation(direction);

		transform.position = currentPos;
		previousPosition = currentPos;

		if (t >= 1f)
		{
			Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
			Debug.Log("F�ze hedefe ula�t�!");
			Destroy(gameObject); // F�ze nesnesini yok et
			Debug.Log("F�ze hedefe ula�t�!");
		}
	}
}
