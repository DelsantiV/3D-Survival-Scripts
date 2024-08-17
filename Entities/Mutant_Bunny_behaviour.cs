using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class MutantBunnyBehaviour : MonoBehaviour
{


    public bool isInSphere;
    float walkingTime;
    float idleTime;
    float runningTime;
    public float wanderingSpeed = 2f;
    public float runningSpeed = 4f;
    private float speed;
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
        anim.speed = Mathf.Pow(speed, 0.7f);

        transform.rotation = Quaternion.LookRotation(targetPosition);
        
        if (coroutineWalking == null)
        {

            coroutineWalking = StartCoroutine(Walking());
            walkingTime = Random.Range(3, 7);
            idleTime = Random.Range(1, 3);
            runningTime = Random.Range(2, 5);
            targetPosition = Random.insideUnitSphere;
            targetPosition.y = 0;
            transform.rotation = Quaternion.LookRotation(targetPosition);



        }


       

    }



    //Coroutines need improvement : see utilities
    public IEnumerator Walking()
    {


        anim.SetBool("isRunning", true);
        yield return new WaitForSeconds(walkingTime);



        speed = 0;
        anim.speed = 1;
        anim.SetBool("isRunning", false);
        yield return new WaitForSeconds(idleTime);

        speed = wanderingSpeed;

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
            targetPosition = (other.transform.position - transform.position);

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
