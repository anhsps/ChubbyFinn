using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;

public class GameManager11 : Singleton<GameManager11>
{
    public static int level = 1;
    [SerializeField] private TextMeshProUGUI lvText;
    [SerializeField] private GameObject winMenu, /*loseMenu, */pauseMenu;
    [SerializeField] private RectTransform winPanel, /*losePanel, */pausePanel;
    [SerializeField] private float topPosY = 250f, middlePosY, tweenDuration = 0.3f;

    protected override void Awake()
    {
        base.Awake();
    }

    async void Start()
    {
        if (lvText) lvText.text = "LEVEL " + (level < 10 ? "0" + level : level);

        await HidePanel(winMenu, winPanel);
        //await HidePanel(loseMenu, losePanel);
        await HidePanel(pauseMenu, pausePanel);
    }

    //public void StartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    public void Retry() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void NextLV()
    {
        level++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PauseGame()
    {
        SoundManager11.Instance.SoundClick();
        ShowPanel(pauseMenu, pausePanel);
    }

    public async void ResumeGame()
    {
        SoundManager11.Instance.SoundClick();
        await HidePanel(pauseMenu, pausePanel);
        Time.timeScale = 1f;
    }

    public void GameWin()
    {
        UnlockNextLevel();
        EndGame(winMenu, winPanel, 2);
    }

    //public void GameLose() => EndGame(loseMenu, losePanel, 3);

    private void EndGame(GameObject menu, RectTransform panel, int soundIndex)
    {
        SoundManager11.Instance.PlaySound(soundIndex);
        ShowPanel(menu, panel);
    }

    public void UnlockNextLevel()
    {
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        if (level >= unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", level + 1);
            PlayerPrefs.Save();
        }
    }

    public void SetCurrentLV(int levelIndex) => SceneManager.LoadScene((level = levelIndex).ToString());

    private void ShowPanel(GameObject menu, RectTransform panel)
    {
        menu.SetActive(true);
        Time.timeScale = 0f;
        menu.GetComponent<CanvasGroup>().DOFade(1, tweenDuration).SetUpdate(true);
        panel.DOAnchorPosY(middlePosY, tweenDuration).SetUpdate(true);
    }

    private async Task HidePanel(GameObject menu, RectTransform panel)
    {
        if (menu == null || panel == null) return;

        panel.DOKill();// huy tween dang chay
        menu.GetComponent<CanvasGroup>().DOKill();

        menu.GetComponent<CanvasGroup>().DOFade(0, tweenDuration).SetUpdate(true);
        await panel.DOAnchorPosY(topPosY, tweenDuration).SetUpdate(true).AsyncWaitForCompletion();
        menu.SetActive(false);
    }
}
