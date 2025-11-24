using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private Button StartButton;
    private Button QuitButton;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("No hay UIDocument en el mismo GameObject que MainMenuManager");
            return;
        }

        var root = uiDocument.rootVisualElement;

        // IMPORTANTE: el name tiene que coincidir EXACTAMENTE con el que pusiste en UI Builder
        StartButton = root.Q<Button>(name: "StartButton");
        QuitButton  = root.Q<Button>(name: "QuitButton");

        if (StartButton == null)
            Debug.LogError("No se ha encontrado el botón 'StartButton' en el UIDocument");
        else
            StartButton.clicked += OnStartClicked;

        if (QuitButton == null)
            Debug.LogError("No se ha encontrado el botón 'QuitButton' en el UIDocument");
        else
            QuitButton.clicked += OnClickQuit;
    }

    private void OnStartClicked()
    {
        Debug.Log("Start pulsado, cargando escena Main...");
        SceneManager.LoadScene("Main"); // Asegúrate que la escena se llama EXACTAMENTE así
    }

    private void OnClickQuit()
    {
        Debug.Log("Quit pulsado, cerrando juego...");
        Application.Quit();
    }
}