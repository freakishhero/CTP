using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    //The movement speed of the camera when controlled
    float movement_speed = 5.0f;

    //The boundary around the screen that the mouse will move the camera within
    float camera_border_threshold = 15.0f;

    //The speed for mousewheel scrolling
    float mouse_scroll_speed = 200.0f;

    //The x and z (perceived y) limits to clamp camera movement at
    Vector2 camera_limit;

    // Use this for initialization
    void Start()
    {
        camera_limit = new Vector2(10, 15);
    }

    // Update is called once per frame
    void Update()
    {
        //The cameras current position
        Vector3 position = transform.position;

        if (Input.GetKey("w") || Input.GetKey("up")
        || Input.mousePosition.y >= Screen.height - camera_border_threshold)
        {
            position.y += movement_speed * Time.deltaTime;
        }

        if (Input.GetKey("s") || Input.GetKey("down")
        || Input.mousePosition.y <= camera_border_threshold)
        {
            position.y -= movement_speed * Time.deltaTime;
        }

        if (Input.GetKey("a") || Input.GetKey("left")
        || Input.mousePosition.x <= camera_border_threshold)
        {
            position.x -= movement_speed * Time.deltaTime;
        }

        if (Input.GetKey("d") || Input.GetKey("right")
        || Input.mousePosition.x >= Screen.width - camera_border_threshold)
        {
            position.x += movement_speed * Time.deltaTime;
        }

        float mouse_scroll_wheel = Input.GetAxis("Mouse ScrollWheel");
        position.z += mouse_scroll_wheel * mouse_scroll_speed * Time.deltaTime;

        position.x = Mathf.Clamp(position.x, 0, camera_limit.x * 2);
        position.y = Mathf.Clamp(position.y, 0, camera_limit.y * 1.5f);
        position.z = Mathf.Clamp(position.z, -20, -5);

        transform.position = position;
    }
}