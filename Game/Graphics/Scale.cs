using UnityEngine;

public class Scale : MonoBehaviour {

    [Header("Parameters")]
    public float scale = 1f;
    public float speed = 1f;

    private Vector3 localScale;

    public void SetLocalScale(Vector3 lscale)
    {
        localScale = lscale;
    }
    
    void Update () {
        float param = scale * Mathf.Cos(speed * Time.time);
        transform.localScale = localScale + new Vector3(param, param, param);
	}
}
