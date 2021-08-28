using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;


[Serializable]
public class TimeInfo
{
    public string time;
}

public class TimeManager : MonoBehaviour
{
    public string gnUserName;
    public string latitude;
    public string longitude;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetCurrentDate(ProcessCurrentTime));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ProcessCurrentTime(TimeInfo info)
    {
        Debug.Log(info.time);
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
