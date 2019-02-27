using UnityEngine;

public class Rotation : MonoBehaviour {

    [Header("Parameters")]
    public float angle = 1f;
    public int axis = 1;

	void Update () {
        transform.Rotate(
            axis == 4 || axis == 5 || axis == 6 || axis == 7 ? angle * Time.deltaTime : 0f,
            axis == 2 || axis == 3 || axis == 6 || axis == 7 ? angle * Time.deltaTime : 0f,
            axis == 1 || axis == 3 || axis == 5 || axis == 7 ? angle * Time.deltaTime : 0f
            );	
	}
}
