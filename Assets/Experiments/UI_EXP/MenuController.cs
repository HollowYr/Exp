#define DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using NaughtyAttributes;
public class MenuController : ImprovedMonoBehaviour
{
    [SerializeField] private VisualTreeAsset _settingsButtonTemplate;

    private UIDocument _doc;
    private VisualElement _buttonHolder;
    private VisualElement _buttonHolderState;
    private VisualElement _settingsButtons;

    private Button _backButton;
    private Button _playButton;
    private Button _settingsButton;
    private Button _exitButton;
    void Start()
    {
        _doc = GetComponent<UIDocument>();
        _buttonHolder = _doc.rootVisualElement.Q<VisualElement>("ButtonsHolder");
        _settingsButtons = _settingsButtonTemplate.CloneTree();
        _buttonHolderState = _buttonHolder;
        _backButton = _settingsButtons.Q<Button>("BackButton");
        _playButton = _doc.rootVisualElement.Q<Button>("PlayButton");
        _settingsButton = _doc.rootVisualElement.Q<Button>("SettingsButton");
        _exitButton = _doc.rootVisualElement.Q<Button>("ExitButton");

        _backButton.clicked += BackButtonOnClicked;
        _playButton.clicked += PlayButtonOnClicked;
        _settingsButton.clicked += SettingsButtonOnClicked;
        _exitButton.clicked += ExitButtonOnClicked;
    }

    private void BackButtonOnClicked()
    {
        _buttonHolder.Clear();
        //_buttonHolder.Add(_buttonHolderState);
        _buttonHolder.Add(_playButton);
        _buttonHolder.Add(_settingsButton);
        _buttonHolder.Add(_exitButton);
#if DEBUG
        _playButton.clicked += PlayButtonOnClicked;
#endif
    }

    private void SettingsButtonOnClicked()
    {
        _buttonHolder.Clear();
        _buttonHolder.Add(_settingsButtons);

#if DEBUG
        _playButton.clicked -= PlayButtonOnClicked;

#endif
    }

    private void ExitButtonOnClicked()
    {
        Debug.Log("Exit game");
    }

    private void PlayButtonOnClicked()
    {
        Debug.Log("Start game");
    }
}

