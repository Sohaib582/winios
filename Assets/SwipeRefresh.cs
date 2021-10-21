using UnityEngine;

public class SwipeRefresh : MonoBehaviour
{
    //public Text outputtext;

    public GameObject LevelMapPanel;
    public GameObject CheckOutPanel;
    private Vector2 starttouchposition;
    private Vector2 currentposition;
    private Vector2 endtouchposition;
    private bool stoptouch = false;

    public float swipeRange;
    public float tapRange;

    private void Update()
    {
        Swipe();
    }

    public void Swipe()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            starttouchposition = Input.GetTouch(0).position;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            currentposition = Input.GetTouch(0).position;
            Vector2 Distance = currentposition - starttouchposition;

            if (!stoptouch)

            {
                /*if(Distance.x < -swipeRange)
                {
                    outputtext.text="left";
                    stoptouch = true;
                    print("left");
                }
                else if (Distance.x > swipeRange)
                {
                    outputtext.text = "right";
                    stoptouch = true;
                    print("right");
                }
                else if (Distance.y > swipeRange)
                {
                    outputtext.text = "up";
                    stoptouch = true;
                    print("up");
                }*/
                if (Distance.y < -swipeRange)
                {
                    //outputtext.text = "down";
                    stoptouch = true;
                    LevelMapPanel.gameObject.SetActive(false);
                    CheckOutPanel.gameObject.GetComponent<Animation>().enabled = true;
                    Invoke("reactivate", 2f);
                }
            }
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            stoptouch = false;
            endtouchposition = Input.GetTouch(0).position;

            Vector2 Distance
                 = endtouchposition - starttouchposition;
            /*if(Mathf.Abs(Distance.x) < tapRange && Mathf.Abs(Distance.y)<tapRange)
            {
                //outputtext.text = "tap";
                //print("tap");
            }*/
        }
    }

    public void reactivate()
    {
        LevelMapPanel.gameObject.SetActive(true);
        CheckOutPanel.gameObject.GetComponent<Animation>().enabled = false;

    }
}