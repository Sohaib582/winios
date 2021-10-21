using System.Collections;
using UnityEngine;

public class TurnOnToasterScript : MonoBehaviour
{
    public GameObject Toaster;
    //public Button Signinbutton;

    private IEnumerator turnon()
    {
        yield return new WaitForSeconds(4.5f);
        Toaster.gameObject.SetActive(false);
    }

    public void turnontoaster()
    {
        Toaster.gameObject.SetActive(true);
        StartCoroutine("turnon");
    }

    // Update is called once per frame
    private void Update()
    {
        //Signinbutton.GetComponent<Button>().onClick.AddListener(GoogleSignInDemo.Instance.OnSignIn);
    }
}