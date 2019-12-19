﻿using UnityEngine;

public class PortInput : MonoBehaviour {

    public float scrollBound = 7.5f;
    public float scrollSpeed = .5f;

    public FadeManager fm;
    public Camera mainCamera;    

    private Vector2 previosPosition = new Vector2(-1f,-1f);

    private bool touchMoved = false;

    private void Update()
    {
        if (!PortUI.launchingRocket)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Moved && previosPosition != new Vector2(-1f, -1f))
                    {
                        Vector3 initialPos = mainCamera.transform.position;

                        Vector3 deltaPos = new Vector3
                            (
                            ((touch.position.x - previosPosition.x) - (touch.position.y - previosPosition.y)) * scrollSpeed,
                            0f,
                            ((touch.position.x - previosPosition.x) + (touch.position.y - previosPosition.y)) * scrollSpeed
                            );


                        bool cond1 = mainCamera.transform.position.z < -Mathf.Abs(mainCamera.transform.position.x) + scrollBound;
                        bool cond2 = mainCamera.transform.position.z > Mathf.Abs(mainCamera.transform.position.x) - scrollBound;

                        if (cond1 && cond2)
                        {
                            mainCamera.transform.position = Vector3.Lerp(initialPos, initialPos - deltaPos, Time.deltaTime);
                        }

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
                                PortUI.camPos = mainCamera.transform.position;
                                fm.SetLevel(5);
                                break;
                            case "Tower":
                                PortUI.camPos = mainCamera.transform.position;
                                fm.SetLevel(6);
                                break;
                        }
                    }
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
                                PortUI.camPos = mainCamera.transform.position;
                                fm.SetLevel(5);
                                break;
                            case "Tower":
                                PortUI.camPos = mainCamera.transform.position;
                                fm.SetLevel(6);
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
                        Vector3 initialPos = mainCamera.transform.position;

                        Vector3 deltaPos = new Vector3
                            (
                            ((Input.mousePosition.x - previosPosition.x) - (Input.mousePosition.y - previosPosition.y)) * scrollSpeed,
                            0f,
                            ((Input.mousePosition.x - previosPosition.x) + (Input.mousePosition.y - previosPosition.y)) * scrollSpeed
                            );

                        Vector3 newPos = Vector3.Lerp(initialPos, initialPos - deltaPos, Time.deltaTime);

                        bool cond1 = newPos.z < -Mathf.Abs(newPos.x) + scrollBound;
                        bool cond2 = newPos.z > Mathf.Abs(newPos.x) - scrollBound;

                        if (cond1 && cond2)
                        {
                            mainCamera.transform.position = Vector3.Lerp(initialPos, initialPos - deltaPos, Time.deltaTime);
                        }
                        //else 
                        //{
                        //    // Four cases:                          
                        //    if      (newPos.x > 0 && newPos.z > 0) // fx > 0 & fz > 0
                        //    {
                        //        float a = 1f;
                        //        float b = 1f;
                        //        float c = -scrollBound;
                        //        float d = Mathf.Abs(a * newPos.x + b * newPos.z + c) / Mathf.Sqrt(a * a + b * b);
                        //        d /= Mathf.Sqrt(2);
                        //        deltaPos -= new Vector3(d, 0, d);
                        //    }
                        //    else if (newPos.x > 0 && newPos.z < 0) // fx > 0 & fz < 0
                        //    {
                        //        float a = 1f;
                        //        float b = -1f;
                        //        float c = -scrollBound;
                        //        float d = Mathf.Abs(a * newPos.x + b * newPos.z + c) / Mathf.Sqrt(a * a + b * b);
                        //        d /= Mathf.Sqrt(2);
                        //        deltaPos -= new Vector3(d, 0, -d);
                        //    }
                        //    else if (newPos.x < 0 && newPos.z > 0) // fx < 0 & fz > 0
                        //    {
                        //        float a = 1f;
                        //        float b = -1f;
                        //        float c = scrollBound;
                        //        float d = Mathf.Abs(a * newPos.x + b * newPos.z + c) / Mathf.Sqrt(a * a + b * b);
                        //        d /= Mathf.Sqrt(2);
                        //        deltaPos -= new Vector3(-d, 0, d);
                        //    }
                        //    else if (newPos.x < 0 && newPos.z < 0) // fx < 0 & fz < 0
                        //    {
                        //        float a = 1f;
                        //        float b = 1f;
                        //        float c = scrollBound;
                        //        float d = Mathf.Abs(a * newPos.x + b * newPos.z + c) / Mathf.Sqrt(a * a + b * b);
                        //        d /= Mathf.Sqrt(2);
                        //        deltaPos -= new Vector3(-d, 0, -d);
                        //    }

                        //    mainCamera.transform.position = Vector3.Lerp(initialPos, initialPos - deltaPos, Time.deltaTime);
                        //}

                    }

                    previosPosition.x = Input.mousePosition.x;
                    previosPosition.y = Input.mousePosition.y;
                }
            }
        }
    }

}
