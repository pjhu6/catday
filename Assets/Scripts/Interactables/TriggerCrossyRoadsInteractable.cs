using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerCrossyRoadsInteractabke : AbstractInteractable
{
    public override void Click()
    {
        Debug.Log("TriggerCrossyRoadsInteractable clicked");
    }

    public override void Interact()
    {
        Debug.Log("Switching to Crossy Roads scene");
        SceneManager.LoadScene("CrossyRoads");
    }

    public override bool IsInteractable()
    {
        // Only if bird mission is completed
        return MissionManager.Instance.IsMissionCompleted("play_bird");
    }
}
