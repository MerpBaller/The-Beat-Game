using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSelfDestruct : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        Invoke("destroy", 0.1f);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log(other.gameObject.name);
        Destroy(gameObject);
    }

    private void destroy()
    {
        if (gameObject.GetComponent<Rigidbody2D>().velocity == Vector2.zero)
        {
            Destroy(gameObject);
        }
    }
}
