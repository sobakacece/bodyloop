using UnityEngine;
using UnityEngine.UI;

public class RadialMenu : MonoBehaviour
{

    public Image loadingImage;
    [Range(0, 1)]
    private float progress = 0;
    private Color currentColor;

    public float ImageProgress
    {
        get => progress;
        set
        {
            progress = value;
            loadingImage.fillAmount = progress;
            if (progress < 0.1f)
                loadingImage.color = Color.Lerp(loadingImage.color, Color.red, progress * 0.5f);
            else if (progress < 0.25f)
                loadingImage.color = Color.Lerp(loadingImage.color, Color.yellow, progress * 0.51f);
            else if (progress > 0.55f)
                loadingImage.color = Color.Lerp(loadingImage.color, Color.green, progress * 0.5f);

            currentColor = loadingImage.color;
            if (progress > 0.95f)
            {
                loadingImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.0f);
            }
            else
            {
                loadingImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1.0f);

            }


        }
    }

    // Update is called once per frame
    void Update()
    {
    }
    void Start()
    {
        currentColor = loadingImage.color;
    }

}
