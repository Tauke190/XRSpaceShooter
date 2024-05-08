using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public enum WaveType
    {
        CONTINOUS,
        WAIT,
    }
    
    public enum EnemyType
    {
        DRONE,
        CRAWLER
    }

    [System.Serializable]
    public class Wave
    {
        public EnemyType enemyType = EnemyType.DRONE;
        public GameObject enemyPrefab;
        public int count;
        public float spawnInterval;
        public float timebetweenattacks;
    }

    public static EnemyManager instance;
    public Transform spawnpoint;
    public List<Vector3> destinationPoints = new List<Vector3>();
    public List<Transform> walls = new List<Transform>();
    public Wave[] waves;
    public int waveIndex;
    public float timeBetweenWaves = 50f;
    private float waveCountdown;
    public WaveType wavetype = WaveType.CONTINOUS;

    public int destinationIndex;
   

    public int remainingEnemies = 0;

    public GameObject debugPoint;

    public GameObject debugParent;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        //StartCoroutine(SpawnWave(waves[waveIndex]));
    }

    void Update()
    {
        if (GameManager.instance._startgame && !GameManager.instance._gameover)
        {
            if (wavetype == WaveType.CONTINOUS)
            {
                if (waveCountdown <= 0)
                {
                    destinationIndex = 0;
                    //StopAllCoroutines();
                    StartCoroutine(SpawnWave(waves[waveIndex]));
                    timeBetweenWaves = 50f;
                    waveCountdown = timeBetweenWaves;
                }
                else
                {
                    waveCountdown -= Time.deltaTime;
                }
            }
            if (wavetype == WaveType.WAIT)
            {
                if (remainingEnemies <= 0 && waveCountdown <= 0)
                {
                    if (waveIndex < waves.Length)
                    {
                        if (waveIndex != 0) // If not the first wave then
                        {

                            RemoveAllDebugPoints();
                            destinationIndex = 0;
                            SetDroneDestinationPoint();
                        }
                        //StopAllCoroutines();
                        StartCoroutine(SpawnWave(waves[waveIndex]));

                    }
                    waveCountdown = timeBetweenWaves;
                }
                else if (remainingEnemies > 0)
                {
                    // Do nothing, waiting for all enemies to be killed
                }
                else
                {
                    waveCountdown -= Time.deltaTime;
                }
            }
        }
    }


    private  IEnumerator SpawnWave(Wave wave)
    {
        remainingEnemies = wave.count;


        for (int i = 0; i < wave.count; i++)
        {
            yield return new WaitForSeconds(wave.spawnInterval);
            switch (wave.enemyType)
            {
                case EnemyType.DRONE:
                    SpawnDrone(wave.enemyPrefab);
                    break;
                case EnemyType.CRAWLER:
                    int randomNum = Random.Range(0, walls.Count); // Choose a random Wall
                    SpawnCrawlerOnWall(wave.enemyPrefab,GetWallCrawlerSpawnPoint(randomNum), walls[randomNum].forward);
                    break;
            }
        }
        waveIndex++;
        
        if (waveIndex == waves.Length)
        {
            waveIndex = 0; 
        }
    }

    private void SpawnDrone(GameObject enemy)
    {
        Quaternion randomRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        GameObject spawnedEnemy = Instantiate(enemy, spawnpoint.position, randomRotation);
        DroneEnemy enemyHealth = spawnedEnemy.GetComponent<DroneEnemy>();
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath += OnEnemyDeath;
        }
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
        SetDroneDestinationPoint();
    }


    public void SetDroneDestinationPoint()
    {
        float minX = Mathf.Infinity;
        float maxX = Mathf.NegativeInfinity;
        float minZ = Mathf.Infinity;
        float maxZ = Mathf.NegativeInfinity;

        foreach (var item in walls)
        {
            minX = Mathf.Min(item.position.x, minX) - 0.5f;
            maxX = Mathf.Max(item.position.x, maxX) + 0.5f;

            minZ = Mathf.Min(item.position.z, minZ) - 0.5f;
            maxZ = Mathf.Max(item.position.z, maxZ) + 0.5f;
        }
        for (int i = 0; i < waves[waveIndex].count; i++)
        {
            destinationPoints.Add(new Vector3(Random.Range(minX, maxX), 1.1f, Random.Range(minZ, maxZ)));
            Instantiate(debugPoint, destinationPoints[i], Quaternion.identity, debugParent.transform);
        }
    }

    private void RemoveAllDebugPoints()
    {
        destinationPoints.Clear();
        for (int i = 0; i < debugParent.transform.childCount; i++)
        {
            debugParent.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnEnemyDeath()
    {
        remainingEnemies--;
    }

    void SpawnCrawlerOnWall(GameObject wallcrawler_prefab, Vector3 spawnPosition, Vector3 wallNormal)
    {
        // Instantiate the object at the spawn position
        GameObject crawler = Instantiate(wallcrawler_prefab, spawnPosition, Quaternion.identity);

        // Calculate the rotation to align the object's forward vector with the wall's normal vector
        Quaternion rotation = Quaternion.LookRotation(wallNormal);

        Quaternion headRotation = Quaternion.LookRotation(Vector3.down);

        // Set the object's rotation
        crawler.transform.rotation = rotation * headRotation;  //* Quaternion.Euler(90,0,0);

        WallCrawler crawlerhealth = crawler.GetComponent<WallCrawler>();
        if (crawler != null)
        {
            crawlerhealth.OnDeath += OnEnemyDeath;
        }
    }


    private Vector3 GetWallCrawlerSpawnPoint(int randomNum)
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
        Vector3 newVector = new Vector3(wallbounds.center.x + 0.05f, wallbounds.max.y, wallbounds.center.z + 0.05f);

        return newVector;
    }


}
