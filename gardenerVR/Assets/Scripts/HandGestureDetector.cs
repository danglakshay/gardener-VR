

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

public class HandGestureDetector : MonoBehaviour
{
    [FormerlySerializedAs("_handTrackingEvents")]
    [SerializeField]
    [Tooltip("The hand tracking events component to subscribe to receive updated joint data to be used for gesture detection.")]
    private XRHandTrackingEvents handTrackingEvents;

    [FormerlySerializedAs("_handShapeOrPose")]
    [SerializeField]
    [Tooltip("The hand shape or pose that must be detected for the gesture to be performed.")]
    private ScriptableObject handShapeOrPose;

    [FormerlySerializedAs("_minimumHoldTime")]
    [SerializeField]
    [Tooltip("The minimum amount of time the hand must be held in the required shape and orientation for the gesture to be performed.")]
    private float minimumHoldTime = 0.2f;

    [FormerlySerializedAs("_gestureDetectionInterval")]
    [SerializeField]
    [Tooltip("The interval at which the gesture detection is performed.")]
    private float gestureDetectionInterval = 0.1f;

    [Tooltip("The event fired when the gesture is performed.")]
    public UnityEvent gesturePerformed;

    [Tooltip("The event fired when the gesture is ended.")]
    public UnityEvent gestureEnded;

    private XRHandShape _handShape;
    private XRHandPose _handPose;
    private bool _wasDetected;
    private bool _performedTriggered;
    private float _timeOfLastConditionCheck;
    private float _holdStartTime;

    public Handedness handedness => handTrackingEvents.handedness;

    private void OnEnable()
    {
        handTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);

        _handShape = handShapeOrPose as XRHandShape;
        _handPose = handShapeOrPose as XRHandPose;
    }

    private void OnDisable()
    {
        handTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);
    }

    private void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
    {
        if (!isActiveAndEnabled || Time.timeSinceLevelLoad < _timeOfLastConditionCheck + gestureDetectionInterval)
            return;

        var detected =
            handTrackingEvents.handIsTracked &&
            _handShape != null && _handShape.CheckConditions(eventArgs) ||
            _handPose != null && _handPose.CheckConditions(eventArgs);

        if (!_wasDetected && detected)
        {
            _holdStartTime = Time.timeSinceLevelLoad;
        }
        else if (_wasDetected && !detected)
        {
            _performedTriggered = false;
            gestureEnded?.Invoke();
        }

        _wasDetected = detected;

        if(!_performedTriggered && detected)
        {
            var holdTimer = Time.timeSinceLevelLoad - _holdStartTime;
            if(holdTimer > minimumHoldTime)
            {
                gesturePerformed?.Invoke();
                _performedTriggered = true;
            }
        }

        _timeOfLastConditionCheck = Time.timeSinceLevelLoad;
    }
}