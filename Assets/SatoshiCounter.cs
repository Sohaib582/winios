using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[SerializeField]
public class SatoshiCounter : MonoBehaviour
{
    public Button withDrawButton;

    [Header("Transaction Values")]
    public Transform templateForTrans;

    public Transform containerForTrans;
    
    [Header("Leaderboard Values")]
    public Transform templateForLeaderboard;

    public Transform containerForLeaderboard;

    [Header("Payout Values")]
    public Transform templateForPayout;

    public Transform containerForPayout;

    [HideInInspector]
    public string finalVal;

    [HideInInspector]
    public double stoshi;

    [Space(20)]
    public TMP_Text textMesh;

    public Text txt;
    public Text dollarText;
    public Text Stoshitxt;
    public Text Stoshitxt2;
    public Text errorMessage;
    public Text EmptyField2;
    public InputField amountInput;
    public InputField addressInput;
    public static SatoshiCounter _instance;

    public static SatoshiCounter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SatoshiCounter>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // dataFillInTable("ads","45","adsfdg","pending");
    }

    public void Update()
    {
        //PlayerPrefs.DeleteKey("scorecount");
        int scorecount = PlayerPrefs.GetInt("scorecount");
        // dividing by ten to convert in satoshi

        stoshi = scorecount;
        Stoshitxt.text = stoshi.ToString();
        //converting into satoshi
        stoshi = stoshi / 100000000000;
        finalVal = string.Format("{0:N11}", stoshi);
         //print(finalVal + " final val");
        //print(stoshi + " stoshi");
        textMesh.text = finalVal;
        txt.text = finalVal.ToString();

        double d = double.Parse(finalVal);
        d = d * 100000000000;
        //print(d + " value of D");
        //int a = (int)(d);
    }

    public void checkForNegativeAmount()
    {
        double val = double.Parse(amountInput.text);

        double d = double.Parse(finalVal);

        int temp = PlayerPrefs.GetInt("scorecount");
        if (val <= 0 || val > d)
        {
            EmptyField2.text = "";
            errorMessage.text = "*Invalid Amount";
            withDrawButton.interactable = false;
            StartCoroutine(hideText());
        }
        else if (val == d)
        {
            EmptyField2.text = "";
            errorMessage.text = "";
            withDrawButton.interactable = true;
        }
        else
        {
            EmptyField2.text = "";
            errorMessage.text = "";
            withDrawButton.interactable = true;
        }
    }

    private IEnumerator hideText()
    {
        yield return new WaitForSeconds(2f);
        errorMessage.text = "";
        EmptyField2.text = "";
    }

    public void dataFillInTable(string to, string amount, string hash, string status, int i)
    {
        float height = 120f;
#if UNITY_ANDROID || UNITY_IOS
        int count = GoogleSignInDemo.Instance.transactionEntries;
#endif
        templateForTrans.gameObject.SetActive(false);

        int rank = i + 1;
        Transform entryTransform = Instantiate(templateForTrans, containerForTrans);
        RectTransform entryrectTransform = entryTransform.GetComponent<RectTransform>();
        entryrectTransform.anchoredPosition = new Vector2(0, -height * i);
        entryrectTransform.Find("idText").GetComponent<Text>().text = rank.ToString();
        entryrectTransform.Find("toText").GetComponent<Text>().text = to.ToString();
        entryrectTransform.Find("amountText").GetComponent<Text>().text = amount.ToString();
        entryrectTransform.Find("hashText").GetComponent<Text>().text = hash.ToString();
        entryrectTransform.Find("statusText").GetComponent<Text>().text = status.ToString();
        entryTransform.gameObject.SetActive(true);
        //}
    }


    public void LbEntriesInTable(string score, string email, int i)
    {
        float height = 120f;
#if UNITY_ANDROID || UNITY_IOS
        int count = GoogleSignInDemo.Instance.leaderboardEntries;
#endif
        templateForLeaderboard.gameObject.SetActive(false);

        int rank = i + 1;
        Transform entryTransform = Instantiate(templateForLeaderboard, containerForLeaderboard);
        RectTransform entryrectTransform = entryTransform.GetComponent<RectTransform>();
        entryrectTransform.anchoredPosition = new Vector2(0, -height * i);
        entryrectTransform.Find("idText").GetComponent<Text>().text = rank.ToString();
        entryrectTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();
        entryrectTransform.Find("emailText").GetComponent<Text>().text = email.ToString(); 
        entryTransform.gameObject.SetActive(true);
    }


    public void PayoutEntriesInTable(string to, string amount, string market, string symbol, string status, int i)
    {
        float height = 120f;
#if UNITY_ANDROID || UNITY_IOS
        int count = GoogleSignInDemo.Instance.payoutEntries;
#endif
        templateForPayout.gameObject.SetActive(false);
        print("payouts entries in table working");
        int rank = i + 1;
        Transform entryTransform = Instantiate(templateForPayout, containerForPayout);
        RectTransform entryrectTransform = entryTransform.GetComponent<RectTransform>();
        entryrectTransform.anchoredPosition = new Vector2(0, -height * i);
        entryrectTransform.Find("idText").GetComponent<Text>().text = rank.ToString();
        entryrectTransform.Find("toText").GetComponent<Text>().text = to.ToString();
        entryrectTransform.Find("amountText").GetComponent<Text>().text = amount.ToString();
        entryrectTransform.Find("marketText").GetComponent<Text>().text = market.ToString();
        entryrectTransform.Find("symbolText").GetComponent<Text>().text = symbol.ToString();
        entryrectTransform.Find("statusText").GetComponent<Text>().text = status.ToString();
        entryTransform.gameObject.SetActive(true);
        //}
    }
}