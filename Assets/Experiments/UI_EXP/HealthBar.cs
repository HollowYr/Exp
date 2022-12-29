#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : ImprovedMonoBehaviour
{
    [SerializeField] private VisualTreeAsset healthCanvas;
    private ProgressBar health;
    private UIDocument uiDoc;
    float value = 0f;
    void Start()
    {
        uiDoc = GetComponent<UIDocument>();
        health = uiDoc.rootVisualElement.Q<ProgressBar>("HealthBar");
    }

    void Update()
    {
        value += Time.deltaTime;
        //value %= 100f;
        health.lowValue = value;
        health.title = $"{Math.Round(value, 2)}/{health.highValue}";
    }
}

