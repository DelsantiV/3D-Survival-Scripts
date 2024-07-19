using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerManager player = PlayerManager.Player;
    public CharacterController controller;
    private GenerateTerrainGrid terrainGrid;
    public GridCell currentGridCell;

    public float baseSpeed = 12f;
    public float speedRunningBonus = 10f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public float speed;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;

    private void Start()
    {
        terrainGrid = Terrain.activeTerrain.GetComponent<GenerateTerrainGrid>();
        currentGridCell = terrainGrid.GetGridCell(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        speed = isGrounded && Input.GetKey(KeyCode.LeftShift) ? speedRunningBonus + baseSpeed : baseSpeed;
        //checking if we hit the ground to reset our falling velocity, otherwise we will fall faster the next time
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float xMove = Input.GetAxis("Horizontal");
        float zMove = Input.GetAxis("Vertical");

        Vector3 move = transform.right * xMove + transform.forward * zMove;

        controller.Move(move * speed * Time.deltaTime);

        //check if the player is on the ground so he can jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            //the equation for jumping
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
        if (xMove !=0 || zMove !=0)
        {
            GetGridCellAfterMovement();
        }
        
        Debug.Log("Current cell : "+currentGridCell.index.ToString()+", at position "+currentGridCell.worldPostion.ToString());

        if (Input.GetKeyDown(KeyCode.C))
        {
            terrainGrid.SetDetailLayerForCell(currentGridCell, 0, 0);
            Debug.Log("Cleaning Grass !");
        }
    }

    private void GetGridCellAfterMovement()
    {
        currentGridCell = terrainGrid.GetGridCell(transform.position);
    }
}
