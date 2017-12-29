using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour {

    public float speed = 1.0F;
    Vector3 center = new Vector3(3, 3, 3);
    Transform trans;

	// Use this for initialization
	void Start () {
        trans = new GameObject().transform;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            Vector3 newAng = trans.eulerAngles + new Vector3(0, Input.GetAxis("Mouse X") * speed, 0);
            float newX = newAng.x + -Input.GetAxis("Mouse Y") * speed;
            if (newX < 89.9 || newX > 270.1)
                newAng.x = newX;
            trans.eulerAngles = newAng;
            transform.position = trans.TransformPoint(new Vector3(0, 0, -23)) + center;
            transform.LookAt(center);
        }
    }
}
