using UnityEngine;
using System.Collections;
using Ximmerse.CrossInput;

public class XimmerseThrowController : MonoBehaviour
{
    public string poseName = "Right_Hand";
    public string triggerName = "Right_Trigger";
    public GameObject throwObjectPrefab;

    public float velocityBoost = 3.0f;

    private VirtualPose virtualPose;
    private GameObject throwObject;
    private float throwObjectInitialScale;

    private const float MAX_TRIGGER_AXIS = 0.5f;
    private bool isMaxTriggerAxisReached = false;

    private Vector3 lastPosition;
    private Vector3 lastLocalPosition;
    private Vector3 lastValidPosition = Vector3.zero;

    private const float MIN_PEAK_MAGNITUDE = 1.0f;
    private const float MAX_PEAK_MAGNITUDE = 50.0f;
    private const float MAX_PEAK_MAGNITUDE_TIME = 0.2f;

    private float peakMagnitude;
    private float peakMagnitudeTime;

    private Vector3 peakVelocity;

    void Start()
    {
        virtualPose = CrossInputManager.VirtualPoseReference(poseName);
    }

    void Update()
    {
        peakMagnitudeTime += Time.deltaTime;
        if (peakMagnitudeTime > MAX_PEAK_MAGNITUDE_TIME)
        {
            ResetPhysicsPeaks();
        }
       
        float triggerAxis = CrossInputManager.GetAxis(triggerName);
        if (triggerAxis > 0)
        {
            if (throwObject == null)
            {
                SpawnThrowObject();
                isMaxTriggerAxisReached = false;
                ResetPhysicsPeaks();
            }

            if (isMaxTriggerAxisReached == false)
            {
                UpdateThrowObjectScale();
            }

            if (triggerAxis >= MAX_TRIGGER_AXIS)
            {
                isMaxTriggerAxisReached = true;
            }

        }
        else if (throwObject != null)
        {
            ReleaseThrowObject();
        }

    }
    
    void FixedUpdate()
    {
        lastPosition = this.transform.position;
        lastLocalPosition = this.transform.localPosition;

        this.transform.position = virtualPose.position;

        UpdatePeakVelocity();

        Vector3 localPosition = this.transform.localPosition;
        if (localPosition.z > 0)
        {
            lastValidPosition = this.transform.position;
        }
        
    }

    private void SpawnThrowObject()
    {
        throwObject = GameObject.Instantiate(throwObjectPrefab);
        throwObjectInitialScale = throwObject.transform.localScale.x;
        throwObject.transform.parent = this.transform;
        throwObject.transform.localPosition = Vector3.zero;

        Rigidbody throwObjectRigidBody = throwObject.gameObject.GetComponent<Rigidbody>();
        throwObjectRigidBody.isKinematic = true;
    }

    private void UpdateThrowObjectScale()
    {
        float triggerAxis = CrossInputManager.GetAxis(triggerName);
        float triggerPercent = Mathf.Clamp01(triggerAxis / MAX_TRIGGER_AXIS);

        float scale = Mathf.SmoothStep(0.0f, 1.0f, triggerPercent) * throwObjectInitialScale;
        SetThrowObjectScale(scale);
    }

    private void UpdatePeakVelocity()
    {
        Vector3 localVelocity = GetLocalVelocity();
        float localVelocityZ = localVelocity.z;
        
        if (localVelocityZ > 0)
        {
            float magnitude = localVelocity.sqrMagnitude;
            bool isValidRange = magnitude >= MIN_PEAK_MAGNITUDE && magnitude <= MAX_PEAK_MAGNITUDE;
            if (isValidRange && magnitude > peakMagnitude)
            {
                peakMagnitude = magnitude;
                peakVelocity = GetVelocity();
            }
            
        }
    }

    private void ResetPhysicsPeaks()
    {
        peakMagnitude = 0.0f;
        peakMagnitudeTime = 0.0f;
        peakVelocity = Vector3.zero;
    }

    private Vector3 GetVelocity()
    {
        Vector3 positionDifference = this.transform.position - lastPosition;
        Vector3 velocity = positionDifference / Time.deltaTime;
        return velocity;
    }

    private Vector3 GetLocalVelocity()
    {
        Vector3 positionDifference = this.transform.localPosition - lastLocalPosition;
        Vector3 velocity = positionDifference / Time.deltaTime;
        return velocity;
    }

    private void ReleaseThrowObject()
    {

        if (peakMagnitude > 0.0f)
        {

            if (this.transform.localPosition.z < 0.0f)
            {
                this.transform.position = lastValidPosition + peakVelocity;
            }

            throwObject.transform.parent = null;

            SetThrowObjectScale(throwObjectInitialScale);
            Rigidbody throwObjectRigidBody = throwObject.gameObject.GetComponent<Rigidbody>();
            throwObjectRigidBody.isKinematic = false;
            
            throwObjectRigidBody.velocity = peakVelocity * velocityBoost;
        }
        else
        {
            Destroy(throwObject);
        }
        

        throwObject = null;
    }

    private void SetThrowObjectScale(float scale)
    {
        if (throwObject != null)
        {
            throwObject.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

}
