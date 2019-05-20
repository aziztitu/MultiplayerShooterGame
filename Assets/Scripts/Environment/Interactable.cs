using System;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public enum InteractionType
    {
        Primary,
        Secondary
    }

    public static int InteractionTypeCount = Enum.GetNames(typeof(InteractionType)).Length;

    [Serializable]
    public class InteractionEvent : UnityEvent<InteractionController>
    {
    }

    [Serializable]
    public class Interaction
    {
        public InteractionType type = InteractionType.Primary;
        public bool enabled = true;
        public bool showGizmo = true;
        public Color gizmoColor = Color.blue;
        public string description = "";
        public float maxRange = 30f;
//        public InteractableUI interactableUi = null;

        public InteractionController.SpecialActionController requiresSpecialActionController =
            InteractionController.SpecialActionController.None;

        public InteractionEvent onInteractionEvent;
    }

    [Tooltip("Can have upto 2 interactions")] [SerializeField]
    public Interaction[] interactions = new Interaction[1];

    void OnValidate()
    {
        if (interactions.Length > InteractionTypeCount)
        {
            Debug.LogWarning("You can only have at most " + InteractionTypeCount + " interactions!");
            Array.Resize(ref interactions, InteractionTypeCount);
        }
    }

    void OnDrawGizmosSelected()
    {
        foreach (Interaction interaction in interactions)
        {
            if (interaction.enabled && interaction.showGizmo)
            {
                Gizmos.color = interaction.gizmoColor;
                Gizmos.DrawWireSphere(transform.position, interaction.maxRange);
            }
        }
    }

    private void Update()
    {
        if (LevelManager.Instance && LevelManager.Instance.currentInteractionController)
        {
            float distanceToPlayer =
                Vector3.Distance(transform.position, LevelManager.Instance.currentInteractionController.transform.position);

            foreach (Interaction interaction in interactions)
            {
                if (interaction.enabled && distanceToPlayer <= interaction.maxRange)
                {
                    LevelManager.Instance.currentInteractionController.AddInteractableInProximity(interaction);
                }
            }
        }
    }

    public Interaction GetInteraction(int index)
    {
        if (index >= interactions.Length)
        {
            return null;
        }

        return interactions[index];
    }

    public void ToggleInteraction(int index, bool enable)
    {
        if (index < interactions.Length)
        {
            interactions[index].enabled = enable;
        }
    }

    /*public void ShowInteractableUI()
    {
        foreach (Interaction interaction in interactions)
        {
            if (interaction.enabled)
            {
                interaction.interactableUi.Show();
            }
        }
    }

    public void HideInteractableUI()
    {
        foreach (Interaction interaction in interactions)
        {
            if (interaction.enabled)
            {
                interaction.interactableUi.Hide();
            }
        }
    }*/
}