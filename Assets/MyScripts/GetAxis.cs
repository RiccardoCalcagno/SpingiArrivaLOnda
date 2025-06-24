using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GetAxis : MonoBehaviour
{
    [SerializeField] private InputActionReference stickAction;

    private bool isPressing = false;
    private float deadzone = 0.15f; // per evitare rumore minimo
    private ControllerWaves controller;

    private void Start()
    {
        controller = GameObject.Find("WavesManipulator").GetComponent<ControllerWaves>();
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
        }

    }
}