using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicles : MonoBehaviour
{
    public BoxCollider2D DoorEntrance;
    public BoxCollider2D CarCollider;
    public GameObject person;
    private BoxCollider2D PersonCollider;
    public GameObject entranceMarker;
    public GameObject InteractionMenu;
    private GameObject driverSideMarker;
    private bool flag;
    private bool inVehicle = false;
    public float accelerationFactor = 60.0f;
    public float turnFactor = 0.125f;
    private float accelerationInput = 0f;
    private float steeringInput = 0f;
    private float currentRotation = 0f;
    private float driftFactor = 0.85f;
    private GameObject tempMenu;
    private int numOptions = 1;
    // Start is called before the first frame update
    void Start()
    {
        PersonCollider = person.GetComponent<BoxCollider2D>();
        flag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inVehicle == false)
        {
            InteractWithVehicle();
        }
        else if (inVehicle == true)
        {
            steeringInput = Input.GetAxis("Horizontal");
            accelerationInput = Input.GetAxis("Vertical");

            if (Input.GetKeyDown(KeyCode.E))
            {
                ExitVehicle();
            }
        }
    }

    private void ExitVehicle()
    {
        driverSideMarker = Instantiate(entranceMarker, gameObject.transform);
        driverSideMarker.name = "Driver Side Marker";
        driverSideMarker.transform.localPosition = new Vector3(0, DoorEntrance.offset.y, 0);
        person.transform.position = driverSideMarker.transform.position;
        Destroy(driverSideMarker);
        person.SetActive(true);
        Camera.main.GetComponent<FollowCharacter>().objectToFollow = person;
        inVehicle = false;
    }

    private void InteractWithVehicle()
    {
        if (DoorEntrance.IsTouching(PersonCollider) == true && flag == false)
        {
            flag = true;
            driverSideMarker = Instantiate(entranceMarker, gameObject.transform);
            driverSideMarker.name = "Driver Side Marker";
            driverSideMarker.SetActive(true);
            driverSideMarker.transform.localPosition = new Vector3(0, DoorEntrance.offset.y, 0);

            tempMenu = Instantiate(InteractionMenu);
            tempMenu.GetComponent<InteractionMenuCreator>().numOptions = numOptions;
            tempMenu.GetComponent<InteractionMenuCreator>().buttonTexts.Add("Enter Vehicle (E)");

            tempMenu.SetActive(true);
        }
        else if (flag == true && DoorEntrance.IsTouching(PersonCollider) == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                EnterVehicle();
                inVehicle = true;

                flag = false;
                Destroy(driverSideMarker);
                Destroy(tempMenu);
            }
        }
        else if (flag == true && DoorEntrance.IsTouching(PersonCollider) == false)
        {
            flag = false;
            Destroy(driverSideMarker);
            Destroy(tempMenu);
        }
    }

    void FixedUpdate()
    {
        if (inVehicle == true)
        {
            EngineForce();
            SteeringForce();
            KillSidewaysVelocity();
        }
        else if (gameObject.GetComponent<BoxCollider2D>().IsTouching(PersonCollider) != true)
        {
            //Need to make sure if something else were to run into it it would move
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    
    }

    private void KillSidewaysVelocity()
    {
        Vector2 forwardVelocity = -transform.right * Vector2.Dot(gameObject.GetComponent<Rigidbody2D>().velocity, -transform.right);
        Vector2 rightVelocity = -transform.up * Vector2.Dot(gameObject.GetComponent<Rigidbody2D>().velocity, -transform.up);

        if (Input.GetKey(KeyCode.Space))
        {
            driftFactor = 0.95f;
            gameObject.GetComponent<Rigidbody2D>().drag = 1f;
            gameObject.GetComponent<Rigidbody2D>().angularDrag = 0.25f;
        }
        else
        {
            driftFactor = 0.85f;
        }

        gameObject.GetComponent<Rigidbody2D>().velocity = forwardVelocity + rightVelocity * driftFactor;
    }

    private void SteeringForce()
    {
        float minSpeedBeforeAllowTurningFactor = gameObject.GetComponent<Rigidbody2D>().velocity.magnitude / 25;
        minSpeedBeforeAllowTurningFactor = Mathf.Clamp01(minSpeedBeforeAllowTurningFactor);

        currentRotation -= steeringInput * turnFactor * minSpeedBeforeAllowTurningFactor;

        gameObject.GetComponent<Rigidbody2D>().MoveRotation(currentRotation);
    }

    private void EngineForce()
    {
        /*
        if (accelerationInput == 0)
        {
            gameObject.GetComponent<Rigidbody2D>().drag = Mathf.Lerp(gameObject.GetComponent<Rigidbody2D>().drag, 0.5f, Time.fixedDeltaTime);
        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().drag = 0;
        }
        */

        gameObject.GetComponent<Rigidbody2D>().drag = 0.5f;
        gameObject.GetComponent<Rigidbody2D>().angularDrag = 0.5f;

        currentRotation = gameObject.GetComponent<Rigidbody2D>().rotation;

        Vector2 engineForceVector = -transform.right * accelerationInput * accelerationFactor;

        gameObject.GetComponent<Rigidbody2D>().AddForce(engineForceVector, ForceMode2D.Force);
    }

    private void EnterVehicle()
    {
        person.SetActive(false);

        Camera.main.GetComponent<FollowCharacter>().objectToFollow = gameObject;
    }
}
