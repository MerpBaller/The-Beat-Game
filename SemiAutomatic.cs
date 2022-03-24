using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemiAutomatic : MonoBehaviour
{
    public int magazineSize;
    public int currentRound;

    void Start()
    {
        currentRound = magazineSize;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && currentRound > 0)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(Resources.Load("Prefabs/Weapons/Bullet") as GameObject, gameObject.transform);
        bullet.transform.parent = null;
        currentRound -= 1;
        Vector2 tempShot = transform.up * 5;
        bullet.GetComponent<Rigidbody2D>().AddForce(tempShot, ForceMode2D.Force);
        //Need to add the reload bullshit
    }
}
