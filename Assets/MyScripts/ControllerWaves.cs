using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerWaves : MonoBehaviour
{
    public float maxSingleWaveDuration;
    public float waveSpeedMin;
    public float waveSpeedMax;
    public float returningSpeed;


    public InputActionReference stickAction;

    private float waterDepth;
    public float WaterDepth
    {
        get
        {            
            return waterDepth;
        }
        private set
        {
            waterDepth = value;
        }
    }

    public float Intensity { get; private set; }


    private float pressStartTime;
    private bool isPressing = false;
    private bool isWaveRunning = false;
    private bool isReturning = false;
    private bool isSingleWave = false;

    private Coroutine waveCoroutine;
    private Coroutine vibrationCoroutine;
    private GameObject sea;
    private void Start()
    {
        sea = GameObject.FindFirstObjectByType<RoomLimits>().gameObject;
    }

    //void OnEnable()
    //{
    //    if (stickAction != null)
    //        stickAction.action.Enable();
    //}

    //void Update()
    //{
    //    if (stickAction == null) return;

    //    Vector2 stickInput = stickAction.action.ReadValue<Vector2>();
    //    float rawY = stickInput.y;

    //    // Convert from [-1, 1] to [0, 1]
    //    float normalizedY = (rawY + 1f) / 2f;

    //    Debug.Log($"Stick Y (normalized 0-1): {normalizedY}");
    //}


    public void JoystickPressureBegin(float newIntensity)
    {
        vibrationCoroutine = StartCoroutine(vibrationCoroutineController());
        pressStartTime = Time.time;
        isPressing = true;
        Intensity = newIntensity;
    }

    public void JoystickPressureEnd()
    {
        StopCoroutine(vibrationCoroutine);
        isPressing = false;

        float heldTime = Time.time - pressStartTime;

        StopWave();

        if (heldTime < maxSingleWaveDuration)
        {
            PlaySingleWave();
        }
        else
        {
            StartContinuousWave();
        }
    }

    void PlaySingleWave()
    {
        isWaveRunning = true;
        waveCoroutine = StartCoroutine(SingleWave(Intensity));
    }

    void StartContinuousWave()
    {
        isWaveRunning = true;
        waveCoroutine = StartCoroutine(ContinuousWave(Intensity));
    }

    void StopWave()
    {
        isWaveRunning = false;
        isSingleWave = false;
        isReturning = false;
        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
        }
    }

    IEnumerator vibrationCoroutineController()
    {
        yield return new WaitForSeconds(maxSingleWaveDuration);
        Debug.Log("Vibration");
        //make the vibration happen
    }

    IEnumerator SingleWave(float intensity)
    {
        yield return BackToBaseY();

        float t = 0f;
        var moltiplicatore = Mathf.Lerp(waveSpeedMin, waveSpeedMax, intensity);

        isSingleWave = true;

        bool crossLine = false;

        while ((crossLine == false || t < 0.3333f * Mathf.PI / moltiplicatore) && !isReturning && isWaveRunning)
        {
            t += Time.deltaTime;

            if (t * moltiplicatore > Mathf.PI && crossLine == false)
            {
                t = 0;
                crossLine = true;
            }

            float sin;

            if (crossLine)
            {
                sin = -Mathf.Sin(t * moltiplicatore * 3f) / 7f;
            }
            else
            {
                sin = Mathf.Sin(t * moltiplicatore);
            }

            WaterDepth = sin;
            yield return new WaitForEndOfFrame();
        }

        isWaveRunning = false;
        isSingleWave = false;

        yield return null;
    }
    IEnumerator ContinuousWave(float intensity)
    {
        float t = 0f;

        yield return BackToBaseY();

        while (!isReturning && isWaveRunning)
        {
            t += Time.deltaTime;
            float sin = Mathf.Sin(t * Mathf.Lerp(waveSpeedMin, waveSpeedMax, intensity));

            if (sin > 0)
            {
                WaterDepth = sin;
            }
            else
            {
                WaterDepth = sin / 2;
            }


            yield return new WaitForEndOfFrame();
        }
    }

    //TODO
    IEnumerator BackToBaseY()
    {
        isReturning = true;
        if (Mathf.Abs(WaterDepth) > 0.02f)
        {
            var startTime = Time.time;
            var startPosition = WaterDepth;
            var durationReturning = Mathf.Abs(WaterDepth) / returningSpeed;

            while (Time.time < startTime + durationReturning)
            {
                var relativeProgress = (Time.time - startTime) / durationReturning;
                WaterDepth = Mathf.Lerp(startPosition, 0, relativeProgress);

                yield return new WaitForEndOfFrame();
            }
        }
        isReturning = false;
        yield return null;
    }

}
