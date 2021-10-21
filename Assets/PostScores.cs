using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PostScores : MonoBehaviour
{
   /* public static PostScores _instance;

    public static PostScores instance
    {
        get
        {
            if (_instance == null)
            { _instance = FindObjectOfType<PostScores>(); }
            return _instance;
        }
    }

    public TextMeshProUGUI PlayerScore;
    //public TextMeshProUGUI PlayerLevel;
    public Text scoresentstatus;

    [HideInInspector]
    public int score;

    //[HideInInspector]
    //public string level;


    private void Start()
    {
       // score = 0;
        //level = "";
        StartCoroutine("SendScoresAndLevels");
    }

    private IEnumerator SendScoresAndLevels()
    {
        yield return new WaitForSeconds(1f);
        GoogleSignInDemo.Instance.postScoresLevels();
    }

    private void Update()
    {
        score = PlayerPrefs.GetInt("serverScore");
        //level = PlayerLevel.text;

        //print(score);
        //print(level);
    }

    public void callPostScoreApi()
    {
        scoresentstatus.text = "Trying Again to send scores";
        GoogleSignInDemo.Instance.postScoresLevels();
    }*/
}