using GoTF.Content;
using UnityEngine;
using UnityEngine.Audio;

namespace GoTF.Content
{
    public class TreeManager : MonoBehaviour
    {
        private GameObject stumpObject;
        private TreeDebris treeDebris;
        private BasicTree tree;

        private void Awake()
        {
            stumpObject = transform.Find("Stump").gameObject;
            tree = transform.Find("Tree").GetComponent<BasicTree>();
            treeDebris = transform.Find("Debris").GetComponent<TreeDebris>();
            tree.OnTreeDestroyed.AddListener(OnTreeDestroyed);
        }

        private void OnTreeDestroyed()
        {
            CreateStumpAndDebris();
        }

        private void CreateStumpAndDebris()
        {
            stumpObject.SetActive(true);
            treeDebris.SetActive(true);
        }
    }
}
