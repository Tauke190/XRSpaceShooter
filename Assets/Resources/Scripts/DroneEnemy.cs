using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class DroneEnemy : MonoBehaviour
{
    public enum EnemyState
    {
        IDLE,      // Turning Around
        MOVING,    //  Moving
        ATTACKING, // Start Attacking
        LOCKED     //  Locked Attacking
    }

    [SerializeField] private float speed;
    [SerializeField] private float rotationspeed = 0;
    [SerializeField] private float timebetweenattacks = 5f;
    [SerializeField] private float _damageAmount;
    [SerializeField] private float health;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private EnemyState enemyState = EnemyState.IDLE;
    public event Action OnDeath;
    [SerializeField] private GameObject bullet;
    Quaternion targetRotation;
    [SerializeField] private LayerMask _layermask;


    [SerializeField] private AudioClip _droneLaunch;
    [SerializeField] private AudioClip _droneShoot;
    [SerializeField] private AudioClip _droneDamage;
    [SerializeField] private AudioClip _droneDeath;

    private Rigidbody rb;
    private GameObject player;
    private Vector3 destinationpoint;
    private int randomNum;
    private AudioSource _audioSource;

    private Coroutine attackCoroutine;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        EnemyManager.instance.destinationIndex++;
        destinationpoint = EnemyManager.instance.destinationPoints[EnemyManager.instance.destinationIndex-1];
        //EnemyManager.instance.destinationPoints.RemoveAt(randomNum);
        enemyState = EnemyState.MOVING;
        playSFX(_droneLaunch);
    }

    private void Update()
    {
            if (Vector3.Distance(player.transform.position, transform.position) > destinationpoint.y && enemyState == EnemyState.MOVING)
            {
                transform.position = Vector3.MoveTowards(transform.position, destinationpoint, speed * Time.deltaTime);
            }
            if (destinationpoint.y - transform.position.y == 0 && enemyState != EnemyState.LOCKED)
            {
                enemyState = EnemyState.IDLE;
            }
            if (enemyState == EnemyState.IDLE)
            {
                Vector3 directionToPlayer = player.transform.position - transform.position;
                targetRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationspeed * Time.deltaTime);
            }
            if (transform.rotation == targetRotation && enemyState != EnemyState.LOCKED)
            {
                enemyState = EnemyState.ATTACKING;
            }
            if (enemyState == EnemyState.ATTACKING)
            {
                attackCoroutine = StartCoroutine(Attack());
                enemyState = EnemyState.LOCKED;
            }
    }

    public void playSFX(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }


  
    private IEnumerator Attack()
    {
        while (true && !GameManager.instance._gameover)
        {
            yield return new WaitForSeconds(EnemyManager.instance.waves[EnemyManager.instance.waveIndex].timebetweenattacks);
            Vector3 rayDirection = Vector3.forward;
            Vector3 raycastOrigin = transform.position;
            Vector3 raycastDirection = transform.forward;
            float raycastDistance = 100f;

            Ray ray = new Ray(raycastOrigin, raycastDirection);

            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, raycastDistance, _layermask))
            {
                Debug.Log("Hit object :", hitInfo.collider.gameObject);
                if (hitInfo.collider.GetComponent<XRPlayer>())
                {
                    playSFX(_droneShoot);
                    Vector3 dir = (hitInfo.collider.transform.position - this.transform.position).normalized;
                    Vector3 spawnPosition = transform.position + dir * 1f;

                    GameObject temp_bullet = Instantiate(bullet, spawnPosition, transform.rotation);
                    temp_bullet.transform.rotation = Quaternion.LookRotation(dir);
                    Destroy(temp_bullet, 5f);
                    hitInfo.collider.gameObject.GetComponent<XRPlayer>().GetHit(_damageAmount);
                }
            }
        }
    }

    public void GetHit(float floatdamage , Vector3 direction)
    {
        GameManager.instance.XRPlayer._score += 20;
        rb.AddExplosionForce(8, direction, 1f);
        enemyState = EnemyState.MOVING;
        health -= floatdamage;
        playSFX(_droneDamage);

        if (health < 0)
        {
            OnDeath?.Invoke();
            GameObject _explosiontemp = Instantiate(_explosionPrefab, this.transform);
            rb.useGravity = true;
            rb.isKinematic = false;
            this.enabled = false;
            playSFX(_droneDeath);
            Destroy(this.gameObject,4);
        }
    }
}


