using UnityEngine;

public class Rotation : MonoBehaviour {

    [Header("Parameters")]
    public float angleX = 1f;
    public float angleY = 1f;
    public float angleZ = 1f;
    public bool timeDependent = false;

    private Vector3 euAngle;

    void Awake()
    {
        euAngle = transform.rotation.eulerAngles;
    }

    void Update () {
        
        if(timeDependent)
        {
            transform.rotation = Quaternion.Euler(
                euAngle.x + Time.time * angleX,
                euAngle.y + Time.time * angleY,
                euAngle.z + Time.time * angleZ
                );
        } else
        {
            transform.Rotate(
            angleX * Time.deltaTime,
            angleY * Time.deltaTime,
            angleZ * Time.deltaTime
            );
        }
	}
}
