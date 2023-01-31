using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController1 : MonoBehaviour
{

    public GameObject frontLeftMesh;
    public WheelCollider frontLeftCollider;
    [Space(10)]
    public GameObject frontRightMesh;
    public WheelCollider frontRightCollider;
    [Space(10)]
    public GameObject rearLeftMesh;
    public WheelCollider rearLeftCollider;
    [Space(10)]
    public GameObject rearRightMesh;
    public WheelCollider rearRightCollider;
    
    public float maxMotorTorque = 500;
    public float motorTorque = 500;
    public float brakeTorque = 1000;

    Rigidbody carRigidbody;
    public bool isAccelerating = false;
    public bool isBraking = false;
    public bool isTurningLeft = false;
    public bool isTurningRight = false;
    public bool isSlippingForward = false;

    public float currentSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = new Vector3(0, -0.5f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        DetectSlippingForward();
        UpdateWheelColliders();

        currentSpeed = carRigidbody.velocity.magnitude * 3.6f;
    }

    void GetInput(){
        if(Input.GetKey(KeyCode.W)){
            if (currentSpeed < 150){
                isAccelerating = true;
                if(!isSlippingForward && motorTorque < maxMotorTorque){
                motorTorque ++;
                }
                else if(isSlippingForward && motorTorque > 0){
                    motorTorque --;
                }
            }
            else{
                isAccelerating = false;
            }

        }
        else{
            isAccelerating = false;
        }
        if(Input.GetKey(KeyCode.S)){
            isBraking = true;
        }
        else{
            isBraking = false;
        }
        if(Input.GetKey(KeyCode.A)){
            isTurningLeft = true;
        }
        else{
            isTurningLeft = false;
        }
        if(Input.GetKey(KeyCode.D)){
            isTurningRight = true;
        }
        else{
            isTurningRight = false;
        }
    }

    void DetectSlippingForward(){
        WheelHit hit;

        frontLeftCollider.GetGroundHit(out hit);
        if (hit.forwardSlip > 1.8f){   
            isSlippingForward = true;
            return;
        }
        frontRightCollider.GetGroundHit(out hit);
        if (hit.forwardSlip > 1.8f){
            isSlippingForward = true;
            return;
        }
        rearLeftCollider.GetGroundHit(out hit);
        if (hit.forwardSlip > 1.8f){
            isSlippingForward = true;
            return;
        }
        rearRightCollider.GetGroundHit(out hit);
        if (hit.forwardSlip > 1.8f){
            isSlippingForward = true;
            return;
        }
        isSlippingForward = false;
    }

    void UpdateWheelColliders(){
        // TODO Check if it is not slipping and change torque accordingly
        if (isAccelerating){
            frontLeftCollider.motorTorque = motorTorque;
            frontRightCollider.motorTorque = motorTorque;
            rearLeftCollider.motorTorque = motorTorque;
            rearRightCollider.motorTorque = motorTorque;
        }
        else{
            frontLeftCollider.motorTorque = 0;
            frontRightCollider.motorTorque = 0;
            rearLeftCollider.motorTorque = 0;
            rearRightCollider.motorTorque = 0;
        }

        if (isBraking){
            frontLeftCollider.brakeTorque = brakeTorque;
            frontRightCollider.brakeTorque = brakeTorque;
            rearLeftCollider.brakeTorque = brakeTorque;
            rearRightCollider.brakeTorque = brakeTorque;
        }
        else{
            frontLeftCollider.brakeTorque = 0;
            frontRightCollider.brakeTorque = 0;
            rearLeftCollider.brakeTorque = 0;
            rearRightCollider.brakeTorque = 0;
        }

        // TODO move wheels slowly to the desired angle
        if (isTurningLeft){
            frontLeftCollider.steerAngle = -30;
            frontRightCollider.steerAngle = -30;
        }
        else if (isTurningRight){
            frontLeftCollider.steerAngle = 30;
            frontRightCollider.steerAngle = 30;
        }
        else{
            frontLeftCollider.steerAngle = 0;
            frontRightCollider.steerAngle = 0;
        }
    }

    void FixedUpdate(){
      UpdateMeshes();
    }

    void UpdateMeshes(){
      UpdateMesh(frontLeftMesh, frontLeftCollider);
      UpdateMesh(frontRightMesh, frontRightCollider);
      UpdateMesh(rearLeftMesh, rearLeftCollider);
      UpdateMesh(rearRightMesh, rearRightCollider);
    }

    void UpdateMesh(GameObject mesh, WheelCollider collider){
      Vector3 pos = mesh.transform.position;
      Quaternion rot = mesh.transform.rotation;
      collider.GetWorldPose(out pos, out rot);
      mesh.transform.position = pos;
      mesh.transform.rotation = rot;
    }
}
