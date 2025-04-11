using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    public GameObject buttonMenuUI;

    public GameObject controlsMenuUI;

    private GameObject darkBackground;

    private VolumeSettings volumeSettings;

    void Start()
    {
        darkBackground = UIManager.Instance.darkBackground;
        volumeSettings = FindObjectOfType<VolumeSettings>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else if (UIManager.Instance.IsUIActive())
            {
                UIManager.Instance.CloseOpenUIElements();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        darkBackground.SetActive(false);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        buttonMenuUI.SetActive(true);
        controlsMenuUI.SetActive(false);

        volumeSettings?.SetVolumeMultiplier(1f);
    }

    void Pause()
    {
        darkBackground.SetActive(true);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;

        volumeSettings?.SetVolumeMultiplier(0.5f);
    }

    public void LoadMenu()
    {
        darkBackground.SetActive(false);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        GameObject essentials = GameObject.Find("Essentials");
        Destroy(essentials);
        SceneManager.LoadScene(0);

        volumeSettings?.SetVolumeMultiplier(1f);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadControls()
    {
        buttonMenuUI.SetActive(false);
        controlsMenuUI.SetActive(true);
    }

    public void LoadButtons()
    {
        buttonMenuUI.SetActive(true);
        controlsMenuUI.SetActive(false);
    }
}
