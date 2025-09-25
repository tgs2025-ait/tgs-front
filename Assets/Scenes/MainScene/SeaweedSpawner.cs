using System.Collections;
using UnityEngine;

public class RandomNegativeZSpawner : MonoBehaviour
{
	[Header("シャチのTransform")]
	[SerializeField] private GameObject orca;
	[Header("海藻のPrefab")]
	[SerializeField] private GameObject spawnPrefab;

	[Header("Spawn Position X Range")] 
	[SerializeField] private Vector2 xRange = new Vector2(-10f, 10f);

	[Header("Spawn Interval Range (seconds)")]
	[SerializeField] private Vector2 spawnIntervalRange = new Vector2(0.5f, 2f);

	[Header("Movement & Distances")]
	[SerializeField] private float moveSpeed = 5f;
	[SerializeField] private float spawnOffsetZ = 30f; // spawn at A.z + spawnOffsetZ
	[SerializeField] private float despawnBehindA = 30f; // destroy when <= A.z - despawnBehindA

	[SerializeField] private bool autoStart = true;

	private Coroutine spawnRoutine;

	private void OnEnable()
	{
		if (autoStart)
		{
			StartSpawning();
		}
	}

	private void OnDisable()
	{
		if (spawnRoutine != null)
		{
			StopCoroutine(spawnRoutine);
			spawnRoutine = null;
		}
	}

	public void StartSpawning()
	{
		if (spawnRoutine == null)
		{
			spawnRoutine = StartCoroutine(SpawnLoop());
		}
	}

	public void StopSpawning()
	{
		if (spawnRoutine != null)
		{
			StopCoroutine(spawnRoutine);
			spawnRoutine = null;
		}
	}

	private IEnumerator SpawnLoop()
	{
		while (true)
		{
			var waitSeconds = Random.Range(
				Mathf.Min(spawnIntervalRange.x, spawnIntervalRange.y),
				Mathf.Max(spawnIntervalRange.x, spawnIntervalRange.y)
			);
			yield return new WaitForSeconds(waitSeconds);

			if (orca == null || spawnPrefab == null)
			{
				continue;
			}

			float x = Random.Range(
				Mathf.Min(xRange.x, xRange.y),
				Mathf.Max(xRange.x, xRange.y)
			);
			Vector3 spawnPos = new Vector3(x, orca.transform.position.y, orca.transform.position.z + spawnOffsetZ);

			GameObject instance = Instantiate(spawnPrefab, spawnPos, Quaternion.identity, this.transform);
			instance.SetActive(true);
			var mover = instance.GetComponent<MoveNegativeZUntilBehind>();
			if (mover == null)
			{
				mover = instance.AddComponent<MoveNegativeZUntilBehind>();
			}
			mover.Initialize(moveSpeed, orca.transform, despawnBehindA);
		}
	}

	private void OnValidate()
	{
		// Ensure sensible values in editor
		moveSpeed = Mathf.Max(0f, moveSpeed);
		spawnOffsetZ = Mathf.Max(0f, spawnOffsetZ);
		despawnBehindA = Mathf.Max(0f, despawnBehindA);
	}
}

public class MoveNegativeZUntilBehind : MonoBehaviour
{
	private float speed;
	private Transform referenceA;
	private float behindDistance;
	private bool initialized;

	public void Initialize(float moveSpeed, Transform reference, float distanceBehind)
	{
		speed = Mathf.Max(0f, moveSpeed);
		referenceA = reference;
		behindDistance = Mathf.Max(0f, distanceBehind);
		initialized = true;
	}

	private void Update()
	{
		if (!initialized || referenceA == null)
		{
			Destroy(gameObject);
			return;
		}

		transform.position += Vector3.back * speed * Time.deltaTime; // move along -Z

		if (transform.position.z <= referenceA.position.z - behindDistance)
		{
			Destroy(gameObject);
		}
	}
}


