using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject victoryUI;
    public GameObject defeatUI;
    public CanvasGroup fadeCanvasGroup; // Arrastra tu CanvasGroup aquí
    private float fadeDuration = 0.5f; // Duración del fade

    public PlayerLook playerlook;
    public FrenzyManager frenzyManager;
    public GunSystem gun;
    public PauseMenu pauseMenu;

    public enum GameState { Victory, Defeat, OnMatch }
    private GameState currentState;

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
        SceneManager.LoadScene("Level4");
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
        SceneManager.LoadScene("MainMenu");
    }
}
