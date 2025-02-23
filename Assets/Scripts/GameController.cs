using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;
    [SerializeField] private List<GameObject> enemies;
    private Animator enemyAnim;

    [SerializeField] private Button UpBtn, DownBtn, LeftBtn, RightBtn;
    [SerializeField] private List<GameObject> arrows;

    [SerializeField] private int arrowCount = 5;
    private int arrowIndex;

    [SerializeField] private Sprite upSprite, downSprite, leftSprite, rightSprite;
    private Sprite[] sprites;

    private string lastAnim;
    private string[] animTriggers = { "Up", "Down", "Left", "Right" };
    private int enemyIndex;
    private int winCount;
    private float nextTime = 1f;
    private bool done;

    void Start()
    {
        sprites = new Sprite[] { upSprite, downSprite, leftSprite, rightSprite };
        InitArrows();

        if (enemies.Count > 0) enemyAnim = enemies[enemyIndex].GetComponentInChildren<Animator>();
        enemies.ForEach(e => e.SetActive(false));
        enemies[enemyIndex].SetActive(true);

        UpBtn.onClick.AddListener(() => Slash(upSprite));
        DownBtn.onClick.AddListener(() => Slash(downSprite));
        LeftBtn.onClick.AddListener(() => Slash(leftSprite));
        RightBtn.onClick.AddListener(() => Slash(rightSprite));
    }

    void Update()
    {
        if (done) return;

        if (Input.GetKeyDown(KeyCode.UpArrow)) Slash(upSprite);
        if (Input.GetKeyDown(KeyCode.DownArrow)) Slash(downSprite);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) Slash(leftSprite);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Slash(rightSprite);

        if (TimeLine.Instance.GetFrom() == 0)
        {
            Debug.Log("Lose");
            SoundManager10.Instance.PlaySound(4);
            AnimHandler(enemyAnim, "Slash");// has exit time 0.25f
            AnimHandler(playerAnim, "Die");
            Invoke(nameof(DelayLose), 0.25f);// if error (fix has exit time = false)
            done = true;
        }
    }

    private void DelayLose() => GameManager10.Instance.GameLose();

    void InitArrows()
    {
        for (int i = 0; i < arrows.Count; i++)
        {
            Image img = arrows[i].GetComponent<Image>();
            if (i < arrowCount)
            {
                arrows[i].SetActive(true);
                img.sprite = sprites[Random.Range(0, sprites.Length)];
            }
            else arrows[i].SetActive(false);
        }
    }

    void Slash(Sprite sprite)
    {
        if (arrowIndex == arrowCount) return;

        if (!SameSprite(sprite))
        {
            SoundManager10.Instance.PlaySound(6);
            TimeLine.Instance.UpdateTimeChange(1f);
            return;
        }

        if (arrowIndex < arrowCount - 1) SoundManager10.Instance.PlaySound(5);// sound slash

        string newAnim;
        do
        {
            newAnim = animTriggers[Random.Range(0, animTriggers.Length)];
        } while (newAnim == lastAnim);
        lastAnim = newAnim;

        AnimHandler(playerAnim, newAnim);
        AnimHandler(enemyAnim, "Slash");

        UpdateArrows();
    }

    void UpdateArrows()
    {
        arrows[arrowIndex].SetActive(false);
        arrowIndex++;

        if (arrowIndex == arrowCount)
        {
            Debug.Log("Win");
            SoundManager10.Instance.PlaySound(4);// sound slash ket lieu
            GameManager10.Instance.IncreaseScore(arrowIndex * 10);
            TimeLine.Instance.ResetFillAmount(-1, 1, nextTime);
            TimeLine.Instance.ChangeDuration(0.2f);

            enemyAnim.SetTrigger("Die");
            StartCoroutine(SwitchEnemy());
        }
    }

    bool SameSprite(Sprite sprite) => arrows[arrowIndex].GetComponent<Image>().sprite == sprite;

    IEnumerator SwitchEnemy()
    {
        yield return new WaitForSeconds(nextTime);

        TimeLine.Instance.ResetFillAmount(1, 0, 0);

        enemies[enemyIndex].SetActive(false);
        enemyIndex = (enemyIndex + 1) % enemies.Count;
        enemyAnim = enemies[enemyIndex].GetComponentInChildren<Animator>();
        enemies[enemyIndex].SetActive(true);
        AnimHandler(enemyAnim, null);
        AnimHandler(playerAnim, null);

        arrowIndex = 0;
        winCount++;
        if (winCount % 5 == 0) arrowCount = Mathf.Min(arrowCount + 1, arrows.Count);
        InitArrows();
    }

    void AnimHandler(Animator animator, string nameAnim)
    {
        animator.Rebind();// reset animator -> idle
        animator.Update(0);// dam bao new anim run ngay
        if (nameAnim != null) animator.SetTrigger(nameAnim);
    }
}
