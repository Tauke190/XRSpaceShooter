using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallCrawler : MonoBehaviour
{

    public enum WalkingMode{
        FLAT,
        DOWN
    }
    public float moveSpeed = 1f;
    private float raycastDistance = 0.05f;

    [SerializeField] public Transform wallCheck;
    [SerializeField] public Transform forwardDir;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject _explosion;
    [SerializeField] private float _damageAmount;

    [SerializeField] private AudioClip _spiderwalking;
    [SerializeField] private AudioClip _spiderdamage;
    private Animator _animator;
    private AudioSource _audioSource;

    public WalkingMode walkingMode = WalkingMode.DOWN;


    private Rigidbody rb;
    private Transform player;
    Vector3 offset;
    [SerializeField] private float health;

    public event Action OnDeath;

    private Coroutine attackCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
       _animator = GetComponentInChildren<Animator>();
       player = GameObject.FindAnyObjectByType<XRPlayer>().transform;
       _audioSource = GetComponent<AudioSource>();
       _audioSource.loop = true;
       _audioSource.Play();
       offset = new Vector3(player.transform.position.x, player.transform.position.y - 1.1f, player.transform.position.z);
       walkingMode = WalkingMode.DOWN;
    }

    void Update()
    {
        if(health > 0)
        {
            ShootRayCastInterval();
            FinalMove();
        }  
    }

    private void ShootRayCastInterval()
    {
        //RaycastHit hitGround;
        //if (Physics.Raycast(transform.position, wallCheck.forward, out hitGround, raycastDistance, layerMask))
        //{
        //    float angle = Vector3.Angle(hitGround.normal, Vector3.up);
        //    Debug.Log("Wall hit  :" + angle + " : " + hitGround.collider.name);
        //    if (angle < 5)
        //    {
        //        walkingMode = WalkingMode.FLAT;
        //    }
        //}
    }

    void FinalMove()
    {
        if (walkingMode == WalkingMode.FLAT)
        {
            transform.LookAt(offset);
            transform.position = Vector3.MoveTowards(transform.position, offset, moveSpeed * Time.deltaTime);  
        }
        if (walkingMode == WalkingMode.DOWN)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }

    public void GetHit(float floatdamage, Vector3 direction)
    {
        GameManager.instance.XRPlayer._score += 15;
        //rb.AddExplosionForce(8, direction, 1f);
        //enemyState = EnemyState.MOVING;
        health -= floatdamage;
        _audioSource.PlayOneShot(_spiderdamage);

        if (health < 0)
        {
            OnDeath?.Invoke();
            _animator.SetTrigger("death");
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.AddExplosionForce(10, new Vector3(3, 6, 3), 2);
            Instantiate(_explosion,transform.position,Quaternion.identity);
            Destroy(this.gameObject,1.5f);
            GetComponent<Collider>().enabled = false;
        }
    }

    private void LateGravity()
    {
        rb.useGravity = true;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            attackCoroutine = StartCoroutine(WallCrawlerAttack(collision));
            Debug.Log("Trigger with player");
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Trigger with Ground");
            walkingMode = WalkingMode.FLAT;
        }
        if (collision.gameObject.CompareTag("DeathPlane"))
        {
            OnDeath?.Invoke();
            Destroy(this.gameObject);
        }
    }
    private IEnumerator WallCrawlerAttack(Collider collision)
    {
        while (true && !GameManager.instance._gameover)
        {
            yield return new WaitForSeconds(EnemyManager.instance.waves[EnemyManager.instance.waveIndex].timebetweenattacks);
            collision.gameObject.GetComponent<XRPlayer>().GetHit(_damageAmount);
        }
      
    }

  
}
