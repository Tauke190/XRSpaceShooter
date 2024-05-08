using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRPlayer : MonoBehaviour
{
    [SerializeField] public int _score;
    [SerializeField] public float health = 100f;
    [SerializeField] public int ammo = 40;
    [SerializeField] private AudioClip _gunshoot;
    [SerializeField] private AudioClip _playerdamage;

    private AudioSource _audioSource;


    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void playSFX(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }

    public void GetHit(float damage)
    {
        PlayPlayerDamageSFX();
        health -= damage;
    }

    public void PlayGunSFX()
    {
        playSFX(_gunshoot);
    }

    public void PlayPlayerDamageSFX()
    {
        playSFX(_playerdamage);
    }

    private void Update()
    {
       if(health <= 0)
       {
            GameManager.instance.GameOver();
       }
    }
}
