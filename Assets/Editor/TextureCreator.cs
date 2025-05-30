using UnityEngine;
using UnityEditor;

public class TextureCreator
{
    [MenuItem("Assets/Create Quad From Texture (URP Unlit)")]
    public static void CreateQuadFromTexture()
    {
        Texture2D tex = Selection.activeObject as Texture2D;
        if (tex == null)
        {
            Debug.LogWarning("Please select a Texture2D asset.");
            return;
        }

        // Create Quad
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.name = tex.name + "_Quad";

        // Create material with URP/Unlit shader
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
        {
            Debug.LogError("URP Unlit shader not found! Make sure URP is installed and active.");
            return;
        }

        Material mat = new Material(shader);

        // Set texture to Base Map
        mat.SetTexture("_BaseMap", tex);

        // Set surface to Opaque and enable Alpha Clipping
        mat.SetFloat("_Surface", 0f); // Opaque
        mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");

        mat.SetFloat("_AlphaClip", 1f); // Enable alpha clipping
        mat.EnableKeyword("_ALPHATEST_ON");

        mat.SetFloat("_Cutoff", 0.5f); // Clip threshold

        // Enable double-sided rendering
        mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        mat.EnableKeyword("_DOUBLESIDED_ON");

        // Assign material to quad
        Renderer renderer = quad.GetComponent<Renderer>();
        renderer.sharedMaterial = mat;

        // Scale quad based on texture aspect ratio (fixed height = 1)
        float aspect = (float)tex.width / tex.height;
        quad.transform.localScale = new Vector3(aspect, 1f, 1f);

        // Position the quad in front of the Scene view camera
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null)
        {
            Camera sceneCam = sceneView.camera;
            Vector3 camPos = sceneCam.transform.position;
            Vector3 forward = sceneCam.transform.forward;

            quad.transform.position = camPos + forward * 2.5f;
        }
        else
        {
            quad.transform.position = Vector3.zero;
        }

        // Select the new quad in Hierarchy
        Selection.activeGameObject = quad;

        Debug.Log("Created quad with URP Unlit Opaque + Alpha Clip + Double-Sided material from texture: " + tex.name);
    }
}
