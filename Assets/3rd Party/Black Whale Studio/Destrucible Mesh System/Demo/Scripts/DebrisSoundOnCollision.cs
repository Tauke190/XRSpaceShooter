using UnityEngine;

namespace BlackWhale.DestructibleMeshSystem.Demo
{
    [RequireComponent(typeof(AudioSource))]
    public class DebrisSoundOnCollision : MonoBehaviour
    {
        [SerializeField] private AudioClip collisionSound;
        [SerializeField] private float minimumVelocity = 2f;
        [SerializeField] private float volumeScale = 1f;

        private AudioSource audioSource;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource component missing! Adding one.", this);
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.relativeVelocity.magnitude > minimumVelocity)
            {
                float volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 10f) * volumeScale;
                audioSource.PlayOneShot(collisionSound, volume);
            }
        }
    }
}