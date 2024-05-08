using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCrawlerSpawner : MonoBehaviour
{

    [SerializeField] private List<Transform> walls = new List<Transform>();
    [SerializeField] private GameObject wallcrawler_prefab;
    [SerializeField] private int noofcrawlers;
    [SerializeField] private int interval = 2;

 
    void Start()
    {
        StartCoroutine(SpanwnCrawlers());
    }

    private IEnumerator SpanwnCrawlers()
    {

        for (int i = 0; i < noofcrawlers; i++)
        {
            yield return new WaitForSeconds(interval);
            int randomNum = Random.Range(0, walls.Count); // Choose a random Wall
            Debug.Log(randomNum);
            SpawnObjectOnWall(getSpawnPoint(randomNum), walls[randomNum].forward);


        }

    }

    void SpawnObjectOnWall(Vector3 spawnPosition, Vector3 wallNormal)
    {
        // Instantiate the object at the spawn position
        GameObject spawnedObject = Instantiate(wallcrawler_prefab, spawnPosition, Quaternion.identity);

        // Calculate the rotation to align the object's forward vector with the wall's normal vector
        Quaternion rotation = Quaternion.LookRotation(wallNormal);

        Quaternion headRotation = Quaternion.LookRotation(Vector3.down);

        // Set the object's rotation
        spawnedObject.transform.rotation = rotation * headRotation;  //* Quaternion.Euler(90,0,0);
    }


    private Vector3 getSpawnPoint(int randomNum)
    {
        float minX = Mathf.Infinity;
        float maxX = Mathf.NegativeInfinity;
        float minZ = Mathf.Infinity;
        float maxZ = Mathf.NegativeInfinity;

        foreach (var item in walls)
        {
            minX = Mathf.Min(item.position.x, minX);
            maxX = Mathf.Max(item.position.x, maxX);
            minZ = Mathf.Min(item.position.z, minZ);
            maxZ = Mathf.Max(item.position.z, maxZ);
        }


        var wallbounds = walls[randomNum].GetComponent<Renderer>().bounds;

        Vector3 newVector = new Vector3(wallbounds.center.x + 0.05f , wallbounds.max.y, wallbounds.center.z+ 0.05f);

        return newVector;
    }

    public void GetWallpositions()
    {
        Transform[] allTransforms = FindObjectsOfType<Transform>();

        foreach (Transform tr in allTransforms)
        {
            // Check if the GameObject's name or "WALL_FACE_EffectMesh"
            if (tr.name.ToLower().Contains("wall_face_effectmesh"))
            {
                walls.Add(tr);
            }
        }
       
    }
}
