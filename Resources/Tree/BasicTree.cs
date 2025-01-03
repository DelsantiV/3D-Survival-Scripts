using UnityEngine;
using UnityEngine.Events;

namespace GoTF.Content
{
    public class BasicTree : BasicResource
    {
        [SerializeField] protected int treeTier;
        public UnityEvent OnTreeDestroyed;

        public override void Awake()
        {
            base.Awake();
            OnTreeDestroyed = new();
        }

        public override void DestroyResource()
        {
            OnTreeDestroyed.Invoke();
            Destroy(gameObject);
        }

        protected override bool CheckToolTierAndType(ItemProperties.DamageProperties damageProperties)
        {
            if (damageProperties.tier >= treeTier) { return true; }
            return false;
        }
    }
}
