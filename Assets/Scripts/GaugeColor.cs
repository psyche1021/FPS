using UnityEngine;
using UnityEngine.UI;

public class GaugeColor : MonoBehaviour
{
    void Update()
    {
        Image image = GetComponent<Image>();

        //Hue (색상), Saturation(채도), Value(명도), Transparent(투명도)
        image.color = Color.HSVToRGB(image.fillAmount / 3, 1.0f, 1.0f);
    }
}
