using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeLine : Singleton<TimeLine>
{
    [SerializeField] private Image timeLineImg;
    public float duration = 10f;
    private float timer;
    private float timeChange;
    private float from, to;

    // Start is called before the first frame update
    void Start()
    {
        timeLineImg.fillAmount = 1;
        from = GetFrom();
        timeChange = duration;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLineImg && GetFrom() != to)
            ChangeFillAmount();
    }

    public void ResetFillAmount(float from, float to, float timeReset)
    {
        timer = 0;
        this.from = (from == -1) ? GetFrom() : from;
        this.to = to;
        timeChange = (timeReset <= 0) ? duration : timeReset;
    }

    public void ChangeFillAmount()
    {
        timer += Time.deltaTime;
        timeLineImg.fillAmount = Mathf.Lerp(from, to, timer / timeChange);
    }

    public float GetFrom() => timeLineImg.fillAmount;
    public void ChangeDuration(float time) => duration = Mathf.Max(duration -= time, 3f);
    public void UpdateTimeChange(float time) => timeChange = Mathf.Max(timeChange -= time, 0);
}
