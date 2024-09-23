using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject victoryUI;
    public GameObject defeatUI;
    public CanvasGroup fadeCanvasGroup;
    private float fadeDuration = 0.5f;

    public PlayerLook playerlook;
    public FrenzyManager frenzyManager;
    public GunSystem gun;
    public PauseMenu pauseMenu;

    public enum GameState { Victory, Defeat, OnMatch }
    private GameState currentState;

    [Header("Habitaciones")]
    public List<GameObject> generatedRooms = new List<GameObject>(); 
    public List<GameObject> generatedCorridors = new List<GameObject>(); 
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddRoom(GameObject room)
    {
        
        if (generatedRooms.Count > 0)
        {
            GameObject previousRoom = generatedRooms[generatedRooms.Count - 1];
            previousRoom.SetActive(false); 
            Debug.Log("Habitación anterior desactivada: " + previousRoom.name);
        }

        generatedRooms.Add(room);
        Debug.Log("Nueva habitación añadida: " + room.name + " - Total habitaciones: " + generatedRooms.Count);
    }

    public void AddCorridor(GameObject corridor)
    {
        // Desactivar el pasillo anterior si existe
        if (generatedCorridors.Count > 0)
        {
            GameObject previousCorridor = generatedCorridors[generatedCorridors.Count - 1];
            previousCorridor.SetActive(false); 
            Debug.Log("Pasillo anterior desactivado: " + previousCorridor.name);
        }

        generatedCorridors.Add(corridor);
        Debug.Log("Nuevo pasillo añadido: " + corridor.name + " - Total pasillos: " + generatedCorridors.Count);
    }

    public int GetRoomCount()
    {
        return generatedRooms.Count;
    }

    public int GetCorridorCount()
    {
        return generatedCorridors.Count;
    }

    private void Start()
    {
        currentState = GameState.OnMatch;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gun.pauseActive = false;
        playerlook.pauseActive = false;
        frenzyManager.enabled = true;
        pauseMenu.enabled = true;
        victoryUI.SetActive(false);
        defeatUI.SetActive(false);

        // Hacer que el fade inicie transparente
        fadeCanvasGroup.alpha = 0;
    }

    public void ShowVictoryScreen()
    {
        if (currentState == GameState.Victory) return;
        Time.timeScale = 0;
        StartCoroutine(FadeAndShowUI(victoryUI));
        currentState = GameState.Victory;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerlook.pauseActive = true;
        gun.pauseActive = true;
        frenzyManager.enabled = false;
        pauseMenu.enabled = false;
    }

    public void ShowDefeatScreen()
    {
        if (currentState == GameState.Defeat) return;
        Time.timeScale = 0;
        StartCoroutine(FadeAndShowUI(defeatUI));
        currentState = GameState.Defeat;
        playerlook.pauseActive = true;
        gun.pauseActive = true;
        frenzyManager.enabled = false;
        pauseMenu.enabled = false;

        // Esperar y luego recargar la escena
        StartCoroutine(WaitAndReloadScene());
    }

    private IEnumerator FadeAndShowUI(GameObject uiElement)
    {
        // Mostrar la UI
        uiElement.SetActive(true);

        // Fade in
        yield return StartCoroutine(Fade(1f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        // Desvanecer la imagen
        float startAlpha = fadeCanvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }

    private IEnumerator WaitAndReloadScene()
    {
        // Espera antes de recargar la escena
        yield return new WaitForSecondsRealtime(1f);

        // Cargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RetryButton()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gun.pauseActive = false;
        playerlook.pauseActive = false;
        frenzyManager.enabled = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void TutorialButton()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gun.pauseActive = false;
        playerlook.pauseActive = false;
        frenzyManager.enabled = true;
        SceneManager.LoadScene("Level1");
    }

    public void Level1Button()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gun.pauseActive = false;
        playerlook.pauseActive = false;
        frenzyManager.enabled = true;
        SceneManager.LoadScene("Level2");
    }

    public void Level2Button()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gun.pauseActive = false;
        playerlook.pauseActive = false;
        frenzyManager.enabled = true;
        SceneManager.LoadScene("Level3");
    }

    public void Level3Button()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gun.pauseActive = false;
        playerlook.pauseActive = false;
        frenzyManager.enabled = true;
        SceneManager.LoadScene("Level4");
    }

    public void Level4Button()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gun.pauseActive = false;
        playerlook.pauseActive = false;
        frenzyManager.enabled = true;
        SceneManager.LoadScene("Endless");
    }
}
