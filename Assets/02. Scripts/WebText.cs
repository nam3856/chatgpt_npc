using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using HtmlAgilityPack; // ������ �ʿ��մϴ�!

public class WebText : MonoBehaviour
{
    public Text MyTextUI;

    void Start()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {
        // ������Ű '������' ������ URL
        // �� URL�� ��Ȯ�ؾ� �մϴ�.
        UnityWebRequest www = UnityWebRequest.Get("https://namu.wiki/w/%EB%B0%B1%EC%A7%80%ED%97%8C");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("�� ��û ����: " + www.error);
            MyTextUI.text = "������ �ε� ����: " + www.error;
        }
        else
        {
            string htmlContent = www.downloadHandler.text;
            ParseHtmlForDescription(htmlContent); // description�� �Ľ��ϴ� �� �޼��� ȣ��
        }
    }

    void ParseHtmlForDescription(string htmlContent)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        // XPath�� ����Ͽ� property="og:description" �Ӽ��� ���� meta �±׸� ã���ϴ�.
        // XPath: "//meta[@property='og:description']"
        HtmlNode descriptionNode = doc.DocumentNode.SelectSingleNode("//meta[@property='og:description']");

        if (descriptionNode != null)
        {
            // ã�� meta �±��� 'content' �Ӽ� ���� �����ɴϴ�.
            string descriptionContent = descriptionNode.GetAttributeValue("content", "Description Not Found");
            MyTextUI.text = descriptionContent;
            Debug.Log("Description�� ���������� �����߽��ϴ�: " + descriptionContent);
        }
        else
        {
            Debug.LogWarning("HTML���� 'og:description' ��Ÿ �±׸� ã�� �� �����ϴ�.");
            MyTextUI.text = "Description�� ã�� �� �����ϴ�.";
        }
    }
}