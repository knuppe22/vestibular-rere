using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField]
    private float speed;

    [SerializeField]
    public float VRFeedback = 1;

    private Vector3 rotation = Vector3.zero;

    // Use this for initialization
    void Start () {
        
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.instance.platform == Platform.None)
        {
            if (Input.GetKey(KeyCode.A))
            {
                rotation.y -= speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                rotation.y += speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.W))
            {
                rotation.x -= speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                rotation.x += speed * Time.deltaTime;
            }
            rotation.x = Mathf.Clamp(rotation.x, -80, 80);
            transform.localEulerAngles = rotation;
        }
        else
        {
            float rotationX = transform.localEulerAngles.x;
            float rotationY = transform.localEulerAngles.y;
            float rotationZ = transform.localEulerAngles.z;
            rotationX = (rotationX > 180) ? rotationX - 360 : rotationX;
            rotationY = (rotationY > 180) ? rotationY - 360 : rotationY;
            rotationZ = (rotationZ > 180) ? rotationZ - 360 : rotationZ;
            rotation = new Vector3(rotationX, rotationY, rotationZ);
        }
        transform.parent.localEulerAngles = (VRFeedback - 1) * rotation;
        if (GameManager.instance.platform == Platform.Vive)
        {
            transform.parent.localPosition += (transform.localPosition - transform.position);
        }
    }
}
