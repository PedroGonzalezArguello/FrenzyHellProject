using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
        MusicManager.Instance.RestartMusic();
    }

    public void Jugar(string Level1)
    {
        SceneManager.LoadScene(Level1);
    }
    public void Salir()
    {

        //Salir del juego.
        Application.Quit();
        print("saliste del juego");
    }
}
