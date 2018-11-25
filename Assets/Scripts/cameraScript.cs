using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour
{

    public float speed;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
        transform.Translate(Vector3.right * Time.deltaTime * speed);
    }
}
