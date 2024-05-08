using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool _debugmode = false;
    public bool _startgame = false;
    public static GameManager instance;
    [SerializeField] public UIManager UImanager;

    public XRPlayer XRPlayer;
    [SerializeField] GameObject ammoPrefab;
    [SerializeField] GameObject healthPrefab;
    [SerializeField] AudioClip[] _wallhit;

    private AudioSource _audioSource;
    [SerializeField] private float _ammospawntime = 5f;

    public List<Transform> walls = new List<Transform>();
    [Range(1, 100)] public int destructionlevel;

    [HideInInspector]public bool _gameover;



    public int totoal_voronoi_cells;

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
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_debugmode)
        {
          
            StartGame();
        }
        else
        {
            UImanager.StartDialogue();
        }
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && _gameover)
        {
            Debug.Log("Restarted");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void StartGame()
    {
        StartCoroutine(SpawnCollectibkes());
        _startgame = true;
    }

    private IEnumerator SpawnCollectibkes()
    {
        while (true)
        {
            yield return new WaitForSeconds(_ammospawntime);
            int num = Random.Range(0, 2);
            switch (num)
            {
                case 0:
                    Instantiate(ammoPrefab, GetRandomsAmmoPosition(), Quaternion.identity);
                    break;
                case 1:
                    Instantiate(healthPrefab, GetRandomsAmmoPosition(), Quaternion.identity);
                    break;
            }
        }
    }

    public void SpawnTutorialOrb()
    {
        Instantiate(ammoPrefab, GameManager.instance.XRPlayer.transform.position + new Vector3(0,0.5f,0.1f), Quaternion.identity);
    }

    public void GameOver()
    {
        _gameover = true;
        UImanager._wavetext.text = "Game Over\n Press trigger button on both controllers to restart";
    }

    public void PlayWallHitSound()
    {
        _audioSource.PlayOneShot(_wallhit[Random.Range(0,_wallhit.Length - 1)]);
    }

    private Vector3 GetRandomsAmmoPosition()
    {
        float minX = Mathf.Infinity;
        float maxX = Mathf.NegativeInfinity;
        float minZ = Mathf.Infinity;
        float maxZ = Mathf.NegativeInfinity;

        foreach (var item in walls)
        {
            minX = Mathf.Min(item.position.x, minX)+0.3f;
            maxX = Mathf.Max(item.position.x, maxX)-0.3f;
            minZ = Mathf.Min(item.position.z, minZ)+0.3f;
            maxZ = Mathf.Max(item.position.z, maxZ)-0.3f;
        }
        Vector3 newVector = new Vector3(Random.Range(minX,maxZ),0.75f,Random.Range(minZ,maxZ));
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
                tr.gameObject.layer = LayerMask.NameToLayer("Wall");
            }
            if (tr.name.ToLower().Contains("floor_effectmesh"))
            {
                tr.gameObject.layer = LayerMask.NameToLayer("Ground");
               //tr.gameObject.transform.position = tr.gameObject.transform.position + new Vector3(0, 1f, 0f);
            }
        }
    }
}
