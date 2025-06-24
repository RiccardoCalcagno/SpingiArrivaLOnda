using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GetAxis : MonoBehaviour
{
    [SerializeField] private InputActionReference stickAction;
    [SerializeField] private InputActionReference bButtonAction;

    private bool isPressing = false;
    private float deadzone = 0.15f; // per evitare rumore minimo
    private ControllerWaves controller;
    private BodyScript scriptBody;

    private void Start()
    {
        controller = GameObject.Find("WavesManipulator").GetComponent<ControllerWaves>();
        scriptBody = GameObject.Find("Torso").GetComponent<BodyScript>();
    }

    private void OnEnable()
    {
        if (stickAction?.action != null)
            stickAction.action.Enable();
    }

    private void OnDisable()
    {
        if (stickAction?.action != null)
            stickAction.action.Disable();
    }

    private void Update()
    {
        Vector2 stickPos = stickAction.action.ReadValue<Vector2>();
        

        float normalizedY = (stickPos.y + 1f) / 2f;

        float distanceFromCenter = Mathf.Sqrt((stickPos.y) * (stickPos.y) + (stickPos.x) * (stickPos.x)) ;

        if (!isPressing && distanceFromCenter > deadzone)
        {
            isPressing = true;
            controller.JoystickPressureBegin(normalizedY/2);
        }
        else if (isPressing && distanceFromCenter <= deadzone)
        {
            isPressing = false;
            controller.JoystickPressureEnd();
        }if (bButtonAction != null && bButtonAction.action.WasPressedThisFrame())
         {
             Debug.Log("B button was just pressed!");
             scriptBody.ResetRotation();
         }



    }
}