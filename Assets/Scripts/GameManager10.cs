using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System.Threading.Tasks;

public class GameManager10 : Singleton<GameManager10>
{
    [SerializeField] private TextMeshProUGUI scoreText, scoreText2, bestText;
    [SerializeField] private GameObject loseMenu, pauseMenu;
    [SerializeField] private RectTransform losePanel, pausePanel;
    [SerializeField] private float topPosY = 250f, middlePosY, tweenDuration = 0.3f;

    private int score;

    protected override void Awake()
    {
        base.Awake();
    }

    async void Start()
    {
        if (scoreText && scoreText2 && bestText) UpdateScore(0);

        if (loseMenu && losePanel) await HidePanel(loseMenu, losePanel);
        if (pauseMenu && pausePanel) await HidePanel(pauseMenu, pausePanel);
    }

    public void Home() => SceneManager.LoadScene("Start");
    public void StartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    public void Retry() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void PauseGame()
    {
        SoundManager10.Instance.SoundClick();
        ShowPanel(pauseMenu, pausePanel);
    }

    public async void ResumeGame()
    {
        SoundManager10.Instance.SoundClick();
        await HidePanel(pauseMenu, pausePanel);
        Time.timeScale = 1f;
    }

    public void GameLose() => EndGame(loseMenu, losePanel, 3);

    private void EndGame(GameObject menu, RectTransform panel, int soundIndex)
    {
        SoundManager10.Instance.PlaySound(soundIndex);
        ShowPanel(menu, panel);
    }

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

    // score
    public void IncreaseScore(int point) => UpdateScore(score + point);

    public void UpdateScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();
        scoreText2.text = score.ToString();
        UpdateBest();
    }

    private void UpdateBest()
    {
        if (score > LoadBest())
            PlayerPrefs.SetInt("best", score);
        bestText.text = LoadBest().ToString();
    }

    private int LoadBest() => PlayerPrefs.GetInt("best", 0);
}
