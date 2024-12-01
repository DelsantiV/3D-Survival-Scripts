using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GoTF.Content
{
    public class BunnyBehaviour : MonoBehaviour
    {


        public bool isInSphere;
        float walkingTime;
        float idleTime;
        float runningTime;
        public float wanderingSpeed = 2f;
        public float runningSpeed = 4f;
        private float speed;
        private bool isMoving
        {
            get { return speed != 0; }
        }
        private Vector3 targetPosition;
        private Coroutine coroutineWalking;
        private Coroutine coroutineRunning;
        Animator anim;
        SphereCollider detectionSphere;


        // Start is called before the first frame update
        void Start()
        {

            detectionSphere = GetComponent<SphereCollider>();
            anim = GetComponent<Animator>();



        }

        // Update is called once per frame
        void Update()
        {


            transform.Translate(Vector3.forward * Time.deltaTime * 3 * speed);
            if (isMoving)
            {
                anim.speed = Mathf.Pow(speed, 0.7f);
            }
            else
            {
                anim.speed = 1;
            }
            transform.rotation = Quaternion.LookRotation(targetPosition);

            if (coroutineWalking == null)
            {
                Debug.Log("Start walking");
                coroutineWalking = StartCoroutine(Walking());
                walkingTime = Random.Range(5, 7);
                idleTime = Random.Range(3, 5);
                runningTime = Random.Range(2, 5);
                targetPosition = Random.insideUnitSphere;
                targetPosition.y = 0;
                transform.rotation = Quaternion.LookRotation(targetPosition);



            }



        }



        //Coroutines need improvement : see utilities
        public IEnumerator Walking()
        {
            speed = wanderingSpeed;
            anim.SetBool("isRunning", true);
            yield return new WaitForSeconds(walkingTime);



            speed = 0;
            anim.SetBool("isRunning", false);
            yield return new WaitForSeconds(idleTime);

            coroutineWalking = null;
        }


        public IEnumerator Running()
        {
            anim.SetBool("isRunning", true);
            yield return new WaitForSeconds(runningTime);



            coroutineRunning = null;

        }


        void OnTriggerStay(Collider other)
        {

            if (other.CompareTag("Player"))
            {
                isInSphere = true;
                targetPosition = (transform.position - other.transform.position);

                targetPosition = targetPosition.normalized;
                targetPosition.y = 0;
                speed = runningSpeed;





            }
        }
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isInSphere = false;

                coroutineWalking = StartCoroutine(Running());



            }

        }

    }
}