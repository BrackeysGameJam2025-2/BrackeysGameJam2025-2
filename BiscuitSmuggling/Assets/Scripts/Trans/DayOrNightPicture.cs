using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DayOrNightPicture : MonoBehaviour
{
    private Image _image;

    [SerializeField]
    private Sprite m_DaySprite;
    [SerializeField]
    private Sprite m_NightSprite;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        _image.sprite = TransitionManager.IsDay() ? m_DaySprite : m_NightSprite;
    }
}
