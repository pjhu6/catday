using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    public static InteractableManager Instance { get; private set; }

    public GameObject pointerPrefab;
    public float pointerSpacing = 0.5f;
    public float interactionDistance = 5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (DialogueManager.Instance.InDialogue() || DialogueManager.Instance.IsInCutscene())
        {
            if (pointerPrefab.activeSelf)
            {
                pointerPrefab.SetActive(false);
            }
            return;
        }

        AbstractInteractable closestInteractable = null;
        float closestDistance = float.MaxValue;

        foreach (var interactable in FindObjectsOfType<AbstractInteractable>())
        {
            float distance = Vector3.Distance(Camera.main.transform.position, interactable.transform.position);
            if (distance < closestDistance
                && distance <= interactionDistance
                && interactable.IsInteractable())
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }

        if (closestInteractable != null)
        {
            // If player is in range, move the pointerPrefab above the interactable
            Vector3 topOfQuad = GetTopOfQuad(closestInteractable.gameObject);

            Vector3 pointerPosition = topOfQuad + Vector3.up * pointerSpacing;
            pointerPrefab.transform.position = pointerPosition;

            // Set the pointerPlayerFacer's new position so it can animate correctly
            pointerPrefab.TryGetComponent(out PointerPlayerFacer pointerPlayerFacer);
            if (pointerPlayerFacer != null)
            {
                pointerPlayerFacer.SetBasePosition(pointerPosition);
            }
            pointerPrefab.SetActive(true);

            // Check for interaction input
            CheckForKeyboardInput(closestInteractable);

            // Check for click input
            CheckForClick();
        }
        else
        {
            pointerPrefab.SetActive(false);
        }
    }

    Vector3 GetTopOfQuad(GameObject quad)
    {
        Transform t = quad.transform;
        float halfHeight = 0.5f * t.localScale.y;
        Vector3 localUp = t.up;

        // World position of the top edge
        Debug.DrawLine(t.position, t.position + localUp * halfHeight, Color.red, 2f);
        return t.position + localUp * halfHeight;
    }

    void CheckForClick()
    {
        // Check for click input
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent<AbstractInteractable>(out var interactable))
                {
                    interactable.Click();
                }
            }
        }
    }
    
    void CheckForKeyboardInput(AbstractInteractable closestInteractable)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            closestInteractable.Interact();
        }
    }
}
