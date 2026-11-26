
using UnityEngine;

public class FacePaintUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FacePaintBrush paintBrush;

    [Header("Brush Colors")]
    [SerializeField] private Color red = Color.red;
    [SerializeField] private Color pink = new Color(1f, 0.2f, 0.55f);
    [SerializeField] private Color purple = new Color(0.6f, 0.2f, 1f);
    [SerializeField] private Color blue = Color.blue;
    [SerializeField] private Color white = Color.white;
    [SerializeField] private Color black = Color.black;

    private void Awake()
    {
        if (paintBrush == null)
            paintBrush = FindAnyObjectByType<FacePaintBrush>();
    }

    public void SelectRed() => SelectColor(red);
    public void SelectPink() => SelectColor(pink);
    public void SelectPurple() => SelectColor(purple);
    public void SelectBlue() => SelectColor(blue);
    public void SelectWhite() => SelectColor(white);
    public void SelectBlack() => SelectColor(black);

    private void SelectColor(Color color)
    {
        if (paintBrush != null)
            paintBrush.SetColor(color);
    }

    public void SetBrushSize(float size)
    {
        if (paintBrush != null)
            paintBrush.SetBrushSize(Mathf.RoundToInt(size));
    }

    public void ClearPaint()
    {
        if (paintBrush != null)
            paintBrush.Clear();
    }
}
