using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{

    public Image loadingImage;
    [SerializeField]
    private Image crossHair;
    [Range(0, 1)]
    private float progress = 0;
    private Color currentColor;

    [SerializeField]
    private Texture2D point;
    [SerializeField]
    private Texture2D cross;

    [SerializeField]
    public Text hintText;

    public float ImageProgress
    {
        get => progress;
        set
        {
            progress = value;
            loadingImage.fillAmount = progress;
            //Debug.Log(progress);
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

    public void UpdateCrossHair(bool couldClimb)
    {
        Texture2D texture = couldClimb ? cross : point;
        crossHair.sprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

}
