using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private float dist = 300.0f;

    [SerializeField]
    private float yaw = 0.0f;

    [SerializeField]
    private float pitch = 45.0f;

    private float pivotX;
    private float pivotZ;
    private float pivotY;

    private GameObject camPivot;

    public void Awake() {
        camPivot = new GameObject("Camera Pivot");
        camPivot.transform.parent = transform;
        Camera.main.transform.parent = camPivot.transform;
    }

    public void Start() {
        pivotX = transform.position.x;
        pivotZ = transform.position.z;
        pivotY = transform.position.y;
    }

    public void Update() {

        if (Input.GetMouseButton(0)) {
            float dy = Input.GetAxis("Mouse X") * 3.0f;
            if (Mathf.Abs(dy) > 0.001f) {
                yaw += dy;
            }
        }

        if (Input.GetMouseButton(0)) {
            float dp = Input.GetAxis("Mouse Y") * -3.0f;
            if (Mathf.Abs(dp) > 0.001f) {
                pitch += dp;
            }
        }

        float dd = Input.mouseScrollDelta.y * dist * 0.025f / (0.025f * (dist + 3.0f)) * 500;
        if (Input.GetKey(KeyCode.LeftShift)) {
            dd *= 3;
        }
        if (Mathf.Abs(dd) > 0.001f) {
            dist -= dd;
        }
    }

    public void LateUpdate() {
        UpdateCameraTransform();
    }

    protected void UpdateCameraTransform() {

        Camera.main.transform.localPosition = new Vector3(0, 0, -dist);
        Camera.main.transform.localRotation = Quaternion.identity;

        camPivot.transform.localPosition = new Vector3(pivotX, pivotY, pivotZ);
        camPivot.transform.localEulerAngles = new Vector3(pitch, yaw, 0.0f);
    }
}
