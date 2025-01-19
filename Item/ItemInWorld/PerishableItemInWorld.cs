using UnityEngine;

namespace GoTF.Content {
    public class PerishableItemInWorld : ItemInWorld
    {
        protected float timeToSpoil;
        protected override void Start()
        {
            base.Start();
            timeToSpoil = item.spoilageProperties.timeToSpoil;
        }

        protected override void Update()
        {
            base.Update();
        }

        protected void Spoil() { }
    }
}
