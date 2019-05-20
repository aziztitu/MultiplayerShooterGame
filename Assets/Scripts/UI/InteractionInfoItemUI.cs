using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionInfoItemUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI description;

    private void Awake()
    {
        RefreshVisibility();
    }

    public void SetDescription(string desc)
    {
        description.text = desc;
        RefreshVisibility();
    }

    void RefreshVisibility()
    {
        Show(description.text.Trim().Length > 0);
    }

    public void Show(bool show = true)
    {
        gameObject.SetActive(show);
    }
}