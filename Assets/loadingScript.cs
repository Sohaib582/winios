using System.Collections;
using UnityEngine;

public class loadingScript : MonoBehaviour
{
    public GameObject loadingPanel;

    public static loadingScript _instance;

    public static loadingScript instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<loadingScript>();
            }
            return _instance;
        }
    }

    public void turnOnLoading()
    {
        loadingPanel.gameObject.SetActive(true);
        StartCoroutine(turnLoadingOff());
    }

    private IEnumerator turnLoadingOff()
    {
        yield return new WaitForSeconds(3f);
        loadingPanel.gameObject.SetActive(false);
    }
}