using UnityEngine;
using System.Collections;

public class InputMovement : MonoBehaviour {

    [Tooltip("Camera to follow the drone. This field is mandatory for proper functioning.")]
    public Camera camera;
    [Tooltip("Scale for movement speed.")]
    public float movementScale = 10;
    [Tooltip("Tilt angle for full speed vertical flight.")]
    public float verticalAngle = 40;
    [Tooltip("Tilt angle for full speed horizontal flight.")]
    public float horizontalAngle = 40;

    private float verticalSpeed = 0f;
    [Tooltip("Vertical flight top speed.")]
    public float verticalTopSpeed = 10f;

    // Use this for initialization
    void Start () {
        foreach(var controller in Input.GetJoystickNames())
            Debug.Log(controller);
	}

    // Update is called once per frame
    void Update() {
        Vector3 frontVector = new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z);
        Vector3 upVector = new Vector3(0, -camera.transform.forward.y, 0);
        
        float targetVerticalAngle = 0f;
        float targetHorizontalAngle = 0f;

        if (Input.GetAxis("Vertical (mimas-control)") > 0) {
            targetVerticalAngle = -40;
        } if (Input.GetAxis("Vertical (mimas-control)") < 0) {
            targetVerticalAngle = 40;
        } if (Input.GetAxis("Horizontal (mimas-control)") > 0) {
            targetHorizontalAngle = -40;
        } if (Input.GetAxis("Horizontal (mimas-control)") < 0) {
            targetHorizontalAngle = 40;
        }

        if (Input.GetAxis("VerticalLeft (mimas-control)") == 0) {
            //this.gameObject.transform.rotation = Quaternion.AngleAxis(0, Vector3.right);
        }

        if (Input.GetAxis("VerticalLeft (mimas-control)") > 0.5f) {
            verticalSpeed += (verticalTopSpeed - verticalSpeed) / 10f;
        } else if (Input.GetAxis("VerticalLeft (mimas-control)") < -0.5f) {
            verticalSpeed -= (verticalTopSpeed - Mathf.Abs(verticalSpeed)) / 10f;
        } else
            verticalSpeed += (0 - verticalSpeed) / 10f;

        if (Input.GetAxis("HorizontalLeft (mimas-control)") > 0)
            this.gameObject.transform.Rotate(0, 1, 0);
        if (Input.GetAxis("HorizontalLeft (mimas-control)") < 0)
            this.gameObject.transform.Rotate(0, -1, 0);

        //final adjustments
        //tilt
        if(this.gameObject.transform.localEulerAngles.x < 100)
            this.gameObject.transform.Rotate(new Vector3(((targetVerticalAngle - this.gameObject.transform.localEulerAngles.x)) * Time.deltaTime, 0, 0), Space.Self);
        else
            this.gameObject.transform.Rotate(new Vector3((((targetVerticalAngle + 360) - this.gameObject.transform.localEulerAngles.x)) * Time.deltaTime, 0, 0), Space.Self);

        if (this.gameObject.transform.localEulerAngles.z < 100)
            this.gameObject.transform.Rotate(new Vector3(0, 0, ((targetHorizontalAngle - this.gameObject.transform.localEulerAngles.z)) * Time.deltaTime), Space.Self);
        else
            this.gameObject.transform.Rotate(new Vector3(0, 0, (((targetHorizontalAngle + 360) - this.gameObject.transform.localEulerAngles.z)) * Time.deltaTime), Space.Self);

        //movement
        //xoz movement
        this.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        if (this.gameObject.transform.localEulerAngles.x < 100 && this.gameObject.transform.localEulerAngles.x > 0) {
            //this.gameObject.transform.Translate(frontVector * movementScale * (this.gameObject.transform.localEulerAngles.x / verticalAngle) * Time.deltaTime, Space.World);
            this.gameObject.GetComponent<Rigidbody>().velocity += frontVector * movementScale * (this.gameObject.transform.localEulerAngles.x / verticalAngle);
        } else if (this.gameObject.transform.localEulerAngles.x > 100) {
            //this.gameObject.transform.Translate(-frontVector * movementScale * ((this.gameObject.transform.localEulerAngles.x - 360) / -verticalAngle) * Time.deltaTime, Space.World);
            this.gameObject.GetComponent<Rigidbody>().velocity += -frontVector * movementScale * ((this.gameObject.transform.localEulerAngles.x - 360) / -verticalAngle);
        }

        if (this.gameObject.transform.localEulerAngles.z < 100 && this.gameObject.transform.localEulerAngles.z > 0) {
            //this.gameObject.transform.Translate((Quaternion.Euler(0, 90, 0) * frontVector) * movementScale * (this.gameObject.transform.localEulerAngles.z / -horizontalAngle) * Time.deltaTime, Space.World);
            this.gameObject.GetComponent<Rigidbody>().velocity += (Quaternion.Euler(0, 90, 0) * frontVector) * movementScale * (this.gameObject.transform.localEulerAngles.z / -horizontalAngle);
        } else if (this.gameObject.transform.localEulerAngles.z > 100) {
            //this.gameObject.transform.Translate((Quaternion.Euler(0, 90, 0) * -frontVector) * movementScale * ((this.gameObject.transform.localEulerAngles.z - 360) / horizontalAngle) * Time.deltaTime, Space.World);
            this.gameObject.GetComponent<Rigidbody>().velocity += (Quaternion.Euler(0, 90, 0) * -frontVector) * movementScale * ((this.gameObject.transform.localEulerAngles.z - 360) / horizontalAngle);
        }
        //vertical movement
        //this.gameObject.transform.Translate(new Vector3(0, verticalSpeed * Time.deltaTime, 0));
        this.gameObject.GetComponent<Rigidbody>().velocity += new Vector3(0, verticalSpeed, 0);

        //make camera's tilt independent of drone
        camera.transform.eulerAngles = new Vector3(17, this.gameObject.transform.eulerAngles.y, 0);
        //make camera's position relative to drone
        Vector3 offset = new Vector3(0f, 0.95f, -1.85f);
        Vector3 correctedOffset = Quaternion.Euler(0, this.gameObject.transform.eulerAngles.y, 0) * offset;
        camera.transform.position = this.gameObject.transform.position + correctedOffset;
    }
}
