using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestMgr : MonoBehaviour
{

    public void request(string url, Action<string> callback)
    {
        this.StartCoroutine(this.RequestRoutine(url, callback));
    }

    private IEnumerator RequestRoutine(string url, Action<string> callback = null)
    {
        var request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        var data = request.downloadHandler.text;

        if (callback != null)
            callback(data);
    }
}