using UnityEngine;

public class ClickInteractor : MonoBehaviour
{
    public float interactionDistance = 10f; // Minimum distance to interact

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent<AbstractInteractable>(out var interactable))
                {
                    float distance = Vector3.Distance(Camera.main.transform.position, hit.collider.transform.position);
                    // Set min distance to interact
                    if (distance <= interactionDistance)
                    {
                        Debug.Log($"Interacting with: {hit.collider.gameObject.name}");
                        interactable.Click();
                    }
                    else
                    {
                        Debug.Log($"Too far to interact with {hit.collider.gameObject.name}. Distance: {distance:F2}");
                    }
                }
            }
        }
    }
}
