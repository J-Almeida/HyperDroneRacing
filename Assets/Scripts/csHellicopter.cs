using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class csHellicopter : MonoBehaviour
{

    public KeyCode UpKey;
    public KeyCode DownKey;
    public KeyCode FrontSpin;
    public KeyCode BackWardSpin;
    public KeyCode LeftTurn;
    public KeyCode RightTurn;
    public KeyCode LeftSpin;
    public KeyCode RightSpin;
    public KeyCode EngineOnOffKey;

    public GameObject[] MainMotor; // Helicoptor Big Propeller. Use Y Rotate Value.
                                   // public GameObject[] SubMotor; //Helicoptor Small Propeller. User X RotateValue.
                                   // public AudioSource _AudioSource; //Helicoptor Engine Sound.

    public float MainMotorRotateVelocity;
    // public float SubMotorRotateVelocity;

    float verticalForceMultiplier = 0.0f;
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

    bool engineIsOn = false;
    float EngineValue = 0.0f;


    float hoverValue;
    float hoverAltitude;
    bool isHovering;

    // debug
    public GameObject debugText1;
    public GameObject debugText2;
    public GameObject debugText3;
    public GameObject debugText4;
    public GameObject debugText5;

    float deadzone = 0.001f;

    public enum ControlState
    {
        Gamepad,
        KeyBoard
    };

    public ControlState ControlType = new ControlState();

    void Awake()
    {
        // Time.timeScale = 0.5f;

        // hoverValue = 10.0f;
        hoverValue = Mathf.Abs(GetComponent<Rigidbody>().mass * Physics.gravity.y); // equivale ao peso do drone, mas não contraria a inércia
        // print("hovering set to " + hoverValue);
    }


    void FixedUpdate()
    {
        MotorVelocityContorl(); // Check Helicoptor UpDown State

        if (engineIsOn)
        {
            /*
            // get vertical speed
            float verticalSpeed = this.GetComponent<Rigidbody>().velocity.y;
            debugText3.GetComponent<Text>().text = "velY = " + verticalSpeed.ToString();
            */


            
            /*
            if (transform.position.y > 100) // altitude limit
                GetComponent<Rigidbody>().AddRelativeForce(-Vector3.up * UpDownValue * UpDownVelocity);
            */

            InvokeRepeating("updateDebugText", 0.0f, 0.5f);

            if (ControlType == ControlState.KeyBoard)
            {
                // UpDown = KeyValue(DownKey, UpKey, UpDown, yUpDown, 1.5f, 0.01f); // original
                UpDown = KeyValue(DownKey, UpKey, UpDown, yUpDown, 1.5f, 0.005f);

                UpDownTurn = KeyValue(BackWardSpin, FrontSpin, UpDownTurn, yUpDownTrun, 1.5f, 0.1f);
                LeftRightTurn = KeyValue(LeftTurn, RightTurn, LeftRightTurn, yLeftRightTurn, 1.5f, 0.1f);
                LeftRightSpin = KeyValue(LeftSpin, RightSpin, LeftRightSpin, yLeftRightSpin, 1, 0.1f);
            }
            else if (ControlType == ControlState.Gamepad)
            { 
                UpDown = Input.GetAxis("LeftVertical (ps3/360)"); // GetAxis devolve entre 0 e 1
                LeftRightTurn = Input.GetAxis("LeftHorizontal (ps3/360)");
                LeftRightSpin = Input.GetAxis("RightHorizontal (ps3/360)");
                UpDownTurn = -Input.GetAxis("RightVertical (ps3/360)");
            }

            Vector3 localAngles = this.transform.localEulerAngles;

            //Pitch Value
            // if (withinBounds(LeftRightSpin, -deadzone, deadzone)) // if input within deadzone
            if (UpDownTurn != 0f) // if input outside deadzone
            {
                Pitch += UpDownTurn * Time.fixedDeltaTime;
                Pitch = Mathf.Clamp(Pitch, -1.2f, 1.2f);
            }
            else // if no input on UpDownTurn
            {
                float torqueScale = 0.5f;
                // debugText4.GetComponent<Text>().text = "dzone, angle: " + localAngles.ToString();

                // estabilizar em relação ao plano yz (roll)
                GetComponent<Rigidbody>().AddTorque(
                    Mathf.DeltaAngle(localAngles.x, 0) * torqueScale,
                    0f,
                    Mathf.DeltaAngle(localAngles.z, 0) * torqueScale);

                // debugText5.GetComponent<Text>().text = "Tx: " + (Mathf.DeltaAngle(localAngles.x, 0) * torqueScale).ToString() + " Ty: " + (Mathf.DeltaAngle(localAngles.y, 0) * torqueScale).ToString();
            }

            //Yaw Value
            Yaw += LeftRightTurn * Time.fixedDeltaTime;

            //Roll Value
            // if (withinBounds(LeftRightSpin, -deadzone, deadzone)) // if input within deadzone
            if (LeftRightSpin != 0f) // if input outside deadzone
            {
                Roll += -LeftRightSpin * Time.fixedDeltaTime;
                Roll = Mathf.Clamp(Roll, -1.2f, 1.2f);
            }
            else // if no input on LeftRightSpin
            {
                float torqueScale = 0.5f;
                // debugText4.GetComponent<Text>().text = "dzone, angle: " + localAngles.ToString();

                // estabilizar em relação ao plano yz (roll)
                GetComponent<Rigidbody>().AddTorque(
                    0f,
                    Mathf.DeltaAngle(localAngles.y, 0) * torqueScale,
                    Mathf.DeltaAngle(localAngles.z, 0) * torqueScale);

                debugText5.GetComponent<Text>().text = "Tx: " + (Mathf.DeltaAngle(localAngles.x, 0) * torqueScale).ToString() + " Ty: " + (Mathf.DeltaAngle(localAngles.y, 0) * torqueScale).ToString();
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.EulerRotation(Pitch, Yaw, Roll), Time.fixedDeltaTime * 1.5f);
            // transform.rotation = Quaternion.Slerp(droneBody.gameObject.transform.rotation, Quaternion.EulerRotation(Pitch, Yaw, Roll), Time.fixedDeltaTime * 1.5f);
        }
    }

    bool withinBounds(float value, double lowerBound, double upperBound)
    {
        if (lowerBound >= upperBound)
            throw new ArgumentException();
        return ((value >= lowerBound) && (value <= upperBound));
    }

    void Update()
    {
        // SoundControl(); // Check Helicoptor Engine Sound
        MotorRotateControl(); // Check Motor Rotate State
        EngineControl(); // Check Engine Turn On/Off
    }

    float KeyValue(KeyCode A, KeyCode B, float Value, float yValue, float _float, float SmoothTime)
    {
        if (Input.GetKey(A))
        { 
            Value -= Time.deltaTime * _float;
        }
        else if (Input.GetKey(B))
        { 
            Value += Time.deltaTime * _float;
        }
        else
        {
            Value = Mathf.SmoothDamp(Value, 0, ref yValue, SmoothTime);
        }

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
        if (engineIsOn)
        {
            if (UpDown != 0.0f) // if vertical input, increase verticalForce multiplier to change altitude
            {
                verticalForceMultiplier += UpDown * 0.1f;
                isHovering = false;

                GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * UpDownValue * verticalForceMultiplier);

                
                debugText2.GetComponent<Text>().text = "(Nhovering) vertForceMult: " + verticalForceMultiplier;
                
            }
            else { // hover if up/down keys are not pressed
                if (!isHovering) hoverAltitude = this.transform.position.y; // set hover altitude if not set before (just started hovering)
                debugText3.GetComponent<Text>().text = "hoverAlt = " + hoverAltitude.ToString();

                isHovering = true;
                
                debugText2.GetComponent<Text>().text = "(hovering) vertForceMult: " + verticalForceMultiplier;

                // verticalForceMultiplier = Mathf.Lerp(verticalForceMultiplier, hoverValue, Time.deltaTime * 2.5f);
                GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * hoverValue); // base hover power to balance weight
                float hoverForce = 1.0f;

                float verticalSpeed = this.GetComponent<Rigidbody>().velocity.y; // código antigo
                // GetComponent<Rigidbody>().AddRelativeForce(-Vector3.up * hoverForce * verticalSpeed); // código antigo ; força pelos 0.1~0.3 num hover suave
                // debugText4.GetComponent<Text>().text = "force = " + (-Vector3.up * hoverForce * verticalSpeed).ToString();
                
                if (this.transform.position.y > hoverAltitude)
                {
                    Vector3 force = -Vector3.up * Mathf.Abs(hoverForce * verticalSpeed * (this.transform.position.y - hoverAltitude) / hoverAltitude);
                    GetComponent<Rigidbody>().AddRelativeForce(force);
                    debugText4.GetComponent<Text>().text = "force = " + force.ToString();
                }
                else
                {
                    Vector3 force = Vector3.up * Mathf.Abs(hoverForce * verticalSpeed * (this.transform.position.y - hoverAltitude) / hoverAltitude);
                    GetComponent<Rigidbody>().AddRelativeForce(force);
                    debugText4.GetComponent<Text>().text = "force = " + force.ToString();
                }

                
            }
        }
        limitVerticalForceMultiplier(); // VER
    }

    void limitVerticalForceMultiplier()
    {
        if (verticalForceMultiplier > 1.0f)
            verticalForceMultiplier = 1.0f;
        else if (verticalForceMultiplier < 0.1f) // nunca fica negativo porque o updown já fica
            verticalForceMultiplier = 0.1f;
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
            if (engineIsOn)
                engineIsOn = false;
            else
                engineIsOn = true;
        }

        if (engineIsOn == true)
        {
            if (EngineValue < 1)
                EngineValue += Time.deltaTime;
            if (EngineValue >= 1)
                EngineValue = 1;
        }

        else if (engineIsOn == false)
        {
            if (EngineValue > 0)
                EngineValue -= Time.deltaTime / 2;
            if (EngineValue <= 0)
                EngineValue = 0;
        }
    }


    void updateDebugText()
    {
        debugText1.GetComponent<Text>().text = "upDown: " + UpDown.ToString();
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