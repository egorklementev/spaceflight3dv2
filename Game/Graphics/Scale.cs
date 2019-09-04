using UnityEngine;

/// <summary>
/// Performs scaling of the object from its initial scale (localScale) 
/// to the specified scale (multiplier * localScale) with the given speed
/// </summary>
public class Scale : MonoBehaviour {

    [Header("Parameters")]
    public float scale = 1f;
    public float speed = 1f;

    private Vector3 localScale;

    /// <summary>
    /// Sets local scale of the object to be scaled
    /// </summary>
    /// <param name="lscale"></param>
    public void SetLocalScale(Vector3 lscale)
    {
        localScale = lscale;
    }
    
    void Update () {
        float param = scale * Mathf.Cos(speed * Time.time);
        transform.localScale = localScale + new Vector3(param, param, param);
	}
}
