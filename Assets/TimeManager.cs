using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

[Serializable]
public class TimeInfo
{
    public string time;
}

public class TimeManager : MonoBehaviour
{
    [Header("Geonames Info")]
    [Tooltip("Your Geonames Username")]
    public string gnUserName;
    [Tooltip("The latitude of the place your wish to use as time reference")]
    public string latitude;
    [Tooltip("The latitude of the place your wish to use as time reference")]
    public string longitude;

    [Header("Expiry Date")]
    [Tooltip("Format : 2021-08-28 18:40")]
    public string expiryDateString;
    DateTime expiryDate;

    [Header("UI")]
    public Text UIText;
    public string initialMessage;
    public string successMessage;
    public string failureMessage;

    // Start is called before the first frame update
    void Start()
    {
        UIText.text = initialMessage;
        try
        {
            expiryDate = DateTime.Parse(expiryDateString, System.Globalization.CultureInfo.InvariantCulture);
        }
        catch(Exception e)
        {
            UIText.text = "Please enter a valid expiry date";
            Application.Quit();
        }
        
        StartCoroutine(GetCurrentDate(ProcessCurrentTime));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ProcessCurrentTime(TimeInfo info)
    {
        Debug.Log(info.time);
        DateTime date = DateTime.Parse(info.time, System.Globalization.CultureInfo.InvariantCulture);
        if(date < expiryDate)
        {
            UIText.text = successMessage;
            // Add Action to take if the license is valid (ie. start software)
        }
        else
        {
            UIText.text = failureMessage;
        }

    }


    IEnumerator GetCurrentDate(Action<TimeInfo> onSuccess)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(String.Format("http://api.geonames.org/timezoneJSON?formatted=true&lat={1}&lng={2}&username={0}&style=full", gnUserName, latitude, longitude)))
        {
            yield return req.SendWebRequest();
            while (!req.isDone)
                yield return null;
            byte[] result = req.downloadHandler.data;
            string timeJSON = System.Text.Encoding.Default.GetString(result);
            TimeInfo info = JsonUtility.FromJson<TimeInfo>(timeJSON);
            onSuccess(info);
        }
    }
}
