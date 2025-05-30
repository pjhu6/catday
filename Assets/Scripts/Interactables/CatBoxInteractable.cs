using UnityEngine;

public class CatBoxInteractable : AbstractInteractable
{
    public Texture2D catOutTexture;  // Use Texture2D instead of Sprite

    private MeshRenderer meshRenderer;
    private Material materialInstance;
    private DialogueObject dialogueObject;

    void Start()
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        if (meshRenderer != null && materialInstance == null)
        {
            // Instantiate a unique material instance so we don't overwrite shared materials
            materialInstance = meshRenderer.material;
        }
        if (dialogueObject == null)
        {
            dialogueObject = GetComponent<DialogueObject>();
        }
    }

    public override void Click()
    {
        Debug.Log("Clicked cat box - not implemented");
    }

    public override void Interact()
    {
        if (materialInstance != null && catOutTexture != null)
        {
            // Assign the texture to the material's base map (_BaseMap or _MainTex)
            materialInstance.SetTexture("_BaseMap", catOutTexture);
            // If your shader uses _MainTex instead, use:
            // materialInstance.SetTexture("_MainTex", catOutTexture);
        }
        else
        {
            Debug.LogWarning("Material instance or catOutTexture is missing!");
        }

        dialogueObject.TriggerDialogue();
    }

    public string[] GetDialogues()
    {
        throw new System.NotImplementedException();
    }
}
