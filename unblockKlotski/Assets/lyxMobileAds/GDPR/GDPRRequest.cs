using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using sw.util;
#if GOOGLEPLAY || UNITY_IOS
using GoogleMobileAds.Ump.Api;
#endif



public class GDPRRequest : MonoBehaviour
{
    private Action<string> gdprCallback;
    

    public static GDPRRequest instance;

    //单例模式
    public static GDPRRequest Instance
    {
        get
        {
            if (null != instance)
            {
                return instance;
            }
            GDPRRequest[] _monoComponents = FindObjectsOfType(typeof(GDPRRequest)) as GDPRRequest[];
            instance = _monoComponents[0];
            return instance;
        }

    }

#if GOOGLEPLAY || UNITY_IOS

    public void startCheck(Action<string> p_gdprCallback)
    {

       
        gdprCallback = p_gdprCallback;
        // Set tag for under age of consent.
        // Here false means users are not under age of consent.
        ConsentRequestParameters request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
           
        };

        if (iOSUtil.IsDebugTest == 1 || AndroidUtil.IsDebugTest==1)
        {
            var debugSettings = new ConsentDebugSettings
            {
                // Geography appears as in EEA for debug devices.
                DebugGeography = DebugGeography.EEA,

                TestDeviceHashedIds =
                new List<string>
                {
                    "EE42F09E-6913-4C18-9C59-6E05478FC131",
                    "94B5C3CB-D9A8-4374-8DCD-6F656FCDF866",
                    "B313404B-E05C-4A56-8CE3-56146327AB9A"
                }

               
            };

            debugSettings.TestDeviceHashedIds.Add(iOSUtil.umpDeviceIdentifiers());
            request.ConsentDebugSettings = debugSettings; //测试，正式发布要去掉
        }


        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);

    }



    void OnConsentInfoUpdated(FormError consentError)
    {
        if (consentError != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(consentError);
            gdprCallback(consentError.Message);
            return;
        }


        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
        {
            if (formError != null)
            {
                // Consent gathering failed.
                UnityEngine.Debug.LogError(formError);
                gdprCallback(formError.Message);
                return;
            }

            // Consent has been gathered.
            gdprCallback("gathered");
        });
    }
#endif
}
