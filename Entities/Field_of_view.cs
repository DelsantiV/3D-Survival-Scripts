using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Field_of_view : MonoBehaviour
{
    public float radius;
    public Animator anim;
    [Range(0, 360)]
    public float angle;
    public float speed;
    public float rotationMaxAngle;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool canSeePlayer;
    public Vector3 targetPosition;
    public Vector3 targetRotation;

    private void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(FOVRoutine());
        StartCoroutine(RunRoutine());
        StartCoroutine(TurnRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();


        }
    }

    private IEnumerator RunRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.001f);

        while (true)
        {
            if (canSeePlayer)
            {
                transform.Translate(Vector3.forward * Time.deltaTime * speed);
                anim.speed = speed / 10;

            }
            yield return wait;
        }
    }


    private IEnumerator TurnRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.01f);

        while (true)
        {
            if (canSeePlayer)
            {
                targetRotation = (targetPosition - transform.position).normalized;

                targetRotation.y = 0;

                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetRotation), rotationMaxAngle);

            }
            yield return wait;
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);



                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                    targetPosition = target.position;
                    speed = Mathf.Min(Mathf.Pow(distanceToTarget, 1.8f) / 5, 20);
                }


                else
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
            canSeePlayer = false;
    }
}

