using UnityEngine;

namespace GoTF.Content
{
    public class TreeDebris : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField] AudioClip fallingSound;
        [SerializeField] AudioClip breakBranchSound;
        [SerializeField] AudioClip hitGroundSound;
        private void OnEnable()
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            Vector3 treeOffset = new Vector3(Random.Range(-0.25f, 0.25f), 0, Random.Range(-0.25f, 0.25f));
            transform.position += treeOffset;
            Vector3 treeRotation = transform.rotation.eulerAngles + new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
            transform.rotation = Quaternion.Euler(treeRotation);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            if (active)
            {
                audioSource = GetComponent<AudioSource>();
                if (fallingSound != null) audioSource.PlayOneShot(fallingSound);
            }
        }

        public void BreakBranch()
        {
            if (breakBranchSound != null) audioSource.PlayOneShot(breakBranchSound);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (LayerMask.LayerToName(collision.gameObject.layer) == "Ground")
            {
                audioSource.PlayOneShot(hitGroundSound);
            }
        }
    }
}