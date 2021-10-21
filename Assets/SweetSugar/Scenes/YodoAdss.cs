/*using UnityEngine;
using Yodo1.MAS;

public class YodoAdss : MonoBehaviour
{
    public static YodoAdss yodo;

    public static YodoAdss _instance;

    public static YodoAdss instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<YodoAdss>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (yodo == null)
        {
            DontDestroyOnLoad(gameObject);
            yodo = this;
        }
        else if (yodo != this)
        {
            Destroy(gameObject);
        }
        Yodo1U3dMas.SetGDPR(true);
        Yodo1U3dMas.SetCOPPA(true);
    }

    private void Start()
    {
        Yodo1U3dMas.InitializeSdk();
        SetInterstitialAdsCallBack();
        SetRewardedVideoAdsCallBack();
        SetBannerAdsCallBack();
    }

    private void SetInterstitialAdsCallBack()
    {
        Yodo1U3dMas.SetInterstitialAdDelegate((Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error) =>
        {
            Debug.Log("[Yodo1 Mas] InterstitialAdDelegate:" + adEvent.ToString() + "\n" + error.ToString());
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdClosed:
                    Debug.Log("[Yodo1 Mas] Interstital ad has been closed.");
                    break;

                case Yodo1U3dAdEvent.AdOpened:
                    Debug.Log("[Yodo1 Mas] Interstital ad has been shown.");
                    break;

                case Yodo1U3dAdEvent.AdError:
                    Debug.Log("[Yodo1 Mas] Interstital ad error, " + error.ToString());
                    break;
            }
        });
    }

    public void ShowInterstitialAds()
    {
        if (Yodo1U3dMas.IsInterstitialAdLoaded())
        {
            Yodo1U3dMas.ShowInterstitialAd();
            loadingScript.instance.turnOnLoading();
        }
        else
        {
            Debug.Log("intertitial ad has not been cached");
        }
    }

    private void SetRewardedVideoAdsCallBack()
    {
        Yodo1U3dMas.SetRewardedAdDelegate((Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error) =>
        {
            Debug.Log("[Yodo1 Mas] RewardVideoDelegate:" + adEvent.ToString() + "\n" + error.ToString());
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdClosed:
                    Debug.Log("[Yodo1 Mas] Reward video ad has been closed.");
                    break;

                case Yodo1U3dAdEvent.AdOpened:
                    Debug.Log("[Yodo1 Mas] Reward video ad has shown successful.");
                    break;

                case Yodo1U3dAdEvent.AdError:
                    Debug.Log("[Yodo1 Mas] Reward video ad error, " + error);
                    break;

                case Yodo1U3dAdEvent.AdReward:
                    Debug.Log("[Yodo1 Mas] Reward video ad reward, give rewards to the player.");
                    break;
            }
        });
    }

    public void ShowRewardedVideoAds()
    {
        if (Yodo1U3dMas.IsRewardedAdLoaded())
        {
            Yodo1U3dMas.ShowRewardedAd();
        }
        else
        {
            Debug.Log("rewarded ad has not been cached");
        }
    }

    public void SetBannerAdsCallBack()
    {
        Yodo1U3dMas.SetBannerAdDelegate((Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error) =>
        {
            Debug.Log("[Yodo1 Mas] BannerdDelegate:" + adEvent.ToString() + "\n" + error.ToString());
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdClosed:
                    Debug.Log("[Yodo1 Mas] Banner ad has been closed.");
                    break;

                case Yodo1U3dAdEvent.AdOpened:
                    Debug.Log("[Yodo1 Mas] Banner ad has been shown.");
                    break;

                case Yodo1U3dAdEvent.AdError:
                    Debug.Log("[Yodo1 Mas] Banner ad error, " + error.ToString());
                    break;
            }
        });
    }

    public void ShowBanner()
    {
        if (Yodo1U3dMas.IsBannerAdLoaded())
        {
            int align = Yodo1U3dBannerAlign.BannerBottom | Yodo1U3dBannerAlign.BannerHorizontalCenter;
            Yodo1U3dMas.ShowBannerAd(align);
        }
        else
        {
            Debug.Log("banner ad has not been cached");
        }
    }

    public void HideBanner()
    {
        if (Yodo1U3dMas.IsBannerAdLoaded())
        {
            Yodo1U3dMas.DismissBannerAd();
        }
    }
}
*/