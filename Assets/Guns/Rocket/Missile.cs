using UnityEngine;

public class Missile : MonoBehaviour
{
	private Vector3 startPoint;
	private Vector3 endPoint;
	private float height;
	private float duration;
	private float timeElapsed;

	private Vector3 previousPosition;

	public void Launch(Vector3 target, float flightTime = 1.5f, float arcHeight = 5f)
	{
		startPoint = transform.position;
		endPoint = target;
		duration = flightTime;
		height = arcHeight;
		timeElapsed = 0f;
		previousPosition = startPoint;
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
			// Hedefe ula��ld�ysa efekt vs.
			Debug.Log("F�ze hedefe ula�t�!");
		}
	}
}
