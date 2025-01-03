using UnityEngine;

namespace GoTF.Content
{
    public class TreeDebris : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField] AudioClip fallingSound;
        [SerializeField] AudioClip breakBranchSound;
        private void OnEnable()
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            rigidbody.AddForce(Vector3.forward, ForceMode.Impulse);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            if (active)
            {
                audioSource = GetComponent<AudioSource>();
                audioSource.PlayOneShot(fallingSound);
            }
        }

        public void BreakBranch()
        {
            audioSource.PlayOneShot(breakBranchSound);
        }
    }
}