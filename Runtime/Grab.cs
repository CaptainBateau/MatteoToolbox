﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour
{

    Transform _myHand;
    Rigidbody _myRigidbody;

    public bool _canGrab = true;

    Vector3 _previousPosition;
    Quaternion _previousRotation;

    [System.Serializable]
    public struct GrabbedProp
    {
        public GameObject _gameObject;
        public Transform _previousParent;
        public Rigidbody _rigidbody;
    }
    [Header("Debug Struct, DONT TOUCH")]
    [SerializeField]
    GrabbedProp _grabbed;


    private void Awake()
    {
        _myHand = gameObject.transform;
        _myRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(!_canGrab && grabbing)
        {
            ReleaseGrabItem();
            UnsetGrab();
        }

        if(_canGrab && grabbing)
        {
            GrabItem();
            UnsetGrab();
        }

        _previousPosition = transform.position;
        _previousRotation = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.parent != transform)
        {
            Debug.Log("TriggerChange");
            _grabbed = new GrabbedProp { _gameObject = other.gameObject, _previousParent = other.transform.parent, _rigidbody = other.gameObject.TryGetComponent(out Rigidbody rigidbody) ? rigidbody : other.gameObject.AddComponent<Rigidbody>() };
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.transform.parent != transform)
        {
            _grabbed = new GrabbedProp();
        }
    }

    public void GrabItem()
    {
        if (_grabbed._gameObject != null) 
        { 
            _canGrab = false;
            _grabbed._rigidbody.isKinematic = true;
            _grabbed._gameObject.transform.parent = _myHand;
            if (!_grabbed._gameObject.TryGetComponent(out FollowHand followHand))
            {
                followHand = _grabbed._gameObject.AddComponent<FollowHand>();
            }
            followHand._handTransform = _myHand;
        }
    }

    public void ReleaseGrabItem()
    {
        _grabbed._gameObject.transform.parent = _grabbed._previousParent;
        _grabbed._rigidbody.isKinematic = false;
        Vector3 changePositionFromLastFrame = transform.position - _previousPosition;
        //float angle = Quaternion.Angle(transform.rotation, _previousRotation);
        _grabbed._rigidbody.AddRelativeForce(changePositionFromLastFrame /(float)Time.deltaTime, ForceMode.Impulse);
        _grabbed._rigidbody.AddRelativeTorque(changePositionFromLastFrame / (float)Time.deltaTime, ForceMode.Impulse);

        
        _grabbed._gameObject.GetComponent<FollowHand>().enabled = false;
        _canGrab = true;
    }


    public void SetGrab(bool pressing = true)
    {
        if(pressing)
         grabbing = true;
    }

    public void UnsetGrab()
    {
        grabbing = false;
    }

    bool grabbing = false;

}
