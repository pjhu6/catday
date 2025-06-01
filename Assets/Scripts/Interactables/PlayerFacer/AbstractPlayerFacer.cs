using UnityEngine;

public abstract class AbstractPlayerFacer : MonoBehaviour
{
    public bool isFlipped = false;
    public bool isEnabled = true;

    protected void Update()
    {   
        if (!isEnabled)
        {
            return;
        }
        
        // Keep the interactable facing the player, only along the y-axis
        Vector3 lookPos = Camera.main.transform.position - transform.position;
        lookPos.y = 0;
        if (!isFlipped)
        {
            lookPos = Quaternion.Euler(0, 180, 0) * lookPos;
        }
        
        transform.rotation = Quaternion.LookRotation(lookPos);
    }
}

