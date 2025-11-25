using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Application = UnityEngine.Application;

public class MainMenuManager : MonoBehaviour
{
    private Button StartButton;
    private Button QuitButton;
    private Button OptionsButton;

    public GameObject UiOptions;   // <-- UI Options separado

    void Start()
    {
        UiOptions.SetActive(false);
    }
    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        StartButton = root.Q<Button>("StartButton");
        QuitButton = root.Q<Button>("QuitButton");
        OptionsButton = root.Q<Button>("SettingsButton");

        if (StartButton != null) StartButton.clicked += OnStartClicked;
        if (QuitButton != null) QuitButton.clicked += OnQuitClicked;
        if (OptionsButton != null) OptionsButton.clicked += OnOptionsClicked;
    }

    private void OnStartClicked()
    {
        SceneManager.LoadScene("Main");
    }

    private void OnQuitClicked()
    {
        Application.Quit();
    }

    private void OnOptionsClicked()
    {
        // Activa el UI Options
        UiOptions.SetActive(true);

    }
}
