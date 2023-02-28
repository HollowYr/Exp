#define DEBUG
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;

public class CameraShake : ImprovedMonoBehaviour
{
    [SerializeField]
    internal CinemachineVirtualCamera currentCamera;
    [SerializeField]
    private float shakeDuration = .1f;
    [SerializeField]
    private float amplitude = 1f;
    [SerializeField]
    private float frequency = 1f;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            DoCameraShake();
    }

    public void DoCameraShake()
    {
        DoCameraShake(currentCamera, shakeDuration, amplitude, frequency);
    }

    private void DoCameraShake(CinemachineVirtualCamera camera, float shakeDuration, float amplitude, float frequency)
    {
        Debug.Log("shake");
        CinemachineBasicMultiChannelPerlin noise = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_FrequencyGain = frequency;
        float startFOV = camera.m_Lens.FieldOfView;
        DOTween.To(() => (noise.m_AmplitudeGain = amplitude), x => noise.m_AmplitudeGain = x, 0, shakeDuration);
        // DOTween.To(() => camera.m_Lens.FieldOfView, x => camera.m_Lens.FieldOfView = x, startFOV + FOVAddAmount, shakeDuration / 2)
        //     .SetEase(Ease.OutQuint);
    }

}

