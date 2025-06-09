using UnityEngine;

public class FenceCombiner : MonoBehaviour
{
    void Start()
    {
        CombineFences();
    }

    void CombineFences()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false); // Disable individual fence
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        GameObject combined = new GameObject("CombinedFence");
        combined.transform.parent = this.transform;
        var mf = combined.AddComponent<MeshFilter>();
        mf.mesh = combinedMesh;

        var mr = combined.AddComponent<MeshRenderer>();
        mr.material = meshFilters[0].GetComponent<MeshRenderer>().sharedMaterial;

        combined.SetActive(true);
    }
}
