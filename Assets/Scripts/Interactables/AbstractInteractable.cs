using UnityEngine;

public abstract class AbstractInteractable : AbstractPlayerFacer
{
    public abstract void Click();
    public abstract void Interact();

    // void Update()
    // {   
    //     // Keep the interactable facing the player, only along the y-axis
    //     Vector3 lookPos = Camera.main.transform.position - transform.position;
    //     lookPos.y = 0;
    //     lookPos = Quaternion.Euler(0, 180, 0) * lookPos;
    //     transform.rotation = Quaternion.LookRotation(lookPos);
    // }
}

