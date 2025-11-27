using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class OptionsMenuManager : MonoBehaviour
{
    private bool isOpen = false;
    private Button CloseButton;
    public GameObject UiOptions;
    private SliderInt sliderSound;
    private SliderInt sliderMusic;
    private Button ReturnButton;
    void Start()
    {
        UiOptions.SetActive(false);
    }
    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;
        CloseButton = root.Q<Button>("CloseButton");
        sliderSound = root.Q<SliderInt>("SliderSound");
        sliderMusic = root.Q<SliderInt>("SliderMusic");
        ReturnButton = root.Q<Button>("ReturnMenuButton");
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("No hay AudioManager en la escena.");
            return;
        }
        if (ReturnButton != null) ReturnButton.clicked += OnReturnClicked;
        if (CloseButton != null) CloseButton.clicked += OnCloseClicked;
        
        if (sliderSound != null)
        {
            sliderSound.value = Mathf.RoundToInt(AudioManager.Instance.soundVolume * 100f);
            sliderSound.RegisterValueChangedCallback(OnSliderSoundChanged);
        }

        if (sliderMusic != null)
        {
            sliderMusic.value = Mathf.RoundToInt(AudioManager.Instance.musicVolume * 100f);
            sliderMusic.RegisterValueChangedCallback(OnSliderMusicChanged);
        }
        ReturnButton.RegisterCallback<MouseEnterEvent>(OnHoverEnterReturn);
        ReturnButton.RegisterCallback<MouseLeaveEvent>(OnHoverExitReturn);
        CloseButton.RegisterCallback<MouseEnterEvent>(OnHoverEnterClose);
        CloseButton.RegisterCallback<MouseLeaveEvent>(OnHoverExitClose);

    }
    private void OnReturnClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();
        SceneManager.LoadScene("MainMenu");
    }
    void OnHoverEnterReturn(MouseEnterEvent evt)
    {
        ReturnButton.style.backgroundColor = new StyleColor(Color.grey);
    }
    void OnHoverExitReturn(MouseLeaveEvent evt)
    {
        ReturnButton.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
    }
    void OnHoverEnterClose(MouseEnterEvent evt)
    {
        CloseButton.style.backgroundColor = new StyleColor(Color.grey);
    }
    void OnHoverExitClose(MouseLeaveEvent evt)
    {
        CloseButton.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0));
    }
    private void OnDisable()
    {
        if (sliderSound != null)
            sliderSound.UnregisterValueChangedCallback(OnSliderSoundChanged);

        if (sliderMusic != null)
            sliderMusic.UnregisterValueChangedCallback(OnSliderMusicChanged);
    }

    private void OnSliderSoundChanged(ChangeEvent<int> evt)
    {
        float vol = evt.newValue / 100f;
        AudioManager.Instance.SetSoundVolume(vol);
    }

    private void OnSliderMusicChanged(ChangeEvent<int> evt)
    {
        float vol = evt.newValue / 100f;
        AudioManager.Instance.SetMusicVolume(vol);
    }
    private void OnCloseClicked()
    {
        if (ReturnButton != null)
            TogglePauseMenu();
        
        UiOptions.SetActive(false);

    }
    public void TogglePauseMenu()
    {
        isOpen = !isOpen;

        if (UiOptions != null)
            UiOptions.SetActive(isOpen);

        GameManager.Instance.TurnManager.SetPaused(isOpen);
    }
}
