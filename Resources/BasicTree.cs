using UnityEngine;

namespace GoTF.Content
{
    public class BasicTree : BasicResource
    {
        protected GameObject stump;
        protected GameObject debris;
        [SerializeField] protected AudioClip fallingSound;
        [SerializeField] protected int treeTier;

        public override void Awake()
        {
            base.Awake();
            stump = transform.Find("Stump").gameObject;
            debris = transform.Find("Debris").gameObject;
        }

        public override void DestroyResource()
        {
            Destroy(gameObject);
            CreateStumpAndDebris();
        }

        private void CreateStumpAndDebris()
        {
            audioSource.PlayOneShot(fallingSound);
            Instantiate(stump);
            Instantiate(debris);
        }

        protected override bool CheckToolTierAndType(ItemProperties.DamageProperties damageProperties)
        {
            if (damageProperties.tier >= treeTier) { return true; }
            return false;
        }
    }
}
