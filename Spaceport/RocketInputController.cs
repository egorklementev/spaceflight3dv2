using UnityEngine;

public class RocketInputController : MonoBehaviour {

    public float scrollSpeed = 1f;

    private Vector2 previosPosition = new Vector2(-1f, -1f);
    private bool isDragging = false;

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hitInfo);
            if (isDragging || (hit && hitInfo.transform.gameObject.tag == "Rocket"))
            {   
                if (touch.phase == TouchPhase.Moved && previosPosition != new Vector2(-1f, -1f))
                {
                    isDragging = true;

                    Vector3 initialPos = transform.position;

                    float deltaAngle = (touch.position.x - previosPosition.x) * scrollSpeed;
                        
                    transform.Rotate(0f, 0f, -deltaAngle);

                    previosPosition.x = touch.position.x;
                    previosPosition.y = touch.position.y;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    previosPosition.x = -1f;
                    previosPosition.y = -1f;

                    isDragging = false;
                }
                else
                {
                    previosPosition.x = touch.position.x;
                    previosPosition.y = touch.position.y;
                }
            }
        }
    }

}
