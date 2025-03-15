using UnityEngine;
using UnityEngine.UI;

public class GaugeColor : MonoBehaviour
{
    void Update()
    {
        Image image = GetComponent<Image>();

        //Hue (����), Saturation(ä��), Value(��), Transparent(����)
        image.color = Color.HSVToRGB(image.fillAmount / 3, 1.0f, 1.0f);
    }
}
