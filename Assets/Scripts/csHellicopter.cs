using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    float rightAnalogSensitivity = 60.0f;
    float maximumPitchDeg = 35.0f;
    float maximumRollDeg = 35.0f;

    bool engineIsOn = false;
    float EngineValue = 0.0f;

    float hoverValue;
    bool isHovering;
    float stabilizationScale = 2.0f; // higher value stops the drone faster

    // debug
    public GameObject debugText1;
    public GameObject debugText2;
    public GameObject debugText3;

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
        if (engineIsOn)
        {
            /*
            if (transform.position.y > 100) // altitude limit
                GetComponent<Rigidbody>().AddRelativeForce(-Vector3.up * UpDownValue * UpDownVelocity);
            */

            InvokeRepeating("updateDebugText", 0.0f, 0.5f);

            if (ControlType == ControlState.Gamepad)
            {
                UpDown = Input.GetAxis("LeftVertical (ps3/360)"); // GetAxis devolve entre 0 e 1
                LeftRightTurn = Input.GetAxis("LeftHorizontal (ps3/360)");
                LeftRightSpin = Input.GetAxis("RightHorizontal (ps3/360)");
                UpDownTurn = -Input.GetAxis("RightVertical (ps3/360)");
            }

            //Pitch Value
            // Pitch += UpDownTurn * Time.fixedDeltaTime * rightAnalogSensitivity;
            Pitch = UpDownTurn * rightAnalogSensitivity;
            Pitch = Mathf.Clamp(Pitch, -maximumPitchDeg, maximumPitchDeg);

            //Yaw Value
            Yaw += LeftRightTurn * Time.fixedDeltaTime * rightAnalogSensitivity;

            //Roll Value
            // Roll += -LeftRightSpin * Time.fixedDeltaTime * rightAnalogSensitivity; 
            Roll = -LeftRightSpin * rightAnalogSensitivity;
            Roll = Mathf.Clamp(Roll, -maximumRollDeg, maximumRollDeg);

            // transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler( Mathf.Min(Pitch, 45.0f), Yaw, Roll), Time.fixedDeltaTime * 1.5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Pitch, Yaw, Roll), Time.fixedDeltaTime * 1.5f);

            MotorControl();
        }
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

    void MotorControl()
    {
        VerticalSpeedControl();
        HorizontalSpeedControl();
    }

    void VerticalSpeedControl()
    {
        // get vertical speed
        float verticalSpeed = this.GetComponent<Rigidbody>().velocity.y;
        debugText3.GetComponent<Text>().text = "velY = " + verticalSpeed.ToString();

        if (engineIsOn)
        {
            if (UpDown != 0.0f) // not hovering
            {
                isHovering = false;
                verticalForceMultiplier = UpDown * 15.0f;
                // debugText2.GetComponent<Text>().text = "not hovering";

                GetComponent<Rigidbody>().AddForce(Vector3.up * hoverValue);

                /*
                Vector3 nonHoverForce = Vector3.up * verticalForceMultiplier;
                GetComponent<Rigidbody>().AddRelativeForce(nonHoverForce);
                */ // comentado para evitar modificar velocidade horizontal na função VerticalSpeedControl

                Vector3 nonHoverForce = Vector3.up * verticalForceMultiplier;
                nonHoverForce.x = 0f;
                nonHoverForce.z = 0f;
                GetComponent<Rigidbody>().AddForce(nonHoverForce);

                debugText2.GetComponent<Text>().text = "(Nhovering) vertForceMult: " + verticalForceMultiplier;
            }
            else { // hover if up/down keys are not pressed

                isHovering = true;
                verticalForceMultiplier = 0f;

                GetComponent<Rigidbody>().AddForce(Vector3.up * hoverValue);
                // float hoverForce = 1.0f;
                float hoverForce = 2.0f;
                GetComponent<Rigidbody>().AddForce(-Vector3.up * hoverForce * verticalSpeed);

                debugText2.GetComponent<Text>().text = "hovering force: " + hoverForce * verticalSpeed;
                // verticalForceMultiplier = Mathf.Lerp(verticalForceMultiplier, hoverValue, Time.deltaTime * 2.5f);
            }
        }
        limitVerticalForceMultiplier();
    }

    void HorizontalSpeedControl()
    // as forças aplicadas nesta função são relativas ao drone
    // logo quando o drone está inclinado para a frente, a força "para a frente" vai ter um componente para baixo
    {
        if (UpDownTurn != 0) // se está a ser utilizado o analógico correspondente
        {
            Vector3 forwardForce = Vector3.forward * Pitch;
            Vector3 transformedForwardForce = this.transform.TransformVector(forwardForce);
            transformedForwardForce.y = 0f;
            GetComponent<Rigidbody>().AddForce(transformedForwardForce);
        }
        else // estabiliza o drone, aplicando uma velocidade no sentido oposto
        {
            float localForwardSpeed = transform.InverseTransformDirection(this.GetComponent<Rigidbody>().velocity).z;

            Vector3 backwardStabilizationForce = Vector3.back * localForwardSpeed * stabilizationScale;
            Vector3 transformedBackwardForce = this.transform.TransformVector(backwardStabilizationForce);
            transformedBackwardForce.y = 0f;
            GetComponent<Rigidbody>().AddForce(transformedBackwardForce);
        }

        if (LeftRightSpin != 0) // se está a ser utilizado o analógico correspondente
        {
            Vector3 sideForce = Vector3.left * Roll;
            Vector3 transformedSideForce = this.transform.TransformVector(sideForce);
            transformedSideForce.y = 0f;
            GetComponent<Rigidbody>().AddForce(transformedSideForce);
        }
        else // estabiliza o drone, aplicando uma velocidade no sentido oposto
        {
            float localSidewaysSpeed = transform.InverseTransformDirection(this.GetComponent<Rigidbody>().velocity).x;

            Vector3 sideStabilizationForce = Vector3.left * localSidewaysSpeed * stabilizationScale;
            Vector3 transformedSideForce = this.transform.TransformVector(sideStabilizationForce);
            transformedSideForce.y = 0f;
            GetComponent<Rigidbody>().AddForce(transformedSideForce);
        }
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