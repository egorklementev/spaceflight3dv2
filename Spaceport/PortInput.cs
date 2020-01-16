using UnityEngine;

public class PortInput : MonoBehaviour {

    public float scrollBoundXLeft = 7.5f;
    public float scrollBoundXRight = 7.5f;
    public float scrollBoundYTop = 7.5f;
    public float scrollBoundYBot = 7.5f;
    public float scrollSpeed = .5f;
    public float scrollZoomChange = .1f;
    public float zoomSpeed = .1f;
    public float camNearestSize = 8.5f;
    public float camFarthestSize = 25f;

    public FadeManager fm;
    public Camera mainCamera;

    private Vector2 previosPosition = new Vector2(-1f,-1f);
    private float touchDistPrev = 0f;
    private float touchDist = 0f;
    private bool touchMoved = false;

    private void Update()
    {
        if (!PortUI.launchingRocket)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.touchCount == 1)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Moved && previosPosition != new Vector2(-1f, -1f))
                    {
                        Vector3 initialPos = mainCamera.transform.localPosition;

                        Vector3 deltaPos = new Vector3
                            (
                            (touch.position.x - previosPosition.x) * scrollSpeed,
                            (touch.position.y - previosPosition.y) * scrollSpeed,
                            0f
                            );

                        Vector3 newPos = initialPos - deltaPos;

                        bool condLeft = newPos.x >= -scrollBoundXLeft;
                        bool condRight = newPos.x <= scrollBoundXRight;
                        bool condTop = newPos.y <= scrollBoundYTop;
                        bool condBot = newPos.y >= -scrollBoundYBot;

                        // Left bound crossing
                        if (!condLeft && condTop && condBot)
                            newPos = new Vector3(
                                -scrollBoundXLeft,
                                newPos.y,
                                newPos.z
                                );
                        // Right bound crossing
                        if (!condRight && condTop && condBot)
                            newPos = new Vector3(
                                scrollBoundXRight,
                                newPos.y,
                                newPos.z
                                );
                        // Top bound crossing
                        if (condRight && condLeft && !condTop)
                            newPos = new Vector3(
                                newPos.x,
                                scrollBoundYTop,
                                newPos.z
                                );
                        // Bottom bound crossing
                        if (condRight && condLeft && !condBot)
                            newPos = new Vector3(
                                newPos.x,
                                -scrollBoundYBot,
                                newPos.z
                                );

                        // Upper-left corner crossing
                        if (!condLeft && !condTop)
                            newPos = new Vector3(
                                -scrollBoundXLeft,
                                scrollBoundYTop,
                                newPos.z
                                );
                        // Upper-right corner crossing
                        if (!condRight && !condTop)
                            newPos = new Vector3(
                                scrollBoundXRight,
                                scrollBoundYTop,
                                newPos.z
                                );
                        // Bottom-left corner crossing
                        if (!condLeft && !condBot)
                            newPos = new Vector3(
                                -scrollBoundXLeft,
                                -scrollBoundYBot,
                                newPos.z
                                );
                        // Bottom-right corner crossing
                        if (!condRight && !condBot)
                            newPos = new Vector3(
                                scrollBoundXRight,
                                -scrollBoundYBot,
                                newPos.z
                                );

                        mainCamera.transform.localPosition = newPos;

                        previosPosition.x = touch.position.x;
                        previosPosition.y = touch.position.y;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        previosPosition.x = -1f;
                        previosPosition.y = -1f;
                    }
                    else
                    {
                        previosPosition.x = touch.position.x;
                        previosPosition.y = touch.position.y;
                    }

                    if (touch.phase == TouchPhase.Began)
                    {
                        touchMoved = false;
                    }

                    if (touch.phase == TouchPhase.Moved)
                    {
                        touchMoved = true;
                    }

                    // Taps on buildings
                    RaycastHit hitInfo = new RaycastHit();
                    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), out hitInfo);

                    if (hit && touch.phase == TouchPhase.Ended && !touchMoved)
                    {
                        switch (hitInfo.transform.gameObject.name)
                        {
                            case "Hangar":
                                PortUI.camPos = mainCamera.transform.localPosition;
                                fm.SetLevel(5);
                                break;
                            case "Tower":
                                PortUI.camPos = mainCamera.transform.localPosition;
                                fm.SetLevel(6);
                                break;
                            case "SRI":
                                PortUI.camPos = mainCamera.transform.localPosition;
                                fm.SetLevel(8);
                                break;
                        }
                    }
                }

                // Zoom using two fingers
                if (Input.touchCount == 2)
                {
                    Touch t1 = Input.GetTouch(0);
                    Touch t2 = Input.GetTouch(1);

                    touchDist = Mathf.Sqrt((t1.position.x - t2.position.x) * (t1.position.x - t2.position.x) + (t1.position.y - t2.position.y) * (t1.position.y - t2.position.y));

                    // Farthest : left 40, right 10, top 50, bot -15
                    // Nearest  : left 50, right 40, top 75, bot 5
                    if ((t1.phase == TouchPhase.Moved || t2.phase == TouchPhase.Moved) && touchDistPrev != 0f)
                    {
                        if (touchDist - touchDistPrev > 0)
                        {
                            if (mainCamera.orthographicSize - zoomSpeed >= camNearestSize)
                            {
                                mainCamera.orthographicSize -= zoomSpeed;
                                scrollSpeed -= scrollZoomChange;

                                // Change bounds since the size of a camera changes
                                float diff = camFarthestSize - camNearestSize;
                                scrollBoundXLeft += zoomSpeed * 10f / diff;
                                scrollBoundXRight += zoomSpeed * 30f / diff;
                                scrollBoundYTop += zoomSpeed * 25f / diff;
                                scrollBoundYBot += zoomSpeed * 20f / diff;
                            }
                        }
                        if (touchDist - touchDistPrev < 0)
                        {
                            if (mainCamera.orthographicSize + zoomSpeed <= camFarthestSize)
                            {
                                mainCamera.orthographicSize += zoomSpeed;
                                scrollSpeed += scrollZoomChange;

                                // Change bounds since the size of a camera changes
                                float diff = camFarthestSize - camNearestSize;
                                scrollBoundXLeft -= zoomSpeed * 10f / diff;
                                scrollBoundXRight -= zoomSpeed * 30f / diff;
                                scrollBoundYTop -= zoomSpeed * 25f / diff;
                                scrollBoundYBot -= zoomSpeed * 20f / diff;
                            }
                        }
                    }
                    else if (t1.phase == TouchPhase.Ended || t2.phase == TouchPhase.Ended)
                    {
                        touchDist = 0f;
                        touchDistPrev = 0f;
                    }

                    touchDistPrev = touchDist;
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    // Taps on buildings
                    RaycastHit hitInfo = new RaycastHit();
                    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

                    if (hit)
                    {
                        switch (hitInfo.transform.gameObject.name)
                        {
                            case "Hangar":
                                PortUI.camPos = mainCamera.transform.localPosition;
                                fm.SetLevel(5);
                                MusicManager.instance.PlaySound("Click sound");
                                break;
                            case "Tower":
                                PortUI.camPos = mainCamera.transform.localPosition;
                                fm.SetLevel(6);
                                MusicManager.instance.PlaySound("Click sound");
                                break;
                            case "SRI":
                                PortUI.camPos = mainCamera.transform.localPosition;
                                fm.SetLevel(8);
                                MusicManager.instance.PlaySound("Click sound");
                                break;
                        }
                    }

                    previosPosition.x = -1f;
                    previosPosition.y = -1f;
                }

                if (Input.GetMouseButton(0))
                {
                    if (previosPosition != new Vector2(-1f, -1f))
                    {
                        Vector3 initialPos = mainCamera.transform.localPosition;

                        Vector3 deltaPos = new Vector3
                            (
                            (Input.mousePosition.x - previosPosition.x) * scrollSpeed,
                            (Input.mousePosition.y - previosPosition.y) * scrollSpeed,
                            0f
                            );

                        Vector3 newPos = initialPos - deltaPos;

                        bool condLeft = newPos.x >= -scrollBoundXLeft;
                        bool condRight = newPos.x <= scrollBoundXRight;
                        bool condTop = newPos.y <= scrollBoundYTop;
                        bool condBot = newPos.y >= -scrollBoundYBot;
                        
                        // Left bound crossing
                        if (!condLeft && condTop && condBot)
                            newPos = new Vector3(
                                -scrollBoundXLeft,
                                newPos.y,
                                newPos.z
                                );
                        // Right bound crossing
                        if (!condRight && condTop && condBot)
                            newPos = new Vector3(
                                scrollBoundXRight,
                                newPos.y,
                                newPos.z
                                );
                        // Top bound crossing
                        if (condRight && condLeft && !condTop)
                            newPos = new Vector3(
                                newPos.x,
                                scrollBoundYTop,
                                newPos.z
                                );
                        // Bottom bound crossing
                        if (condRight && condLeft && !condBot)
                            newPos = new Vector3(
                                newPos.x,
                                -scrollBoundYBot,
                                newPos.z
                                );

                        // Upper-left corner crossing
                        if (!condLeft && !condTop)
                            newPos = new Vector3(
                                -scrollBoundXLeft,
                                scrollBoundYTop,
                                newPos.z
                                );
                        // Upper-right corner crossing
                        if (!condRight && !condTop)
                            newPos = new Vector3(
                                scrollBoundXRight,
                                scrollBoundYTop,
                                newPos.z
                                );
                        // Bottom-left corner crossing
                        if (!condLeft && !condBot)
                            newPos = new Vector3(
                                -scrollBoundXLeft,
                                -scrollBoundYBot,
                                newPos.z
                                );
                        // Bottom-right corner crossing
                        if (!condRight && !condBot)
                            newPos = new Vector3(
                                scrollBoundXRight,
                                -scrollBoundYBot,
                                newPos.z
                                );

                        mainCamera.transform.localPosition = newPos;
                    }

                    previosPosition.x = Input.mousePosition.x;
                    previosPosition.y = Input.mousePosition.y;
                }

                // Zoom using mouse wheel
                if (Input.mouseScrollDelta.y > 0)
                {
                    if (mainCamera.orthographicSize - zoomSpeed >= camNearestSize)
                    {
                        mainCamera.orthographicSize -= zoomSpeed;
                        scrollSpeed -= scrollZoomChange;

                        // Change bounds since the size of a camera changes
                        float diff = camFarthestSize - camNearestSize;
                        scrollBoundXLeft += zoomSpeed * 10f / diff;
                        scrollBoundXRight += zoomSpeed * 30f / diff;
                        scrollBoundYTop += zoomSpeed * 25f / diff;
                        scrollBoundYBot += zoomSpeed * 20f / diff;
                    }
                }
                if (Input.mouseScrollDelta.y < 0)
                {
                    if (mainCamera.orthographicSize + zoomSpeed <= camFarthestSize)
                    {
                        mainCamera.orthographicSize += zoomSpeed;
                        scrollSpeed += scrollZoomChange;

                        // Change bounds since the size of a camera changes
                        float diff = camFarthestSize - camNearestSize;
                        scrollBoundXLeft -= zoomSpeed * 10f / diff;
                        scrollBoundXRight -= zoomSpeed * 30f / diff;
                        scrollBoundYTop -= zoomSpeed * 25f / diff;
                        scrollBoundYBot -= zoomSpeed * 20f / diff;
                    }
                }
            }
        }
    }
}
