using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour
{
    private Vector2 mouse_pos;
    private Vector3 object_pos;
    private Quaternion quaternion;
    private float angle;
    private float xDiff;
    private float yDiff;
    public float movementSpeed = 15f;
    public float maxSpeed = 15f;
    private float maxTurningSpeed = 0.1f;
    private float sidewaysVelocityWhileMovingForward = 0.4f;
    private Quaternion currentRotation;
    private Rigidbody2D rb;
    private State currentState;
    public GameObject weaponHeld;
    public GameObject InventoryManager;
    private bool itemCurrentlyEquipped;
    private GameObject itemEquipped;
    public GameObject EquippedItemUI;

    enum State
    {
        NoItemEquipped,
        WeaponEquipped,
        ToolEquipped
    }

    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        currentState = State.NoItemEquipped;
        itemCurrentlyEquipped = false;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.NoItemEquipped:
                gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/PoliceOfficer");
                weaponHeld.SetActive(false);
                break;
            case State.WeaponEquipped:
                gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/PoliceOfficerHoldingWeapon");
                weaponHeld.SetActive(true);
                break; 
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && itemCurrentlyEquipped == false)
        {
            itemEquipped = Instantiate(InventoryManager.GetComponent<CharacterInventory>().EquipItem(1), weaponHeld.transform);
            if (itemEquipped.tag == "Weapon")
            {
                currentState = State.WeaponEquipped;
                weaponHeld.SetActive(true);
                itemEquipped.SetActive(true);
                itemCurrentlyEquipped = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && itemCurrentlyEquipped == true)
        {
            currentState = State.NoItemEquipped;
            weaponHeld.SetActive(false);
            Destroy(itemEquipped);
            itemCurrentlyEquipped = false;
            EquippedItemUI.GetComponent<Image>().sprite = null;
            Color tempColor = EquippedItemUI.GetComponent<Image>().color;
            tempColor.a = 0f;
            EquippedItemUI.GetComponent<Image>().color = tempColor;
        }
    }

    void FixedUpdate()
    {
        CharaterDirection();
        CharaterMove();
    }

    private void CharaterMove()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.drag = 0;
            sidewaysVelocityWhileMovingForward = 0.4f;

            if (rb.velocity.x > maxSpeed) { rb.velocity = new Vector2(maxSpeed, rb.velocity.y); }
            else if (rb.velocity.x < -maxSpeed) { rb.velocity = new Vector2(-maxSpeed, rb.velocity.y); }

            if (rb.velocity.y > maxSpeed) { rb.velocity = new Vector2(rb.velocity.x, maxSpeed); }
            else if (rb.velocity.y < -maxSpeed) { rb.velocity = new Vector2(rb.velocity.x, -maxSpeed); }

            if (Input.GetKey(KeyCode.A))
            {
                Vector2 moveVector = (transform.up + -transform.right) * movementSpeed;
                rb.AddForce(moveVector, ForceMode2D.Force);
                sidewaysVelocityWhileMovingForward = 0.99f;
                KillSidewaysVelocity();
            }
            else if (Input.GetKey(KeyCode.D))
            {
                Vector2 moveVector = (transform.up + transform.right) * movementSpeed;
                rb.AddForce(moveVector, ForceMode2D.Force);
                sidewaysVelocityWhileMovingForward = 0.99f;
                KillSidewaysVelocity();
            }
            else if (Input.GetKey(KeyCode.S))
            {
                rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(0, 0), Time.fixedDeltaTime);
                KillSidewaysVelocity();
            }
            else
            {
                Vector2 moveVector = transform.up * movementSpeed;
                rb.AddForce(moveVector, ForceMode2D.Force);
                KillSidewaysVelocity();
            }
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rb.drag = 0;
            sidewaysVelocityWhileMovingForward = 0.8f;

            if (rb.velocity.x > maxSpeed / 2) { rb.velocity = new Vector2(maxSpeed / 2, rb.velocity.y); }
            else if (rb.velocity.x < -maxSpeed / 2) { rb.velocity = new Vector2(-maxSpeed / 2, rb.velocity.y); }

            if (rb.velocity.y > maxSpeed / 2) { rb.velocity = new Vector2(rb.velocity.x, maxSpeed / 2); }
            else if (rb.velocity.y < -maxSpeed / 2) { rb.velocity = new Vector2(rb.velocity.x, -maxSpeed / 2); }

            if (Input.GetKey(KeyCode.D))
            {
                rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(0, 0), Time.fixedDeltaTime);
                KillForwardVelocity();
            }
            else if (Input.GetKey(KeyCode.S))
            {
                Vector2 moveVector = (-transform.up + -transform.right) * movementSpeed / 2;
                rb.AddForce(moveVector, ForceMode2D.Force);
                sidewaysVelocityWhileMovingForward = 0.99f;
                KillForwardVelocity();
            }
            else
            {
                Vector2 moveVector = -transform.right * movementSpeed / 2;
                rb.AddForce(moveVector, ForceMode2D.Force);
                KillForwardVelocity();
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.drag = 0;
            sidewaysVelocityWhileMovingForward = 0.8f;

            if (rb.velocity.x > maxSpeed / 2) { rb.velocity = new Vector2(maxSpeed / 2, rb.velocity.y); }
            else if (rb.velocity.x < -maxSpeed / 2) { rb.velocity = new Vector2(-maxSpeed / 2, rb.velocity.y); }

            if (rb.velocity.y > maxSpeed / 2) { rb.velocity = new Vector2(rb.velocity.x, maxSpeed / 2); }
            else if (rb.velocity.y < -maxSpeed / 2) { rb.velocity = new Vector2(rb.velocity.x, -maxSpeed / 2); }

            if (Input.GetKey(KeyCode.S))
            {
                Vector2 moveVector = (-transform.up + transform.right) * movementSpeed / 2;
                rb.AddForce(moveVector, ForceMode2D.Force);
                sidewaysVelocityWhileMovingForward = 0.99f;
                KillForwardVelocity();
            }
            else
            {
                Vector2 moveVector = transform.right * movementSpeed / 2;
                rb.AddForce(moveVector, ForceMode2D.Force);
                KillForwardVelocity();
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rb.drag = 0;
            sidewaysVelocityWhileMovingForward = 0.4f;

            if (rb.velocity.x > maxSpeed / 2) { rb.velocity = new Vector2(maxSpeed / 2, rb.velocity.y); }
            else if (rb.velocity.x < -maxSpeed / 2) { rb.velocity = new Vector2(-maxSpeed / 2, rb.velocity.y); }

            if (rb.velocity.y > maxSpeed / 2) { rb.velocity = new Vector2(rb.velocity.x, maxSpeed / 2); }
            else if (rb.velocity.y < -maxSpeed / 2) { rb.velocity = new Vector2(rb.velocity.x, -maxSpeed / 2); }

            Vector2 moveVector = -transform.up * movementSpeed;
            rb.AddForce(moveVector, ForceMode2D.Force);
            KillSidewaysVelocity();
        }
        else 
        {
            rb.velocity = Vector2.Lerp(rb.velocity, new Vector2(0, 0), Time.fixedDeltaTime);
            rb.drag = 10;
        }
    }

    private void KillSidewaysVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.velocity, transform.right);

        rb.velocity = forwardVelocity + rightVelocity * sidewaysVelocityWhileMovingForward;
    }

    private void KillForwardVelocity()
    {
        Vector2 forwardVelocity = transform.up * Vector2.Dot(rb.velocity, transform.up);
        Vector2 rightVelocity = transform.right * Vector2.Dot(rb.velocity, transform.right);

        rb.velocity = rightVelocity + forwardVelocity * sidewaysVelocityWhileMovingForward;
    }

    private void CharaterDirection()
    {
        currentRotation = this.transform.rotation;
        mouse_pos = Input.mousePosition;
        object_pos = Camera.main.WorldToScreenPoint(this.transform.position);
        xDiff = mouse_pos.x - object_pos.x;
        yDiff = mouse_pos.y - object_pos.y;
        angle = Mathf.Atan2(yDiff, xDiff) * Mathf.Rad2Deg;
        angle -= 90;
        quaternion = Quaternion.Euler(0, 0, angle);
        rb.transform.rotation = Quaternion.Lerp(currentRotation, quaternion, maxTurningSpeed);
        if (rb.angularVelocity > 0 || rb.angularVelocity < 0)
        {
            rb.angularVelocity = 0;
        }
    }
}