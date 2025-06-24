using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
public class WebImage : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(GetTexture());
    }

    public RawImage RawTexture;
    IEnumerator GetTexture()
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture("https://image.genie.co.kr/Y/IMAGE/IMG_ARTIST/080/606/695/80606695_1656318669979_12_600x600.JPG");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            RawTexture.texture = myTexture;
        }
    }
}
