using UnityEngine;
namespace GoTF.Content
{
    public interface IInteractable
    {
        public string ObjectName {get;}

        public bool ShouldDisplayNameOnMouseOver { get; }

        public void OnCollectInteract(PlayerManager player);
    }
}
