#define Debug
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugUI : ImprovedMonoBehaviour
{
    [SerializeField] private Transform player;
    private UIDocument uiDocument;
    private Label speed;
    private Label state;
    private Rigidbody rb;
    private PlayerStateAgent playerStateAgent;
    void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        playerStateAgent = player.GetComponent<PlayerStateAgent>();
        uiDocument = GetComponent<UIDocument>();
        speed = uiDocument.rootVisualElement.Q<Label>("Speed");
        state = uiDocument.rootVisualElement.Q<Label>("State");

    }

    void Update()
    {
        if (rb == null) return;
        speed.text = $"Speed {Math.Round(rb.velocity.magnitude, 2)}";
        state.text = $"State {playerStateAgent.stateMachine.GetCurrentState().ToString()}";
    }
}
