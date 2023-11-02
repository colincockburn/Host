using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_controller : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;

    private void Update()
    {
        target = GameManager.instance.controlledObject.transform;
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
