using UnityEngine;

public class Rotation : MonoBehaviour {

    [Header("Parameters")]
    public float angle = 1f;

	void Update () {
        transform.Rotate(0f, angle * Time.deltaTime, 0f);	
	}
}
