using UnityEngine;
using System.Collections.Generic;

namespace BlackWhale.DestructibleMeshSystem.Demo
{
    public enum Hand
    {
        Left,
        Right
    }
    public class XRGun : MonoBehaviour
    {
        [SerializeField] private LayerMask destroyableLayerMask;
        [SerializeField] private List<GameObject> debrisPrefabs;
        [SerializeField] private float destroyRadius = 0.05f;
        [SerializeField] public Hand hand = Hand.Right;

        private XRPlayer _xrplayer;


        [SerializeField] private Transform forward;

        private Vector3 lastRayOrigin;
        private Vector3 lastRayDirection;
        [SerializeField] private GameObject bullet;



        private void Start()
        {
            _xrplayer = GetComponentInParent<XRPlayer>();
        }


        void Update()
        {
            switch (hand)
            {
                case Hand.Left:
                    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
                    {
                        TryDestroyObjectInPath(hand);
                        
                    }
                    break;
                case Hand.Right:
                    if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
                    {
                        TryDestroyObjectInPath(hand);
                      
                    }
                    break;
            }
        }

        private void TryDestroyObjectInPath(Hand hand)
        {
         

        
           
            Vector3 controllerPosition;
            Quaternion controllerRotation;

            if (hand == Hand.Right)
            {
                //controllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
                //controllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
                controllerPosition = forward.position;
                controllerRotation = forward.rotation;
            }
            else
            {
                //controllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
                //controllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
                controllerPosition = forward.position;
                controllerRotation = forward.rotation;
            }
          
            Vector3 rayDirection = controllerRotation * Vector3.forward;

            lastRayOrigin = controllerPosition;
            lastRayDirection = rayDirection;

            if(GameManager.instance.XRPlayer.ammo > 0)
            {
                GameObject temp_bullet = Instantiate(bullet, forward.position, forward.rotation);
                Destroy(temp_bullet, 5f);
                _xrplayer.PlayGunSFX();
                GameManager.instance.XRPlayer.ammo--;
                if (Physics.Raycast(controllerPosition, rayDirection, out RaycastHit hit, Mathf.Infinity, destroyableLayerMask))
                {
                    Collider[] hitColliders = Physics.OverlapSphere(hit.point, destroyRadius, destroyableLayerMask);
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Drone"))
                    {
                        hit.collider.GetComponent<DroneEnemy>().GetHit(40f, rayDirection);
                        //Debug.Log("HitEnemy");
                    }
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Crawler"))
                    {
                        hit.collider.GetComponent<WallCrawler>().GetHit(40f, rayDirection);
                        //Debug.Log("CrawlerHit");
                    }
                    else
                    {
                        foreach (Collider hitCollider in hitColliders)
                        {
                            if (hitCollider.gameObject.layer == LayerMask.NameToLayer("Destroy"))
                            {
                                GameObject debrisPrefab = debrisPrefabs[Random.Range(0, debrisPrefabs.Count)];
                                GameObject debris = Instantiate(debrisPrefab, hit.point, Quaternion.identity);
                                Destroy(hitCollider.gameObject);
                                Destroy(debris, 3f);
                                GameManager.instance.PlayWallHitSound();
                            }
                        }
                    }
                }
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (lastRayOrigin != null && lastRayDirection != null)
            {
                Gizmos.DrawRay(lastRayOrigin, lastRayDirection * 100);
            }
        }
    }
}