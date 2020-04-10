using UnityEngine;

public class BallController : MonoBehaviour {

    [SerializeField]
    private float forceMultiplier = 1.0f;

    private Rigidbody rb;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        Vector3 f = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical")) * forceMultiplier;
        rb.AddForce(f, Input.GetKey(KeyCode.LeftShift) ? ForceMode.Impulse : ForceMode.Force);
    }
}
