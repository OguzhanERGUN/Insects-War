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

		// Hedefe olan uzaklýða göre süreyi hesapla (sabit hýzla)
		float distance = Vector3.Distance(startPoint, endPoint);
		height = Mathf.Clamp(distance * 0.25f, 1f, 10f); // mesafe * katsayý, min/max ile sýnýrlama
		duration = distance / speed; // süre = mesafe / hýz
	}


	private void Update()
	{
		if (timeElapsed > duration) return;

		timeElapsed += Time.deltaTime;
		float t = Mathf.Clamp01(timeElapsed / duration);

		// Yatay pozisyon
		Vector3 currentPos = Vector3.Lerp(startPoint, endPoint, t);

		// Dikey parabolik yükselme
		float arc = -4 * height * t * t + 4 * height * t;
		currentPos.y += arc;

		// Yönü güncelle
		Vector3 direction = currentPos - previousPosition;
		if (direction != Vector3.zero)
			transform.rotation = Quaternion.LookRotation(direction);

		transform.position = currentPos;
		previousPosition = currentPos;

		if (t >= 1f)
		{
			Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
			Debug.Log("Füze hedefe ulaþtý!");
			Destroy(gameObject); // Füze nesnesini yok et
			Debug.Log("Füze hedefe ulaþtý!");
		}
	}
}
