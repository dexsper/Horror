
using System;
using UnityEngine;

#if UNITY_ANDROID
using Google.Play.Review;
#endif

public class Review : MonoBehaviour
{
    public static Review Instance;

    private void Awake()
    {
        Instance = this;
    }

#if UNITY_ANDROID
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
#endif

    private void Start()
    {
#if UNITY_ANDROID
        _reviewManager = new ReviewManager();
#endif
    }


    public void OnReview()
    {
        Debug.Log("Review Show");
#if UNITY_IOS
        UnityEngine.iOS.Device.RequestStoreReview();
#endif


#if UNITY_ANDROID
                StartCoroutine(OpenReview());
#endif
    }


#if UNITY_ANDROID
    private IEnumerator OpenReview()
    {
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }
        _playReviewInfo = requestFlowOperation.GetResult();

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null;
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            yield break;
        }
    }
#endif
}
