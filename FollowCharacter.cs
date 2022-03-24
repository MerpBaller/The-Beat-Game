using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCharacter : MonoBehaviour
{
    public GameObject objectToFollow;
    private Vector3 position;
    private Vector3 differentiatedPosition;
    private float cameraScrollSpeed = 1.0f;
    private Vector3 object_pos;
    private Vector2 mouse_pos;
    private float xDiff;
    private float yDiff;
    private float xTranslationDiff;
    private float yTranslationDiff;

    // Update is called once per frame
    void Update()
    {
        position = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, -10);

        mouse_pos = Input.mousePosition;
        object_pos = Camera.main.WorldToScreenPoint(this.transform.position);
        xDiff = mouse_pos.x - object_pos.x;
        yDiff = mouse_pos.y - object_pos.y;

        xTranslationDiff = (xDiff / 960)  * (Camera.main.orthographicSize / 2.5f);
        yTranslationDiff = (yDiff /520) * (Camera.main.orthographicSize / 2.5f);
        differentiatedPosition = new Vector3(objectToFollow.transform.position.x + xTranslationDiff, objectToFollow.transform.position.y + yTranslationDiff, -10);
        this.transform.position = Vector3.Lerp(position, differentiatedPosition, 500f);

        if (Input.mouseScrollDelta.y > 0)
        {
            Camera.main.orthographicSize -= cameraScrollSpeed;
            if (Camera.main.orthographicSize <= 2)
            {
                Camera.main.orthographicSize = 2;
            }
        }
        
        if (Input.mouseScrollDelta.y < 0)
        {
            Camera.main.orthographicSize += cameraScrollSpeed;
            if (Camera.main.orthographicSize >= 50)
            {
                Camera.main.orthographicSize = 50;
            }
        }
        

    }
}
