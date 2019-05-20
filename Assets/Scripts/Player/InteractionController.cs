using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
    public static int MaxInteractions => Interactable.InteractionTypeCount;

    [Serializable]
    public enum SpecialActionController
    {
        None,
        Player,
        Flight
    }

    public float InteractionRadius = 0.5f;
    public float InteractionAngle = 30f;
    public float HighlightRadius = 10f;

    public InteractionInfoUI interactionInfoUi;
    public SpecialActionController SpecialActionControllerTag = SpecialActionController.None;

    private readonly Dictionary<Interactable.InteractionType, string> InteractionInputButtons =
        new Dictionary<Interactable.InteractionType, string>()
        {
            {Interactable.InteractionType.Primary, "Primary Interaction"},
            {Interactable.InteractionType.Secondary, "Secondary Interaction"},
        };

    [SerializeField] private float _maxDistance = 100f;

    private bool[] _interactionInputs = new bool[MaxInteractions];
//    private InteractableUI[] _activeInteractableUIs = new InteractableUI[MaxInteractions];

    private Stack<Outline> _activeOutlines = new Stack<Outline>();

    private List<Interactable.Interaction> _interactionsInProximity = new List<Interactable.Interaction>();

    public InteractionController()
    {
    }

    // Use this for initialization
    void Start()
    {
    }

    void LateUpdate()
    {
//        CheckHighlights();


//        InteractableUI[] interactableUIsToActivate;

        DetectInteractionInputs();
//        CheckInteraction(out interactableUIsToActivate);
        CheckInteractionUsingProximity();

        /*for (int i = 0; i < interactableUIsToActivate.Length; i++)
        {
            if (interactableUIsToActivate[i] != _activeInteractableUIs[i])
            {
                if (_activeInteractableUIs[i] != null)
                {
                    _activeInteractableUIs[i].Hide();
                    _activeInteractableUIs[i] = null;
                }

                if (interactableUIsToActivate[i] != null)
                {
                    interactableUIsToActivate[i].Show();
                    _activeInteractableUIs[i] = interactableUIsToActivate[i];
                }
            }
        }*/

        _interactionsInProximity.Clear();
    }

    private void DetectInteractionInputs()
    {
        for (int i = 0; i < MaxInteractions; i++)
        {
            _interactionInputs[i] = false;
        }

        foreach (Interactable.InteractionType interactionType in Enum.GetValues(typeof(Interactable.InteractionType)))
        {
            if (Input.GetButtonDown(InteractionInputButtons[interactionType]))
            {
                _interactionInputs[(int) interactionType] = true;
                return; // Ensures that at most only one interaction input is ever true
            }
        }
    }

    void CheckInteractionUsingProximity()
    {
        string[] interactionDescriptions = new string[MaxInteractions];
        for (int i = 0; i < MaxInteractions; i++)
        {
            interactionDescriptions[i] = null;
        }

        bool showInteractionInfoUI = false;
        foreach (Interactable.Interaction interaction in _interactionsInProximity)
        {
            if (interaction.enabled && interactionDescriptions[(int) interaction.type] == null &&
                (interaction.requiresSpecialActionController == SpecialActionController.None ||
                 interaction.requiresSpecialActionController == SpecialActionControllerTag))
            {
                interactionDescriptions[(int) interaction.type] = interaction.description;
                showInteractionInfoUI = true;

                if (_interactionInputs[(int) interaction.type])
                {
                    interaction.onInteractionEvent.Invoke(this);
                }
            }
        }

        for (int i = 0; i < MaxInteractions; i++)
        {
            if (interactionDescriptions[i] == null)
            {
                interactionDescriptions[i] = "";
            }

            interactionInfoUi.SetInteractionDescription((Interactable.InteractionType) i, interactionDescriptions[i]);
        }
    }

    private void CheckInteraction(out InteractableUI[] interactableUIsToActivate)
    {
        int layerMask = -5; //All layers

        layerMask = layerMask & ~(1 << LayerMask.NameToLayer("Curiosity"));

        Debug.Log(Convert.ToString(~(1 << LayerMask.NameToLayer("Curiosity")), 2));
        Debug.Log(Convert.ToString(layerMask, 2));

        RaycastHit raycastHit;

        Camera camera = Camera.main;

        bool hitDetected = Physics.SphereCast(camera.transform.position, InteractionRadius,
            camera.transform.forward,
            out raycastHit,
            _maxDistance,
            layerMask, QueryTriggerInteraction.Ignore);

        interactableUIsToActivate = new InteractableUI[MaxInteractions];

        if (hitDetected)
        {
            if (raycastHit.distance > 0)
            {
                Debug.Log("Hit Detected: " + raycastHit.transform.gameObject);
                Interactable interactiveObject = raycastHit.transform.GetComponent<Interactable>();
                if (interactiveObject == null)
                {
                    interactiveObject = raycastHit.transform.GetComponentInParent<Interactable>();
                }

                if (interactiveObject != null && interactiveObject.enabled)
                {
                    int interactionCount = Mathf.Min(MaxInteractions, interactiveObject.interactions.Length);

                    for (int i = 0; i < interactionCount; i++)
                    {
                        Interactable.Interaction interaction = interactiveObject.interactions[i];

                        if (interaction.enabled &&
                            (interaction.requiresSpecialActionController == SpecialActionController.None ||
                             interaction.requiresSpecialActionController == SpecialActionControllerTag) &&
                            Vector3.Distance(transform.position, interactiveObject.transform.position) <=
                            interaction.maxRange)
                        {
                            if (interactableUIsToActivate[(int) interaction.type] == null)
                            {
//                                interactableUIsToActivate[(int) interaction.type] = interaction.interactableUi;
                            }

                            if (_interactionInputs[(int) interaction.type])
                            {
                                interaction.onInteractionEvent.Invoke(this);
                            }
                        }
                    }
                }
            }
        }
    }

    void CheckHighlights()
    {
        foreach (Outline outline in _activeOutlines)
        {
            outline.enabled = false;
        }

        _activeOutlines.Clear();


        int layerMask = -5; //All layers

        Camera camera = Camera.main;

        RaycastHit[] raycastHits = Physics.SphereCastAll(camera.transform.position, HighlightRadius,
            camera.transform.forward,
            _maxDistance,
            layerMask, QueryTriggerInteraction.Ignore);

        foreach (RaycastHit raycastHit in raycastHits)
        {
            Outline[] outlines = raycastHit.collider.gameObject.GetComponentsInChildren<Outline>();

            if (outlines.Length > 0)
            {
//                Debug.Log("Pointing at: " + raycastHit.transform.gameObject);
                Interactable interactable = raycastHit.transform.GetComponent<Interactable>();
                if (interactable != null && interactable.enabled)
                {
                    int interactionCount = Mathf.Min(MaxInteractions, interactable.interactions.Length);

                    bool highlightable = false;
                    for (int i = 0; i < interactionCount; i++)
                    {
                        Interactable.Interaction interaction = interactable.interactions[i];

                        if (interaction.enabled &&
                            Vector3.Distance(transform.position, interactable.transform.position) <=
                            interaction.maxRange)
                        {
                            highlightable = true;
                            break;
                        }
                    }

                    if (highlightable)
                    {
                        foreach (Outline outline in outlines)
                        {
                            outline.enabled = true;
                            _activeOutlines.Push(outline);
                        }
                    }
                }
            }
        }
    }

    public void AddInteractableInProximity(Interactable.Interaction interaction)
    {
        _interactionsInProximity.Add(interaction);
    }
}