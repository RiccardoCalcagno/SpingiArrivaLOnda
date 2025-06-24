using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerWaves : MonoBehaviour
{
    public float maxSingleWaveDuration;
    public float waveSpeedMin;
    public float waveSpeedMax;
    public float returningSpeed;
    public OVRInput.Controller controllerToVibrate = OVRInput.Controller.RTouch; //scegli il controller
    
    //impostazioni della vibrazione
    [Range(0, 1)] public float frequency = 0.5f;
    [Range(0, 1)] public float amplitude = 0.5f;
    public float duration = 0.2f;

    

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
    private AudioSource audio;

    private Coroutine waveCoroutine;
    private Coroutine vibrationCoroutine;
    
    private void Start()
    {
        audio = GameObject.Find("WaveSound").GetComponent<AudioSource>();
        audio.Play();
        audio.volume = 0; 
    }

    private void Update()
    {
        audio.volume = Mathf.Abs(waterDepth) + 0.5f;
        if (waterDepth > -0.025f && waterDepth < 0.025f && !isWaveRunning)
        {
            audio.volume = 0; 
        }
    }

    public void JoystickPressureBegin(float intensity)
    {
        vibrationCoroutine = StartCoroutine(vibrationCoroutineController());
        pressStartTime = Time.time;
        isPressing = true;
        Intensity = intensity;
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
        //make the vibration happen
        OVRInput.SetControllerVibration(frequency, amplitude, controllerToVibrate);
        yield return new WaitForSeconds(duration);
        OVRInput.SetControllerVibration(0, 0, controllerToVibrate);
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
