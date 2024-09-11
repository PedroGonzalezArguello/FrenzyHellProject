using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timeElapsed = 0f; // Tiempo transcurrido en segundos
    [SerializeField] private TextMeshProUGUI timerText; // Referencia al componente Text de UI

    private bool timerIsRunning = true;

    public Respawn respawn;

    private void Start()
    {
        // Inicia el temporizador
        StartTimer();
    }

    private void Update()
    {
        // Actualiza el temporizador solo si está en funcionamiento
        if (timerIsRunning)
        {
            timeElapsed += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    private void StartTimer()
    {
        timerIsRunning = true;
    }

    private void UpdateTimerDisplay()
    {
        // Convierte el tiempo transcurrido a minutos y segundos y actualiza el texto del temporizador
        float minutes = Mathf.FloorToInt(timeElapsed / 60);
        float seconds = Mathf.FloorToInt(timeElapsed % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
