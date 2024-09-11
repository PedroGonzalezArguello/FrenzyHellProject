using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CanvasManager : MonoBehaviour
{
    public GameObject Bullets6;
    public GameObject Bullets5;
    public GameObject Bullets4;
    public GameObject Bullets3;
    public GameObject Bullets2;
    public GameObject Bullets1;
    public GameObject Bullets0;

    [SerializeField] GunSystem gunSystem;

    public enum GameState { bullets6, bullets5, bullets4, bullets3, bullets2, bullets1, bullets0 }
    private GameState currentState;

    private void Update()
    {
        if (gunSystem.bulletsLeft == 6)
        {
            ShowBullets();
            Bullets5.SetActive(false);
            Bullets4.SetActive(false);
            Bullets3.SetActive(false);
            Bullets2.SetActive(false);
            Bullets1.SetActive(false);
            Bullets0.SetActive(false);
        }
        else if(gunSystem.bulletsLeft == 5)
        {
            Show5Bullets();
        }
        else if (gunSystem.bulletsLeft == 4)
        {
            Show4Bullets();
        }
        else if (gunSystem.bulletsLeft == 3)
        {
            Show3Bullets();
        }
        else if (gunSystem.bulletsLeft == 2)
        {
            Show2Bullets();
        }
        else if (gunSystem.bulletsLeft == 1)
        {
            Show1Bullets();
        }
        else if (gunSystem.bulletsLeft == 0)
        {
            Show0Bullets();
        }

    }
    public void ShowBullets()
    {
        currentState = GameState.bullets6;
        Bullets6.SetActive(true);
    }
    public void Show5Bullets()
    {
        currentState = GameState.bullets5;
        Bullets6.SetActive(false);
        Bullets5.SetActive(true);
       

    }
    public void Show4Bullets()
    {
        currentState = GameState.bullets4;
        Bullets5.SetActive(false);
        Bullets4.SetActive(true);
        
    }
    public void Show3Bullets()
    {
        currentState = GameState.bullets3;
        Bullets4.SetActive(false);
        Bullets3.SetActive(true);
        
    }
    public void Show2Bullets()
    {
        currentState = GameState.bullets2;
        Bullets3.SetActive(false);
        Bullets2.SetActive(true);
        
    }
    public void Show1Bullets()
    {
        currentState = GameState.bullets1;
        Bullets2.SetActive(false);
        Bullets1.SetActive(true);
        
    }
    public void Show0Bullets()
    {
        currentState = GameState.bullets0;
        Bullets1.SetActive(false);
        Bullets0.SetActive(true);
        
    }

}
