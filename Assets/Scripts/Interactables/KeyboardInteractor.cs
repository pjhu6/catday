using UnityEngine;

public class KeyboardInteractor : MonoBehaviour
{
    public float interactionDistance = 10f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            AbstractInteractable closestInteractable = null;
            float closestDistance = float.MaxValue;

            foreach (var interactable in FindObjectsOfType<AbstractInteractable>())
            {
                float distance = Vector3.Distance(Camera.main.transform.position, interactable.transform.position);
                if (distance < closestDistance && distance <= interactionDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }

            if (closestInteractable != null)
            {
                Debug.Log($"Interacting with: {closestInteractable.gameObject.name}");
                closestInteractable.Interact();
            }
        }
    }
}
