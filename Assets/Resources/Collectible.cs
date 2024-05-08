using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.HandGrab;


public class Collectible : MonoBehaviour
{


    private enum TYPE{
        AMMO,
        HEALTH
    }

    private Animator anim;
    private HandGrabInteractable _interactable;
    [SerializeField] private TYPE collectibletype = TYPE.AMMO;
    private AudioSource audioSource;
    [SerializeField]private AudioClip _ammoClip;
    [SerializeField]private AudioClip _healClip;
    [SerializeField]private int ammo_value = 20;
    [SerializeField] private int health_value = 40;

    private void Start()
    {
        anim = GetComponent<Animator>();
        _interactable = gameObject.GetComponent<HandGrabInteractable>();
        audioSource = GetComponent<AudioSource>();
    }

    
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Controller"))
        {
            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) || OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                anim.SetTrigger("Collapse");
                switch (collectibletype)
                {
                    case TYPE.AMMO:
                        Invoke("GetAmmo", 1f);
                        audioSource.PlayOneShot(_ammoClip);
                        this.gameObject.GetComponent<Collider>().enabled = false;
                        break;
                    case TYPE.HEALTH:
                        Invoke("GetHealth", 1f);
                        audioSource.PlayOneShot(_healClip);
                        this.gameObject.GetComponent<Collider>().enabled = false;
                        break;
                }
            }
        }
        
    }

    private void GetAmmo()
    {
        GameManager.instance.XRPlayer.ammo += ammo_value;
        Destroy(this.gameObject,2f);
    }

    private void GetHealth()
    {
        GameManager.instance.XRPlayer.health += health_value;
        Destroy(this.gameObject,2f);
    }
}
