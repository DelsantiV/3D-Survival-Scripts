using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoTF.Content
{
    public class AIMovement : MonoBehaviour
    {
        private Animator animator;

        public float moveSpeed = 1.0f;

        private Vector3 stopPosition;

        private float walkTime;
        public float walkCounter;
        private float waitTime;
        public float waitCounter;

        private int WalkDirection;

        public bool isWalking;

        void Start()
        {
            animator = GetComponent<Animator>();

            //So that all the prefabs don't move/stop at the same time
            walkTime = Random.Range(7, 10);
            waitTime = Random.Range(3, 6);


            waitCounter = waitTime;
            walkCounter = walkTime;

            ChooseDirection();
        }

        // Update is called once per frame
        void Update()
        {
            if (isWalking)
            {

                animator.SetBool("isRunning", true);

                walkCounter -= Time.deltaTime;

                switch (WalkDirection)
                {
                    case 0:
                        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                        transform.position += transform.forward * moveSpeed * Time.deltaTime;
                        break;
                    case 1:
                        transform.localRotation = Quaternion.Euler(0f, 90, 0f);
                        transform.position += transform.forward * moveSpeed * Time.deltaTime;
                        break;
                    case 2:
                        transform.localRotation = Quaternion.Euler(0f, -90, 0f);
                        transform.position += transform.forward * moveSpeed * Time.deltaTime;
                        break;
                    case 3:
                        transform.localRotation = Quaternion.Euler(0f, 180, 0f);
                        transform.position += transform.forward * moveSpeed * Time.deltaTime;
                        break;
                }

                if (walkCounter <= 0)
                {
                    stopPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                    isWalking = false;
                    //stop movement
                    transform.position = stopPosition;
                    animator.SetBool("isRunning", false);
                    //reset the waitCounter
                    waitCounter = waitTime;
                }


            }
            else
            {

                waitCounter -= Time.deltaTime;

                if (waitCounter <= 0)
                {
                    ChooseDirection();
                }
            }
        }


        public void ChooseDirection()
        {
            WalkDirection = Random.Range(0, 4);

            isWalking = true;
            walkCounter = walkTime;
        }
    }
}
