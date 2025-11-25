using UnityEngine;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Application = UnityEngine.Application;
public class OptionsMenuManager : MonoBehaviour
{
    private Button CloseButton;
    public GameObject UiOptions;   // <-- UI Options separado

    void Start()
    {
        UiOptions.SetActive(false);
    }
    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        CloseButton = root.Q<Button>("CloseButton");

        if (CloseButton != null) CloseButton.clicked += OnCloseClicked;

    }

    private void OnCloseClicked()
    {
        UiOptions.SetActive(false);

    }
}
