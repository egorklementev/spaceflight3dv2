using UnityEngine;

public class Rotation : MonoBehaviour {

    [Header("Parameters")]
    public float angleX = 1f;
    public float angleY = 1f;
    public float angleZ = 1f;

	void Update () {
        transform.Rotate(
            angleX * Time.deltaTime,
            angleY * Time.deltaTime,
            angleZ * Time.deltaTime
            );	
	}
}
