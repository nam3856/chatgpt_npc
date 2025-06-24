using TMPro;
using UnityEngine;

public class TextHeightSetter : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    public float minHeight = 50f;

    public string Text
    {
        get => textMeshProUGUI.text;
        set
        {
            textMeshProUGUI.text = value;
            SetHeight();
        }
    }

    private void Start()
    {
        if (textMeshProUGUI == null)
        {
            textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        }
        SetHeight();
    }

    private void SetHeight()
    {
        // 텍스트의 높이를 계산하고 최소 높이로 설정
        float preferredHeight = textMeshProUGUI.preferredHeight;
        textMeshProUGUI.rectTransform.sizeDelta = new Vector2(textMeshProUGUI.rectTransform.sizeDelta.x, Mathf.Max(preferredHeight, minHeight));
    }
}
