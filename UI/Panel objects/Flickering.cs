using UnityEngine;

public class Flickering : MonoBehaviour {

    public float rate = 1f; // How fast to try to flicker
    public float chance = .5f; // With what chance
    public int lampColor = 1;

    private float timer = 0f;

    private void Awake()
    {
        timer = Random.Range(0f, 1f);
        Animator a = GetComponent<Animator>();
        a.SetInteger("LampColor", lampColor);
    }

    private void Update () {

        timer += Time.deltaTime;

        if (timer > rate)
        {
            timer = 0f;

            if (Random.Range(0f, 1f) <= chance)
            {
                Animator a = GetComponent<Animator>();
                a.SetBool("IsOn", !a.GetBool("IsOn"));
            }
        }

	}
}
