using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject MenuPausa;
    public GameObject MenuSalir;
    public PlayerLook playerlook;
    public FrenzyManager frenzyManager;
    public GunSystem gun;
    public bool Pause = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(Pause == false)
            {
                MenuPausa.SetActive(true);
                Pause = true;

                Time.timeScale = 0;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                playerlook.pauseActive = true;
                gun.pauseActive = true;
                frenzyManager.enabled = false;
            }
            else if (Pause == true)
            {
                Resume();
            }
        }
    }

    public void Resume()
    {
        MenuPausa.SetActive(false);
        MenuSalir.SetActive(false);
        Pause = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gun.pauseActive = false;
        playerlook.pauseActive = false;
        frenzyManager.enabled = true;
    }

    public void GoToMenu(string NombreMenu)
    {
        SceneManager.LoadScene(NombreMenu);
        MusicManager.Instance.StopMusic();


        Pause = false;

        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        gun.pauseActive = false;
        playerlook.pauseActive = false;
        frenzyManager.enabled = false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
