#if UNITY_ANDROID || UNITY_IOS
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CheckInternetConnection : MonoBehaviour
{
    [SerializeField] public Text ConnectionErrorText;
    [SerializeField] private Button InternetCheckButton;
    public GameObject LoadingPanel;

    public static CheckInternetConnection _instance;

    public static CheckInternetConnection instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CheckInternetConnection>();
            }
            return _instance;
        }
    }

    private void Start()
    {
        StartCoroutine(CheckForInternetConnection());
    }

    private IEnumerator CheckForInternetConnection()
    {
        UnityWebRequest request = new UnityWebRequest("http://google.com");
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            InternetCheckButton.gameObject.SetActive(true);
            ConnectionErrorText.gameObject.SetActive(true);
            LoadingPanel.gameObject.SetActive(false);
        }
        else
        {
            LoadingPanel.gameObject.SetActive(false);
            InternetCheckButton.gameObject.SetActive(false);
            ConnectionErrorText.gameObject.SetActive(false);
        }
    }

    public void TryAgain()
    {
        StartCoroutine(CheckForInternetConnection());
        LoadingPanel.gameObject.SetActive(true);
        ConnectionErrorText.gameObject.SetActive(false);
    }
    
}
#endif