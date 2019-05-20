using TMPro;
using UnityEngine;

public class InteractionInfoUI : MonoBehaviour
{
    public InteractionInfoItemUI primaryItem;
    public InteractionInfoItemUI secondaryItem;

    private InteractionInfoItemUI[] interactionItems;

    private void Awake()
    {
        interactionItems = new[]
        {
            primaryItem,
            secondaryItem
        };
    }

    private void Start()
    {
        RefreshVisibility();
    }

    public void SetInteractionDescription(Interactable.InteractionType interactionType, string description)
    {
        switch (interactionType)
        {
            case Interactable.InteractionType.Primary:
                primaryItem.SetDescription(description);
                break;
            case Interactable.InteractionType.Secondary:
                secondaryItem.SetDescription(description);
                break;
        }
        
        RefreshVisibility();
    }

    void RefreshVisibility()
    {
        foreach (var interactionItem in interactionItems)
        {
            if (interactionItem.gameObject.activeSelf)
            {
                Show();
                return;
            }
        }
        
        Show(false);
    }

    public void Show(bool show = true)
    {
        gameObject.SetActive(show);
    }
}