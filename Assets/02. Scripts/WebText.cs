using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using HtmlAgilityPack; // 여전히 필요합니다!

public class WebText : MonoBehaviour
{
    public Text MyTextUI;

    void Start()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        // 나무위키 '백지헌' 문서의 URL
        // 이 URL은 정확해야 합니다.
        UnityWebRequest www = UnityWebRequest.Get("https://namu.wiki/w/%EB%B0%B1%EC%A7%80%ED%97%8C");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("웹 요청 오류: " + www.error);
            MyTextUI.text = "콘텐츠 로드 실패: " + www.error;
        }
        else
        {
            string htmlContent = www.downloadHandler.text;
            ParseHtmlForDescription(htmlContent); // description을 파싱하는 새 메서드 호출
        }
    }

    void ParseHtmlForDescription(string htmlContent)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        // XPath를 사용하여 property="og:description" 속성을 가진 meta 태그를 찾습니다.
        // XPath: "//meta[@property='og:description']"
        HtmlNode descriptionNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:description']");

        if (descriptionNode != null)
        {
            // 찾은 meta 태그의 'content' 속성 값을 가져옵니다.
            string descriptionContent = descriptionNode.GetAttributeValue("content", "Description Not Found");
            MyTextUI.text = descriptionContent;
            Debug.Log("Description을 성공적으로 추출했습니다: " + descriptionContent);
        }
        else
        {
            Debug.LogWarning("HTML에서 'og:description' 메타 태그를 찾을 수 없습니다.");
            MyTextUI.text = "Description을 찾을 수 없습니다.";
        }
    }
}