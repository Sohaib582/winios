
using Firebase;
using Firebase.Auth;
using Google;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoogleSignInDemo : MonoBehaviour
{
    public GameObject NoPayoutsText;
    public GameObject LeaderboardBtn;
    public Toggle Binance;
    public Toggle Coinbase;
    public GameObject closeBtn;
    public Text usernameforgame;
    public Text currentvaluetext;
    public Text PrefsCheckForJWT;
    public Text PrefsCheckForUID;
    public InputField addressInputField;
    public InputField amountInputField;
    public GameObject loadingPanel;
    public GameObject withdrawimage;
    public GameObject withdrawtext;
    public GameObject successfull;
    public GameObject failed;
    public Text mytext;
    public Text mytext1;
    public Text amount;
    public Text address;
    public Text noHistoryFoundText;
    public Text EmptyField1;
    public Text EmptyField2;
    private RequestHelper currentRequest;
    private readonly string basePath = "https://inovixion-backend-games-apis-54ezow346q-uc.a.run.app/";

    [HideInInspector]
    public string firebaseIdToken = "";

    [HideInInspector]
    public string playerName;

    [HideInInspector]
    public string Uids = "";

    public Text infoText;
    public string webClientId = "134472923530-ahg61cap7hdur6grg8r27cggegi6hhpp.apps.googleusercontent.com";
    public GameObject signInButton;
    public GameObject CashoutButton;
    public GameObject logInPanel;
    public GameObject CashOutPanel;
    public GameObject leaderboardPanel;
    public GameObject historyPanel;
    public GameObject LevelsMap;

    [HideInInspector]
    public int transactionEntries;

    [HideInInspector]
    public int leaderboardEntries;

    [HideInInspector]
    public int payoutEntries;

    [HideInInspector]
    public double currentVal;

    public double limitVal = 0.000001;

    [HideInInspector]
    public int playerScore;

    [HideInInspector]
    public string signatureForScore;

    [HideInInspector]
    public string signatureForTrans;

    [HideInInspector]
    public bool dailyreward = false;

    [HideInInspector]
    public string userChoiceForWithdraw = "";

    public static UnityEngine.Event Event;
    private FirebaseAuth auth;
    private GoogleSignInConfiguration configuration;

    private void LogMessage(string title, string message)
    {
#if UNITY_EDITOR
        EditorUtility.DisplayDialog(title, message, "Ok");
#else
		Debug.Log(message);
#endif
    }

    
    public static GoogleSignInDemo _instance;

    public static GoogleSignInDemo Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GoogleSignInDemo>();
            }
            return _instance;
        }
    }

    public void Update()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform== RuntimePlatform.Android)
        {
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (CashOutPanel.gameObject.activeInHierarchy == false && leaderboardPanel.gameObject.activeInHierarchy == false && LevelsMap.gameObject.activeInHierarchy)
                {
                    Scene scene = SceneManager.GetActiveScene();
                    if (scene.name == "game")
                    {
//                        YodoAdss.instance.ShowInterstitialAds();
                        SceneManager.LoadScene("main");
                    }
                }
                if (CashOutPanel.gameObject.activeInHierarchy)
                {
                    CashOutPanel.gameObject.SetActive(false);
                }
                if (leaderboardPanel.gameObject.activeInHierarchy)
                {
                    leaderboardPanel.gameObject.SetActive(false);
                }
            }
        }

        //print(PlayerPrefs.GetInt("server"));
        //print(PlayerPrefs.GetString("type"));

        currentVal = SatoshiCounter.Instance.stoshi;
        
        //for testing
        /*print(currentVal+" current val");
        print(limitVal + " limitval");
        if(currentVal > limitVal)
        {
            print("current val is greater");
        }
        else if(currentVal == limitVal)
        {
            print("equal vals");
        }
        else if(limitVal > currentVal)
        {
            print("limit val is greater");
        }*/

        firebaseIdToken = PlayerPrefs.GetString("idtoken");
        Uids = PlayerPrefs.GetString("UID");

        PrefsCheckForJWT.text = PlayerPrefs.GetString("idtoken") /*+ ":::::::::::: THIS IS FIREBASE ID TOKENNNNNNNNNNNNNNNNNNN"*/;
        PrefsCheckForUID.text = PlayerPrefs.GetString("UID");

        string playerScoree = SatoshiCounter.Instance.Stoshitxt.text;  //setting amountInINX for post withdraw request
        playerScore = int.Parse(playerScoree);                        //playerScore = amountInINX
    }

    public void userWithdrawCoinbase()
    {
        if (Coinbase.isOn == false && Binance.isOn == false)
        {
            Binance.isOn = true;
            userChoiceForWithdraw = "BINANCE";
        }
        if (Coinbase.isOn == true)
        {
            userChoiceForWithdraw = "COINBASE";
            Binance.isOn = false;
            Coinbase.isOn = true;
        }
    }

    public void userWithdrawBinance()
    {
        if (Coinbase.isOn == false && Binance.isOn == false)
        {
            Binance.isOn = true;
            userChoiceForWithdraw = "BINANCE";
        }
        if (Binance.isOn == true)
        {
            userChoiceForWithdraw = "BINANCE";
            Coinbase.isOn = false;
            Binance.isOn = true;
        }
    }

    private string getHMACBase64Text(string text)
    {
        string privateKey = "zsh3LfEFaLfadLEmCR5h5yeNEy5vuv";

        using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(privateKey)))
        {
            var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(text));
            return Convert.ToBase64String(hash);
        }
    }

    public void Start()
    {
        if (CashoutButton.gameObject.activeInHierarchy == true)
        {
            StartCoroutine("loginSilently");
        }
        userChoiceForWithdraw = "BINANCE";

        StartCoroutine(callGetScores());
    }

    private IEnumerator callGetScores()
    {
        yield return new WaitForSeconds(2f);
        GetUpdatedScore();
    }

    private IEnumerator loginSilently()
    {
        yield return new WaitForSeconds(0.1f);
        OnSignInSilently();
    }

    private void Awake()
    {
        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
        CheckFirebaseDependencies();
        if (PlayerPrefs.GetInt("signin") == 0)
        {
            signInButton.gameObject.SetActive(true);
            CashoutButton.gameObject.SetActive(false);
            CashOutPanel.gameObject.SetActive(false);
        }
        if (PlayerPrefs.GetInt("signin") == 1)
        {
            signInButton.gameObject.SetActive(false);
            CashoutButton.gameObject.SetActive(true);
        }
    }

    private void CheckFirebaseDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                    auth = FirebaseAuth.DefaultInstance;
                else
                    AddToInformation("Could not resolve all Firebase dependencies: " + task.Result.ToString());
            }
            else
            {
                AddToInformation("Dependency check was not completed. Error : " + task.Exception.Message);
            }
        });
    }

    public void SignOutFromGoogle()
    {
        OnSignOut();
    }

    public void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddToInformation("Calling SignIn");

        //setting scores to 0
        PlayerPrefs.DeleteKey("scorecount");
        SweetSugar.Scripts.Core.InitScript.Instance.deletegems();
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private void OnSignOut()
    {
        AddToInformation("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
        signInButton.gameObject.SetActive(true);
        CashoutButton.gameObject.SetActive(false);
        CashOutPanel.gameObject.SetActive(false);
        PlayerPrefs.SetInt("signin", 0);
    }

    public void OnDisconnect()
    {
        AddToInformation("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    AddToInformation("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    AddToInformation("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            AddToInformation("Canceled");
        }
        else
        {
            AddToInformation("Welcome: " + task.Result.DisplayName + "!");
            //AddToInformation("Email = " + task.Result.Email);
            // AddToInformation("Google ID Token = " + task.Result.IdToken);

            SignInWithGoogleOnFirebase(task.Result.IdToken);

            Uids = task.Result.UserId;
            PlayerPrefs.SetString("UID", Uids);
            Uids = PlayerPrefs.GetString("UID");
            signInButton.gameObject.SetActive(false);
            CashoutButton.gameObject.SetActive(true);
            PlayerPrefs.SetInt("signin", 1);
            playerName = task.Result.DisplayName;
            usernameforgame.text = task.Result.DisplayName;
            closeBtn.gameObject.SetActive(true);
            LeaderboardBtn.gameObject.SetActive(true);
            PlayerPrefs.SetInt("leaderboardbtn", 1);
            putData();

            //to hide login panel on each level
            PlayerPrefs.SetInt("guest", 1);
        }
    }

    public void putData()
    {
        datacontainer datacontainer = new datacontainer();
        RestClient.Put("https://inovixion-games-4b507-default-rtdb.firebaseio.com/" + playerName + ".json", datacontainer);
    }

    private void SignInWithGoogleOnFirebase(string IdToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(IdToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                    AddToInformation("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
            }
            else
            {
                Firebase.Auth.FirebaseUser user = auth.CurrentUser;
                AddToInformation("BEFORE TOKEN SYNCING");

                user.TokenAsync(true).ContinueWith(tokenSyncTask =>
                {
                    if (tokenSyncTask.IsCanceled)
                    {
                        AddToInformation(" TOKEN SYNCINC CANCELED");

                        Debug.LogError("TokenAsync was canceled.");
                        return;
                    }

                    if (tokenSyncTask.IsFaulted)
                    {
                        AddToInformation("TokenAsync encountered an error: " + task.Exception);

                        Debug.LogError("TokenAsync encountered an error: " + task.Exception);
                        return;
                    }
                    AddToInformation("working on");

                    firebaseIdToken = tokenSyncTask.Result;

                    //saves token locally
                    PlayerPrefs.SetString("idtoken", firebaseIdToken);
                    firebaseIdToken = PlayerPrefs.GetString("idtoken");

                    //AddToInformation(firebaseIdToken + "   :::::THIS IS FIREBASE ID TOKEN:::::");
                    // Send token to your backend via HTTPS
                    // ...
                    dailyreward = true;
                    GetUpdatedScore();
                });
                AddToInformation("Sign In Successful.");
            }
        });
    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddToInformation("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
        //usernameforgame.text = "signed in successfully;";
    }

    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        AddToInformation("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private void AddToInformation(string str)
    {
        infoText.text += "\n" + str;
    }

    public void GetUserWithdrawTransactions()
    {
        currentRequest = new RequestHelper
        {
            Uri = basePath + "user/transactions",
            Method = "GET",

            Headers = new Dictionary<string, string>
            {
                { "jwt", PlayerPrefs.GetString("idtoken") }
            },

            IgnoreHttpException = false, //Prevent to catch http exceptions

            UseHttpContinue = true,
        };

        RestClient.Get(currentRequest).Then(res =>
        {
            //print(res.Text);
            Root1 root = JsonUtility.FromJson<Root1>(res.Text);
            transactionEntries = root.transactions.Count();

            if (transactionEntries == 0)
            {   //print("zero entries");
                noHistoryFoundText.gameObject.SetActive(true);
                historyPanel.gameObject.SetActive(false);
                //print("callingwithdraw;");
            }
            else if (transactionEntries >= 1)
            {
                //print("greater than 1");
                for (int i = 0; i < root.transactions.Count; i++)
                {
                    SatoshiCounter.Instance.dataFillInTable(root.transactions[i].to, root.transactions[i].amount.ToString(), root.transactions[i]._id, root.transactions[i].status, i);
                    //print(root.transactions[i].amount.ToString());
                    noHistoryFoundText.gameObject.SetActive(false);
                    historyPanel.gameObject.SetActive(true);
                }
            }

            loadingPanel.gameObject.SetActive(false);
            //loadingPanel.transform.GetChild(0).GetComponent<Text>().text = "Loading...";
        })

            .Catch(err => Loader(err));
    }

    public void Loader(System.Exception s)
    {
        loadingPanel.transform.GetChild(0).GetComponent<Text>().text = s.ToString();
        failed.SetActive(true);
        loadingPanel.gameObject.SetActive(false);
        Invoke("hide", 5);
    }

    public void hide()
    {
        //loadingPanel.transform.GetChild(0).GetComponent<Text>().text = "Loading...";
        failed.SetActive(false);
    }

    public void onWithdrawButtonClicked()
    {
        if (addressInputField.text == "")
        {
            addressInputField.image.color = new Color32(212, 173, 173, 255);
            EmptyField1.text = "*required";
            StartCoroutine(CheckforStringinInputField1());
        }
        else if (amountInputField.text == "")
        {
            amountInputField.image.color = new Color32(212, 173, 173, 255);
            EmptyField2.text = "*required";
            StartCoroutine(CheckforStringinInputField2());
        }
        else if (addressInputField.text == "" &&
                 amountInputField.text == "")
        {
            amountInputField.image.color = new Color32(212, 173, 173, 255);
            addressInputField.image.color = new Color32(212, 173, 173, 255);
            EmptyField1.text = "*required";
            EmptyField2.text = "*required";
            StartCoroutine(checkbothfields());
        }
        else if (amountInputField.text != "" && currentVal < limitVal)
        {
            amountInputField.image.color = new Color32(212, 173, 173, 255);
            EmptyField2.text = "Amount is less than 0.000001 BTC";
            StartCoroutine(LimitAmount());
        }
        else if (addressInputField.text != "" && amountInputField.text != "")
        {
            long requestTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            signatureForTrans = "address=" + address.text.ToString() + "&amountInINX=" + playerScore.ToString() + "&symbol=BTC" + "&market=" + userChoiceForWithdraw + "&timestamp="
                      + requestTimeStamp.ToString() + "&";

            /*print("address=" + address.text.ToString() + "&amountInINX=" + playerScore.ToString() + "&symbol=BTC&timestamp="
                      + requestTimeStamp.ToString() + "&");*/
            withdrawtext.SetActive(false);
            withdrawimage.SetActive(true);
            //print(getHMACBase64Text(signatureForTrans));
            currentRequest = new RequestHelper
            {
                Uri = basePath + "user/withdraw",
                Method = "POST",

                Headers = new Dictionary<string, string>
                {
                    { "jwt", PlayerPrefs.GetString("idtoken") }
                },

                Body = new IWithdrawPayment
                {
                    address = address.text.ToString(),
                    //amount = amount.text.ToString(),
                    amountInINX = playerScore,
                    symbol = "BTC",
                    market = userChoiceForWithdraw,
                    signature = getHMACBase64Text(signatureForTrans),
                    timestamp = requestTimeStamp
                },
                IgnoreHttpException = false, //Prevent to catch http exceptions

                UseHttpContinue = true,
            };

            RestClient.Post<IWithdrawPayment>(currentRequest)
           .Then(res =>
           {
               withdrawimage.SetActive(false);
               withdrawtext.SetActive(true);
               // And later we can clear the default query string params for all requests
               RestClient.ClearDefaultParams();
               //loadingPanel.transform.GetChild(0).GetComponent<Text>().text= JsonUtility.ToJson(res, true);
               clearallInputs();
               mytext.text = JsonUtility.ToJson(res, true);
               //this.LogMessage("Success", JsonUtility.ToJson(res, true));
               successOnWithDraw();
           })

            .Catch(err => failedOnWithdraw(err));
        }
    }

    private IEnumerator LimitAmount()
    {
        yield return new WaitForSeconds(3f);
        amountInputField.image.color = new Color32(255, 255, 255, 255);
        EmptyField2.text = "";
    }

    private IEnumerator CheckforStringinInputField1()
    {
        yield return new WaitForSeconds(3f);
        addressInputField.image.color = new Color32(255, 255, 255, 255);
        EmptyField1.text = "";
    }

    private IEnumerator CheckforStringinInputField2()
    {
        yield return new WaitForSeconds(3f);
        amountInputField.image.color = new Color32(255, 255, 255, 255);
        EmptyField2.text = "";
    }

    private IEnumerator checkbothfields()
    {
        yield return new WaitForSeconds(3f);
        addressInputField.image.color = new Color32(255, 255, 255, 255);
        EmptyField1.text = "";
        amountInputField.image.color = new Color32(255, 255, 255, 255);
        EmptyField2.text = "";
    }

    public void clearallInputs()
    {
        amountInputField.image.color = new Color32(255, 255, 255, 255);
        addressInputField.image.color = new Color32(255, 255, 255, 255);
        addressInputField.text = "";
        amountInputField.text = "";
        EmptyField1.text = "";
        EmptyField2.text = "";
    }

    public void successOnWithDraw()
    {
        SatoshiCounter.Instance.withDrawButton.interactable = false;
        //todo success
        successfull.SetActive(true);
        Invoke("hideSuccess", 3);
        clearallInputs();

        /* double d = double.Parse(SatoshiCounter.Instance.finalVal);
         d = d * 10000000000;
         int a = (int)(d);*/

       
        PlayerPrefs.SetInt("scorecount", -PlayerPrefs.GetInt("scorecount"));

        PlayerPrefs.SetInt("serverScore", -PlayerPrefs.GetInt("serverScore"));

        //sent scores and type to server for withdrawing cash
        PlayerPrefs.SetInt("server", 0);
        PlayerPrefs.SetInt("server", -PlayerPrefs.GetInt("server"));
        //print(a);
        PlayerPrefs.SetString("type", "");
        PlayerPrefs.SetString("type", "withdraw_cash");
        GoogleSignInDemo.Instance.postScoresLevels();

        SweetSugar.Scripts.Core.InitScript.Instance.deletegems();
    }

    public void failedOnWithdraw(System.Exception s)
    {
        print(s.ToString());
        // todo failed
        SatoshiCounter.Instance.withDrawButton.interactable = false;
        loadingPanel.SetActive(false);
        failed.SetActive(true);
        Invoke("hideFailed", 3);
        //this.LogMessage("Error", err.Message);
    }

    public void MaxValue()
    {
        if (limitVal > currentVal)
        {
            amountInputField.image.color = new Color32(212, 173, 173, 255);
            EmptyField2.text = "Amount is less than 0.000001 BTC";
            StartCoroutine(LimitAmount());           
        }
        else if(currentVal >= limitVal)
        {
            amountInputField.text = SatoshiCounter.Instance.finalVal;
            amountInputField.onEndEdit.Invoke("");

        }
       
    }

    public void hideSuccess()
    {
        successfull.SetActive(false);
    }

    public void hideFailed()
    {
        failed.SetActive(false);
    }

    public void ChangeCash()
    {
        double d = double.Parse(SatoshiCounter.Instance.finalVal);
        currentRequest = new RequestHelper
        {
            Uri = basePath + "rate/BTC/" + d,
            Method = "GET",

            Headers = new Dictionary<string, string>
            {
                { "jwt",PlayerPrefs.GetString("idtoken") }
            },

            IgnoreHttpException = false, //Prevent to catch http exceptions

            UseHttpContinue = true,
        };

        RestClient.Get(currentRequest).Then(res =>
        {
            float f = float.Parse(res.Text);
            string s = string.Format("{0:N6}", f);
            SatoshiCounter.Instance.dollarText.text = s;
        }).Catch(err =>
        print(err)
        );
    }

    public void postScoresLevels()
    {
        long requestTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        signatureForScore = "score=" + PlayerPrefs.GetInt("server").ToString() + "&type=" + PlayerPrefs.GetString("type") + "&timestamp=" + requestTimeStamp.ToString() + "&";
        currentRequest = new RequestHelper
        {
            Uri = basePath + "user/scores",
            Method = "POST",

            Headers = new Dictionary<string, string>
            {
                { "jwt", PlayerPrefs.GetString("idtoken") }
            },

            Body = new postLevelScores
            {
                score = PlayerPrefs.GetInt("server"),
                type = PlayerPrefs.GetString("type"),
                signature = getHMACBase64Text(signatureForScore),
                timestamp = requestTimeStamp,
                message = signatureForScore
            },
            IgnoreHttpException = true, //Prevent to catch http exceptions

            UseHttpContinue = true,
        };

        RestClient.Post<postLevelScores>(currentRequest)

       .Then(res =>
       {
           // And later we can clear the default query string params for all requests
           RestClient.ClearDefaultParams();
           Debug.Log("success in sending scores");
           GetUpdatedScore();
       })
        .Catch(err => failedOnPostingScores(err));
    }

    public void failedOnPostingScores(Exception err)
    {
        CheckInternetConnection.instance.ConnectionErrorText.gameObject.SetActive(true);
        Debug.Log("failure in sending scores");
        postScoresLevels();
    }

    public void GetUpdatedScore()
    {
        currentRequest = new RequestHelper
        {
            Uri = basePath + "user/scores",
            Method = "GET",

            Headers = new Dictionary<string, string>
            {
                { "jwt", PlayerPrefs.GetString("idtoken") }
            },

            IgnoreHttpException = false, //Prevent to catch http exceptions

            UseHttpContinue = true,
        };

        RestClient.Get(currentRequest).Then(res =>
        {
            getTotalScores root = JsonUtility.FromJson<getTotalScores>(res.Text);

            PlayerPrefs.SetString("scoresfromserver", root.score.ToString());
            //print(PlayerPrefs.GetString("scoresfromserver") + " this is getupdated score");

            if (int.Parse(PlayerPrefs.GetString("scoresfromserver")) > PlayerPrefs.GetInt("scorecount"))
            {
                PlayerPrefs.SetInt("scorecount", int.Parse(PlayerPrefs.GetString("scoresfromserver")));
            }
            if (int.Parse(PlayerPrefs.GetString("scoresfromserver")) < PlayerPrefs.GetInt("scorecount"))
            {
                PlayerPrefs.SetInt("scorecount", int.Parse(PlayerPrefs.GetString("scoresfromserver")));
            }
            SatoshiCounter.Instance.Stoshitxt2.text = PlayerPrefs.GetString("scoresfromserver");
        })
         .Catch(err => failedToGetScores(err));
    }

    public void failedToGetScores(System.Exception s)
    {
        //print("failedtogetscores");
        SatoshiCounter.Instance.Stoshitxt2.text = s.ToString();
    }

    public void GetLeaderboard()
    {
        currentRequest = new RequestHelper
        {
            Uri = basePath + "user/leaderboard",
            Method = "GET",

            Headers = new Dictionary<string, string>
            {
                { "jwt" , PlayerPrefs.GetString("idtoken") }
            },

            IgnoreHttpException = false, //Prevent to catch http exceptions
            UseHttpContinue = true,
        };

        RestClient.Get(currentRequest).Then(res =>
        {
            //print(res.Text);
            Root root = JsonUtility.FromJson<Root>(res.Text);
            leaderboardEntries = root.users.Count();
            if (leaderboardEntries == 0)
            {
                print("leaderboard is empty");
            }
            else if (leaderboardEntries >= 1)
            {
                for (int i = 0; i < root.users.Count; i++)
                {
                    SatoshiCounter.Instance.LbEntriesInTable(root.users[i].score.ToString(), root.users[i].email, i);
                }
            }
        })
            .Catch(err => Exp(err));
    }

    public void Exp(System.Exception s)
    {
        print(s);
        failed.SetActive(true);
        Invoke("off", 5);
    }

    public void off()
    {
        failed.SetActive(false);
    }

    public void GetPayouts()
    {
        currentRequest = new RequestHelper
        {
            Uri = basePath + "user/payouts",
            Method = "GET",

            Headers = new Dictionary<string, string>
            {
                { "jwt", PlayerPrefs.GetString("idtoken") }
            },

            IgnoreHttpException = true, //Prevent to catch http exceptions

            UseHttpContinue = true,
        };

        RestClient.Get(currentRequest).Then(res =>
        {
            //print("payouts is working");
            //print(res.Text);
            Payouts root = JsonUtility.FromJson<Payouts>(res.Text);
            payoutEntries = root.transactions.Count();

            if (payoutEntries == 0)
            {
                NoPayoutsText.gameObject.SetActive(true);
                //print("payouts is empty");
            }
            else if (payoutEntries >= 1)
            {
                NoPayoutsText.gameObject.SetActive(false);
                for (int i = 0; i < root.transactions.Count; i++)
                {
                    SatoshiCounter.Instance.PayoutEntriesInTable(root.transactions[i].to, root.transactions[i].amount.ToString(), root.transactions[i].market, root.transactions[i].symbol, root.transactions[i].status, i);
                }
            }
        })
         .Catch(err => ExpErr(err));
    }

    public void ExpErr(System.Exception s)
    {
        print(s);
        failed.SetActive(true);
        //loadingPanel.gameObject.SetActive(false);
        Invoke("off", 5);
    }
    public void TelegramLink()
    {
        Application.OpenURL("http://t.me/RPG007BOT");
    }
}

#region Serialized Classes

public class datacontainer : MonoBehaviour
{
    public string username;
    public string userID;
    public string Userids;

    public datacontainer()
    {
        username = GoogleSignInDemo.Instance.playerName;
        userID = GoogleSignInDemo.Instance.firebaseIdToken;
        Userids = GoogleSignInDemo.Instance.Uids;
    }
}

[Serializable]
public class IWithdrawPayment
{
    public string address;

    //public string amount;
    public int amountInINX;

    public string symbol;
    public string market;
    public string signature;
    public long timestamp;

    public override string ToString()
    {
        return UnityEngine.JsonUtility.ToJson(this, true);
    }
}

//Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
[Serializable]
public class Transaction
{
    public string _id;
    public string userId;
    public double amount;
    public string to;
    public string symbol;
    public string status;
    public string type;
    public DateTime createdAt;
    public DateTime updatedAt;
    public int __v;
}

[Serializable]
public class Root1
{
    public int code;

    public string message;

    public List<Transaction> transactions;
}

[Serializable]
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public class postLevelScores
{
    public int score;
    public string type;
    public string signature;
    public long timestamp;
    public string message;
}

public class getTotalScores
{
    public int code;
    public string message;
    public int score;
}

[Serializable]//for leaderboard start
public class Root
{
    public int code;
    public string message;
    public List<User> users;
}

[Serializable]// for leaderboard end
public class User
{
    public double score;
    public string email;
}

[Serializable]//for payouts start
public class Payouts
{
    public int code;
    public string message;
    public List<Transaction1> transactions;
}

[Serializable]//for payouts end
public class Transaction1
{
    public string _id;
    public double amount;
    public string to;
    public string symbol;
    public string status;
    public string type;
    public string market;
    public DateTime createdAt;
    public DateTime updatedAt;
    public int __v;
}

#endregion Serialized Classes

