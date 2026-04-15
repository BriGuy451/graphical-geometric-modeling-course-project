using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugLabel : MonoBehaviour
{
    private Vector3 _anchorPosition = Vector3.zero;

    private RectTransform _rectTransform;
    private Image _image;
    private TextMeshProUGUI _textMesh;

    private GameObject _childGameObject;

    void Start()
    {
        _childGameObject = Instantiate(new GameObject(), transform);

        _image = gameObject.AddComponent<Image>();
        _textMesh = _childGameObject.AddComponent<TextMeshProUGUI>();

        _rectTransform = (RectTransform)transform;
    }

    public void SetRectTransformProperties()
    {
        _rectTransform.anchorMin = new Vector2(0f,0f);
        _rectTransform.anchorMax = new Vector2(1f,1f);
    }

    public void SetAnchorPosition(Vector3 position)
    {
        _anchorPosition = position;
    }

    public void SetImageColor(Color color)
    {
        _image.color = color;
    }

    public void SetText(string text)
    {
        _textMesh.text = text;
    }

}