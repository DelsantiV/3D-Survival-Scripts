using UnityEngine;

namespace GoTF.Content
{
    public class TreeBranch : MonoBehaviour
    {
        private float maxSpeedMagnitude = 2f;
        private TreeDebris treeDebris;

        private void OnEnable()
        {
            treeDebris = transform.parent.GetComponent<TreeDebris>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.relativeVelocity.magnitude > maxSpeedMagnitude) 
            {
                BreakBranch();
            }
        }

        private void BreakBranch()
        {
            transform.parent = null;
            treeDebris.BreakBranch();
            Debug.Log("Branch broke !");
        }
    }
}