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
            if (waterDepth > 0)
            {
                return waterDepth;
            }
            else
            {
                return waterDepth / (isSingleWave ? 10f : 2f);
            }         
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

    void OnEnable()
    {
        if (stickAction != null)
            stickAction.action.Enable();
    }

    void Update()
    {
        if (stickAction == null) return;

        Vector2 stickInput = stickAction.action.ReadValue<Vector2>();
        float rawY = stickInput.y;

        // Convert from [-1, 1] to [0, 1]
        float normalizedY = (rawY + 1f) / 2f;

        Debug.Log($"Stick Y (normalized 0-1): {normalizedY}");
    }


    public void JoystickPressureBegin(float newIntensity)
    {        
        vibrationCoroutine = StartCoroutine(vibrationCoroutineController()); 
        pressStartTime = Time.time;
        isPressing = true;
        StartCoroutine(waitForWaveStop(newIntensity));
    }

    IEnumerator waitForWaveStop(float newIntensity)
    {
        while (isWaveRunning && WaterDepth > 0)
        {
            yield return new WaitForEndOfFrame();
        }
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
        waveCoroutine = StartCoroutine(SingleWave());
    }

    void StartContinuousWave()
    {
        isWaveRunning = true;
        waveCoroutine = StartCoroutine(ContinuousWave());
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

        Debug.Log("After Stop: " + WaterDepth);
    }

    IEnumerator vibrationCoroutineController()
    {
        yield return new WaitForSeconds(maxSingleWaveDuration);
        Debug.Log("Vibration");
        //make the vibration happen
    }

    IEnumerator SingleWave()
    {
        isReturning = true;
        yield return BackToBaseY();
        isReturning = false;
        float t = 0f;
        float duration = Mathf.PI * 2f / Mathf.Lerp(waveSpeedMin, waveSpeedMax, Intensity);
        isSingleWave = true;        

        while (t < duration && !isReturning)
        {
            t += Time.deltaTime;
            var moltiplicatore = Mathf.Lerp(waveSpeedMin, waveSpeedMax, Intensity);   
            float sin = Mathf.Sin(t * moltiplicatore);
            WaterDepth = sin;            
            yield return new WaitForEndOfFrame();
        }

        isWaveRunning = false;
        isSingleWave = false;

        yield return null;
    }
    IEnumerator ContinuousWave()
    {
        float t = 0f;
        isReturning = true;
        yield return BackToBaseY();
        isReturning = false;
        while (!isReturning)
        {
            t += Time.deltaTime;
            float sin = Mathf.Sin(t * Mathf.Lerp(waveSpeedMin, waveSpeedMax, Intensity));
            WaterDepth = sin;           
            yield return new WaitForEndOfFrame();
        }
    }

    //TODO
    IEnumerator BackToBaseY()
    {              
        if(Mathf.Abs(WaterDepth) > 0.02f)
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
    }   
}
