using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour {

    [Header("Params")]
	public float amplitude = 1f; // Multiplier of the size of the object    
    public float moveTime = 1f; // Time to travel to the destination
    public float chance = 0.999f;

    private bool isMoving = false;
    private Vector3 initialPos;
    private Vector3 destination;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        initialPos = gameObject.transform.position;
    }

    private void FixedUpdate()
    {
        if (!isMoving)
        {
            if (Random.Range(0f, 1f) > chance)
            {
                // Choose the new destination
                float randX = Random.Range(0f, amplitude * gameObject.transform.localScale.x);
                float randY = Random.Range(0f, amplitude * gameObject.transform.localScale.y);
                float randZ = Random.Range(0f, amplitude * gameObject.transform.localScale.z);

                destination = new Vector3(initialPos.x + randX, initialPos.y + randY, initialPos.z + randZ);
                velocity.x += .1f;

                isMoving = true;
            }            
        } else
        {
            if (velocity.sqrMagnitude < .001f)
            {
                gameObject.transform.position = destination;
                isMoving = false;
            } else
            {
                gameObject.transform.position = Vector3.SmoothDamp(gameObject.transform.position, destination, ref velocity, moveTime);
            }
        }
    }

    public void SetPivotPosition(Vector3 position)
    {
        initialPos = position;
    }

}
