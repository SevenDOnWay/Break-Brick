using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VContainer;

public class BallManager : MonoBehaviour
{

    SelectState selectState;
    CharacterEntry characterEntry;
    [Inject, HideInInspector] public PlayScreen playScreen;


    private Dictionary<string, float> properties = new Dictionary<string, float> {
        {"Speed", 5},
        {"CritChance", 0},
        {"CritMultiplier", 2},
        {"FireChance", 0},
        {"LightningChance", 0},

    };
    List<GameObject> balls = new List<GameObject>();

    //public IReadOnlyDictionary<string, float> GetProperties() => properties;

    public delegate GameObject RequestBall();
    public RequestBall requestBall;
    public event Action OnAllBallsDone;

    public Vector2 ballPos;
    [SerializeField] TextMeshProUGUI t_BallCount;
    bool ballPosLocked = false;


    public void StartGame()
    {

        selectState = GameObject.FindGameObjectWithTag("Select State").GetComponent<SelectState>();
        characterEntry = GameObject.FindGameObjectWithTag("Character Entry").GetComponent<CharacterEntry>();

        ballPos = new Vector2(0, -playScreen.squareSize * 6);

        RequestExtraBall();

        characterEntry.characters[selectState.characterIndex].Apply(gameObject.GetComponent<BallManager>());

        UpdateText();
    }

    #region Upgrade_Logic
    public void RequestExtraBall(int extraballs = 1)
    {
        for (int i = 0; i < extraballs; i++)
        {
            balls.Add(requestBall());
        }

        //TODO: Update text in here for now.

        UpdateText();
    }

    public void ModifyProperty(string key, float value)
    {
        if (!properties.ContainsKey(key))
        {
            Debug.LogWarning($"Property {key} not found!");
            return;
        }
        // propagate to all existing balls

    }
    #endregion

    public void LaunchBall(Vector2 direction)
    {
        UnlockBallPos();
        StartCoroutine(LaunchSequence(direction));
        //Debug.Log($"Balls in list: {balls.Count}");
    }

    IEnumerator LaunchSequence(Vector2 direction)
    {
        float speed = properties["Speed"];
        //TODO: wait for done level up
        foreach (var ball in balls)
        {
            ball.GetComponent<Rigidbody2D>().AddForce(direction * speed, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.1f); // stagger launch
        }

        yield return WaitAllBalls();
    }

    IEnumerator WaitAllBalls()
    {
        int finishedCount = 0;
        int totalBalls = balls.Count;
        float beginTime = Time.time;

        Action<BallScript> onBallFinished = (ball) => finishedCount++;

        // Subscribe
        foreach (var ball in balls)
        {
            var script = ball.GetComponent<BallScript>();
            script.OnBallFinished += onBallFinished;
        }

        while (finishedCount < totalBalls)
        {
            if (Time.time > beginTime + 5f)
            {
                Debug.Log("Too long, speed up balls");
                foreach (var ball in balls)
                {
                    BallScript script = ball.GetComponent<BallScript>();
                    script.rb.linearVelocity *= 3;
                }
                beginTime += 10f;
            }
            yield return null; // wait 1 frame
        }

        // Wait until all balls are finished
        yield return new WaitUntil(() => finishedCount >= totalBalls);

        // Unsubscribe
        foreach (var ball in balls)
        {
            BallScript script = ball.GetComponent<BallScript>();
            script.OnBallFinished -= onBallFinished;
        }

        OnAllBallsDone?.Invoke();

        UpdateText();
        Debug.Log("All balls are done!");
    }

    void UpdateText()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(ballPos);

        if (ballPos.x > 0)
        {
            t_BallCount.transform.position = screenPos + new Vector3(-20, 150, 0);
        }
        else
        {
            t_BallCount.transform.position = screenPos + new Vector3(20, 150, 0);
        }


        t_BallCount.text = balls.Count.ToString();
    }

    public void ResetBallPos(Vector2 newPos)
    {
        if (!ballPosLocked)
        {
            ballPos = newPos;
            ballPosLocked = true; // only first ball can update
        }
    }

    public void UnlockBallPos()
    {
        ballPosLocked = false;
    }



}
