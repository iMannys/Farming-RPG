using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float defaultOffset = 5f;
    public float offsetY = 1f;
    public float smoothing =2f;

    private float currentOffset;

    private Vector3 currentTargetPosition;

    //player transform component
    Transform playerPos;
    // Start is called before the first frame update
    void Start()
    {
        //Find the player gameobject in the scene and get its transform component
        playerPos = FindObjectOfType<PlayerController>().transform;
        currentOffset = defaultOffset;

        UpdateTargetPosition();
        transform.position = currentTargetPosition;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTargetPosition();
        FollowPlayer();
    }

    public void SetZoomOffset(float offset)
    {
        currentOffset = offset;
        UpdateTargetPosition();
    }

    void UpdateTargetPosition()
    {
        //Position the camera should be in
        Vector3 offsetDirection = -transform.forward * currentOffset;
        currentTargetPosition = playerPos.position + offsetDirection;
        currentTargetPosition.y += offsetY;
    }

    //Following the player
    void FollowPlayer()
    {
        //Set the position accordingly
        transform.position = Vector3.Lerp(transform.position, currentTargetPosition, smoothing * Time.deltaTime);
    }
}
