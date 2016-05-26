using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class csHellicopter : MonoBehaviour {

    public KeyCode UpKey;
    public KeyCode DownKey;
    public KeyCode FrontSpin;
    public KeyCode BackWardSpin;
    public KeyCode LeftTurn;
    public KeyCode RightTurn;
    public KeyCode LeftSpin;
    public KeyCode RightSpin;
    public KeyCode EngineOnOffKey;

    public GameObject[] MainMotor; //Helicoptor Big Propeller. Use Y Rotate Value.
    // public GameObject[] SubMotor; //Helicoptor Small Propeller. User X RotateValue.
    // public AudioSource _AudioSource; //Helicoptor Engine Sound.

    public float MainMotorRotateVelocity;
    // public float SubMotorRotateVelocity;

    float UpDownVelocity = 0.0f; 
    float MainMotorRotation = 0.0f;
    // float SubMotorRotation = 0.0f;
    
    public float UpDownValue;
    float UpDown;
    float yUpDown;

    //Rotate Value
    float Pitch;
    float UpDownTurn;
    float yUpDownTrun;

    float Yaw;
    float LeftRightTurn;
    float yLeftRightTurn;

    float Roll;
    float LeftRightSpin;
    float yLeftRightSpin;

    bool EngineTurnOnOff = false;
    float EngineValue = 0.0f;



    // debug
    public GameObject debugText;

    public enum ControlState
    {
        Mouse,
        KeyBoard
    };

    public ControlState ControlType = new ControlState();

    void FixedUpdate()
    {
        if (EngineTurnOnOff)
        {
            GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * UpDownValue * UpDownVelocity);

            if (transform.position.y > 100)
                GetComponent<Rigidbody>().AddRelativeForce(-Vector3.up * UpDownValue * UpDownVelocity);

            // UpDown = KeyValue(DownKey, UpKey, UpDown, yUpDown, 1.5f, 0.01f);
            UpDown = KeyValue(DownKey, UpKey, UpDown, yUpDown, 1.5f, 0.015f);

    

        InvokeRepeating("updateDebugText", 0.0f, 0.5f);

            /*
            // restringir o movimento vertical quando não se está a carregar nas teclas (não diretamente mas através do UpDown)
            if (UpDown == 0) {
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
                UpDown = 0.0f;
            }
            else
            {
                this.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
            */


            if (ControlType == ControlState.KeyBoard)
            {
                UpDownTurn = KeyValue(BackWardSpin, FrontSpin, UpDownTurn, yUpDownTrun, 1.5f, 0.1f);
                LeftRightTurn = KeyValue(LeftTurn, RightTurn, LeftRightTurn, yLeftRightTurn, 1.5f, 0.1f);
                LeftRightSpin = KeyValue(LeftSpin, RightSpin, LeftRightSpin, yLeftRightSpin, 1, 0.1f);
            }

            else if (ControlType == ControlState.Mouse)
            {
                //Pitch Value
                UpDownTurn = Input.GetAxis("Mouse Y");
                Pitch -= UpDownTurn * Time.fixedDeltaTime *0.01f;
                Pitch = Mathf.Clamp(Pitch, -1.2f, 1.2f);

                //Yaw Value
                LeftRightTurn = Input.GetAxis("Mouse X");
                Yaw += LeftRightTurn * Time.fixedDeltaTime *0.01f;

                LeftRightSpin = KeyValue(LeftSpin, RightSpin, LeftRightSpin, yLeftRightSpin, 1, 0.1f);
            }

            //Pitch Value
            Pitch += UpDownTurn * Time.fixedDeltaTime;
            Pitch = Mathf.Clamp(Pitch, -1.2f, 1.2f);

            //Yaw Value
            Yaw += LeftRightTurn * Time.fixedDeltaTime;

            //Roll Value
            Roll += -LeftRightSpin * Time.fixedDeltaTime;
            Roll = Mathf.Clamp(Roll, -1.2f, 1.2f);

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.EulerRotation(Pitch, Yaw, Roll), Time.fixedDeltaTime * 1.5f);
        }
    }
	
	void Update ()
    {
        // SoundControl(); // Check Helicoptor Engine Sound
        MotorRotateControl(); // Check Motor Rotate State
        MotorVelocityContorl(); // Check Helicoptor UpDown State
        EngineControl(); // Check Engine Turn On/Off
	}

    float KeyValue(KeyCode A,KeyCode B, float Value , float yValue , float _float , float SmoothTime)
    {
        if(Input.GetKey(A))
            Value -= Time.deltaTime * _float;
        else if (Input.GetKey(B))
            Value += Time.deltaTime * _float;
        else
            Value = Mathf.SmoothDamp(Value, 0, ref yValue, SmoothTime);

        Value = Mathf.Clamp(Value, -1, 1);
        return Value;
    }
    /*
    float StabilizeUp(float Value, float yValue, float _float, float SmoothTime)
    {
        if (Input.GetKey(A))
            Value -= Time.deltaTime * _float;
        else if (Input.GetKey(B))
            Value += Time.deltaTime * _float;
        else
            Value = Mathf.SmoothDamp(Value, 0, ref yValue, SmoothTime);

        Value = Mathf.Clamp(Value, -1, 1);
        return Value;
    }
    */

    void MotorVelocityContorl()
    {
        if (EngineTurnOnOff)
        {
            float Hovering = (Mathf.Abs(GetComponent<Rigidbody>().mass * Physics.gravity.y) / UpDownValue); //for maintain altitude.

            if (UpDown != 0.0f)
                UpDownVelocity += UpDown * 0.1f; //if Input Up/Down Axes, Increace UpDownVelocity for Increace altitude.
            else
                UpDownVelocity = Mathf.Lerp(UpDownVelocity, Hovering, Time.deltaTime); //if not input Up/Down Axes, Hovering.
        }
       CheckUpDownVelocity();
    }

    void CheckUpDownVelocity()
    {
        if (UpDownVelocity > 1.0f)
            UpDownVelocity = 1.0f;
        else if (UpDownVelocity < 0.1f)
            UpDownVelocity = 0.1f;
    }

    void MotorRotateControl()
    {
        if (MainMotor.Length > 0)
        {
            /*
            print("wing 0 position: ");
            print(MainMotor[0].transform.position.ToString());
            print("wing 0 local position: ");
            print(MainMotor[0].transform.localPosition.ToString());
            print("wing 0 box collider center position: ");
            print(MainMotor[0].GetComponent<BoxCollider>().center.ToString());
            */


            for (int i = 0; i < MainMotor.Length; i++)
                MainMotor[i].transform.Rotate(0, MainMotorRotation, 0);

        }

        /*
        if (SubMotor.Length > 0)
        {
            for (int i = 0; i < SubMotor.Length; i++)
                SubMotor[i].transform.Rotate(0, 0, SubMotorRotation);
        }
        */

        MainMotorRotation = MainMotorRotateVelocity * EngineValue * Time.deltaTime;
        // SubMotorRotation = SubMotorRotateVelocity * EngineValue * Time.deltaTime;
    }

    void EngineControl()
    {
        if (Input.GetKeyDown(EngineOnOffKey))
        {
            if (EngineTurnOnOff)
                EngineTurnOnOff = false;
            else
                EngineTurnOnOff = true;
        }

        if (EngineTurnOnOff == true)
        {
            if (EngineValue < 1)
                EngineValue += Time.deltaTime;
            if (EngineValue >= 1)
                EngineValue = 1;
        }

        else if (EngineTurnOnOff == false)
        {
            if (EngineValue > 0)
                EngineValue -= Time.deltaTime / 2;
            if (EngineValue <= 0)
                EngineValue = 0;
        }
    }

    
    void updateDebugText()
    {
        debugText.GetComponent<Text>().text = UpDown.ToString();
    }
    

    /*
    void SoundControl()
    {
        if (EngineValue > 0)
        {
            _AudioSource.mute = false;
            _AudioSource.pitch = EngineValue;
        }
        else
            _AudioSource.mute = true;
    }
    */
}