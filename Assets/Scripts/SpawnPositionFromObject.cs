using UnityEngine;

public class SpawnPositionFromObject : MonoBehaviour
{

    [SerializeField]
    private GameObject spawnPositionReference;
    [SerializeField] 
    private float xOffset, yOffset, zOffset;
    private Vector3 offsetSpawnPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPositionReference = GameObject.FindGameObjectWithTag("MainCamera");
        SetSpawnPosition();
    }

    public void SetSpawnPosition()
    {
        offsetSpawnPosition = (spawnPositionReference.transform.position);
        offsetSpawnPosition += spawnPositionReference.transform.forward * zOffset;
        offsetSpawnPosition += spawnPositionReference.transform.right * xOffset;
        offsetSpawnPosition += spawnPositionReference.transform.up * yOffset;
        transform.position = offsetSpawnPosition;
    }
}
