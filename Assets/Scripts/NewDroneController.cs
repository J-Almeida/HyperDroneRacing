using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NewDroneController : MonoBehaviour
{
    [Space(10)]
    [Tooltip("Current number of laps completed by the drone.")]
    public int currentNumberOfLaps = 0;
    [Space(10)]
    [Tooltip("Camera to follow the drone. This field is mandatory for proper functioning.")]
    public Camera camera;


    public KeyCode EngineOnOffKey;

    public GameObject[] MainMotor; // Helicoptor Big Propeller. Use Y Rotate Value.
                                   // public GameObject[] SubMotor; //Helicoptor Small Propeller. User X RotateValue.
                                   // public AudioSource _AudioSource; //Helicoptor Engine Sound.

    public float MainMotorRotateVelocity;
    // public float SubMotorRotateVelocity;

    float verticalForceScale = 0.0f;
    float VerticalForceMultiplier = 15.0f;
    float MainMotorRotation = 0.0f;
    // float SubMotorRotation = 0.0f;

    public float MaxEnginePowerVertical;
    public float MaxEnginePowerHorizontal;
    public float MaxEnginePowerTotal;
    public float CurrentEnginePowerVertical = 0f;
    public float CurrentEnginePowerHorizontal = 0f;
    public float CurrentEnginePowerTotal = 0f;

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

    public float forceScale = 0.5f;

    // debug
    public GameObject debugText1;
    public GameObject debugText2;
    public GameObject debugText3;
    public GameObject debugText4;

    NewDroneAudio DroneSoundController;

    [SerializeField]
    private Sprite BoostIcon_enabled;
    [SerializeField]
    private Sprite BoostIcon_disabled;
    [SerializeField]
    private Image BoostBar;
    [SerializeField]
    private Image BoostIcon;
    [SerializeField]
    private Image BoostMeter;
    float BoostStamina = 1.0f; // current boost stamina [0-1]
    float BoostValue = 1.5f;
    bool UsingBoost = false;
    bool BoostOnCooldown = false;


    public enum ControlState
    {
        Gamepad,
        KeyBoard
    };

    public ControlState ControlType = new ControlState();

    void Awake()
    {
        DroneSoundController = this.GetComponent<NewDroneAudio>();
        /*
        for (int i = 0; i < MainMotor.Length; i++)
            MainMotor[i].transform.Rotate(90.0f, 0f, 0f);
        */

        // Time.timeScale = 0.5f;

        // hoverValue = 10.0f;
        hoverValue = Mathf.Abs(GetComponent<Rigidbody>().mass * Physics.gravity.y); // equivale ao peso do drone, mas não contraria a inércia
        // print("hovering set to " + hoverValue);
        // print("mass = " + GetComponent<Rigidbody>().mass);

        MaxEnginePowerVertical = hoverValue + VerticalForceMultiplier; // valor base + máximo para verticalForce Scale
        MaxEnginePowerHorizontal = maximumPitchDeg + maximumRollDeg;
        MaxEnginePowerTotal = MaxEnginePowerVertical + MaxEnginePowerHorizontal;
        // print("MaxEnginePowerTotal: " + MaxEnginePowerTotal);

        // maxEnginePower = máximo vertical + máximo horizontal
        // máximo vertical = hoverValue + max VerticalForceScale = hovervalue + max upDown * verticalForceMultiplier = hovervalue + verticalForceMultiplier
        // máximo horizontal = máximo frente + máximo lado = max Pitch + max Roll = maximumPitchDeg + maximumRollDeg
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

            if (Input.GetKeyDown("joystick button 3"))
            { 
                DroneSoundController.PlaySound_fixedLength("horn");
            }

            if (Input.GetKey("joystick button 5"))
            {
                UseBoost();
            }
            else
            {
                UsingBoost = false;
                ChargeBoost();
            }

            if (Input.GetKeyDown("joystick button 5"))
                DroneSoundController.PlaySound("boost");

            if (Input.GetKeyUp("joystick button 5"))
                DroneSoundController.StopSound("boost");

            // Pitch Value
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
        Vector3 frontVector = new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z);
        Vector3 upVector = new Vector3(0, -camera.transform.forward.y, 0);

        // SoundControl(); // Check Helicoptor Engine Sound
        MotorRotateControl(); // Check Motor Rotate State
        EngineControl(); // Check Engine Turn On/Off

        //make camera's tilt independent of drone
        camera.transform.eulerAngles = new Vector3(17, this.gameObject.transform.eulerAngles.y, 0);
        //make camera's position relative to drone
        Vector3 offset = new Vector3(0f, 0.95f, -1.85f);
        Vector3 correctedOffset = Quaternion.Euler(0, this.gameObject.transform.eulerAngles.y, 0) * offset;
        camera.transform.position = this.gameObject.transform.position + correctedOffset;
        // TODO Refactor disto
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
        CurrentEnginePowerVertical = hoverValue; // base value
        CurrentEnginePowerHorizontal = 0f;
        VerticalSpeedControl();
        HorizontalSpeedControl();
        CurrentEnginePowerTotal = CurrentEnginePowerHorizontal + CurrentEnginePowerVertical;

        debugText2.GetComponent<Text>().text = "CEPvert = " + (CurrentEnginePowerVertical / MaxEnginePowerVertical) * 100 + " %";
        debugText3.GetComponent<Text>().text = "CEPhori = " + (CurrentEnginePowerHorizontal / MaxEnginePowerHorizontal) * 100 + " %";
        debugText4.GetComponent<Text>().text = "TEP = " + (CurrentEnginePowerVertical + CurrentEnginePowerHorizontal) / MaxEnginePowerTotal * 100 + " %";

        // simplesmente somar as 2 percentagens divididas?
    }

    void VerticalSpeedControl()
    {
        // get vertical speed
        float verticalSpeed = this.GetComponent<Rigidbody>().velocity.y;

        if (engineIsOn)
        {
            if (UpDown != 0.0f) // not hovering
            {
                isHovering = false;
                verticalForceScale = UpDown * VerticalForceMultiplier;
                GetComponent<Rigidbody>().AddForce(Vector3.up * hoverValue);

                /*
                Vector3 nonHoverForce = Vector3.up * verticalForceMultiplier;
                GetComponent<Rigidbody>().AddRelativeForce(nonHoverForce);
                */ // comentado para evitar modificar velocidade horizontal na função VerticalSpeedControl

                Vector3 nonHoverForce = Vector3.up * verticalForceScale;
                nonHoverForce.x = 0f;
                nonHoverForce.z = 0f;
                if (UsingBoost)
                    GetComponent<Rigidbody>().AddForce(nonHoverForce * forceScale * BoostValue);
                else
                    GetComponent<Rigidbody>().AddForce(nonHoverForce * forceScale);

                CurrentEnginePowerVertical += Mathf.Abs(verticalForceScale);
            }
            else { // hover if up/down keys are not pressed
                isHovering = true;
                verticalForceScale = 0f;
                GetComponent<Rigidbody>().AddForce(Vector3.up * hoverValue);
                float hoverForce = 2.0f;
                GetComponent<Rigidbody>().AddForce(-Vector3.up * hoverForce * verticalSpeed); // aplicar forceScale aqui?

                CurrentEnginePowerVertical += Mathf.Abs(hoverForce * verticalSpeed); // cuidado porque o verticalSpeed não tem limites rígidos

                // verticalForceMultiplier = Mathf.Lerp(verticalForceMultiplier, hoverValue, Time.deltaTime * 2.5f);
            }
        }
        // limitVerticalForceScale();
    }

    void HorizontalSpeedControl()
    // as forças aplicadas nesta função são relativas ao drone
    // logo quando o drone está inclinado para a frente, a força "para a frente" vai ter um componente para baixo
    // isto é eliminado com a eliminação da componente y da força transformada
    {
        if (UpDownTurn != 0) // se está a ser utilizado o analógico correspondente
        {
            Vector3 forwardForce = Vector3.forward * Pitch;
            Vector3 transformedForwardForce = this.transform.TransformVector(forwardForce);
            transformedForwardForce.y = 0f;

            if (UsingBoost)
                GetComponent<Rigidbody>().AddForce(transformedForwardForce * forceScale * BoostValue);
            else
                GetComponent<Rigidbody>().AddForce(transformedForwardForce * forceScale);

            CurrentEnginePowerHorizontal += Mathf.Abs(Pitch);
        }
        else // estabiliza o drone, aplicando uma força no sentido oposto
        {
            float localForwardSpeed = transform.InverseTransformDirection(this.GetComponent<Rigidbody>().velocity).z;

            Vector3 backwardStabilizationForce = Vector3.back * localForwardSpeed * stabilizationScale;
            Vector3 transformedBackwardForce = this.transform.TransformVector(backwardStabilizationForce);
            transformedBackwardForce.y = 0f;
            GetComponent<Rigidbody>().AddForce(transformedBackwardForce); // não leva forceScale

            CurrentEnginePowerHorizontal += Mathf.Abs(localForwardSpeed * stabilizationScale);
        }

        if (LeftRightSpin != 0) // se está a ser utilizado o analógico correspondente
        {
            Vector3 sideForce = Vector3.left * Roll;
            Vector3 transformedSideForce = this.transform.TransformVector(sideForce);
            transformedSideForce.y = 0f;
            if (UsingBoost)
                GetComponent<Rigidbody>().AddForce(transformedSideForce * forceScale * BoostValue);
            else
                GetComponent<Rigidbody>().AddForce(transformedSideForce * forceScale);

            CurrentEnginePowerHorizontal += Mathf.Abs(Roll);
        }
        else // estabiliza o drone, aplicando uma velocidade no sentido oposto
        {
            float localSidewaysSpeed = transform.InverseTransformDirection(this.GetComponent<Rigidbody>().velocity).x;

            Vector3 sideStabilizationForce = Vector3.left * localSidewaysSpeed * stabilizationScale;
            Vector3 transformedSideForce = this.transform.TransformVector(sideStabilizationForce);
            transformedSideForce.y = 0f;
            GetComponent<Rigidbody>().AddForce(transformedSideForce); // não leva forcescale

            CurrentEnginePowerHorizontal += Mathf.Abs(localSidewaysSpeed * stabilizationScale);
        }
    }

    /*
    void limitVerticalForceScale()
    {
        if (verticalForceScale > 1.0f)
            verticalForceScale = 1.0f;
        else if (verticalForceScale < 0.1f) // nunca fica negativo porque o updown já fica
            verticalForceScale = 0.1f;
    }
    */

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
                MainMotor[i].transform.Rotate(0, 0, MainMotorRotation);
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
        if (Input.GetKeyDown(EngineOnOffKey) || Input.GetKeyDown("joystick button 7"))
        {
            if (engineIsOn)
                engineIsOn = false;
            else
            {
                // DroneSoundController.PlaySound_fixedLength("startEngine");
                engineIsOn = true;
            }
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

    void UseBoost()
    {
        if (BoostOnCooldown) return;

        UsingBoost = true;
        BoostStamina -= Time.fixedDeltaTime * 0.5f;
        BoostMeter.fillAmount = BoostStamina;

        // if (currentBoost > 1.0f) currentBoost = 1.0f;
        if (BoostStamina < 0.0f)
        {
            BoostStamina = 0.0f;
            UsingBoost = false;
            BoostOnCooldown = true;
            BoostCooldown();
        }
    }

    void ChargeBoost()
    {
        if (BoostOnCooldown) return;

        BoostStamina += Time.deltaTime * 0.2f;
        BoostMeter.fillAmount = BoostStamina;

        if (BoostStamina > 1.0f)
        {
            DroneSoundController.StopSound("boost");
            BoostStamina = 1.0f;
        }
    }

    void BoostCooldown()
    {
        StartCoroutine("BoostBarBlink");
    }

    IEnumerator BoostBarBlink()
    {
        BoostIcon.sprite = BoostIcon_disabled;

        for (int i = 0; i < 10; i++)
        { 
            if (i % 2 == 0)
                BoostBar.enabled = false;
            else
                BoostBar.enabled = true;
            yield return new WaitForSeconds(0.5f);
        }
        BoostBar.enabled = true;
        BoostOnCooldown = false;
        // BoostStamina = 1.0f;
        BoostMeter.fillAmount = BoostStamina;
        BoostIcon.sprite = BoostIcon_enabled;
    }

    void OnCollisionEnter(Collision collision)
    {
        // print(GetComponent<Rigidbody>().velocity.magnitude);
        if(GetComponent<Rigidbody>().velocity.magnitude > 50.0f)
        {
            DroneSoundController.PlaySound_fixedLength("crashExplosion");
        }
        else
        {
            DroneSoundController.PlaySound_fixedLength("crash");
        }
        
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