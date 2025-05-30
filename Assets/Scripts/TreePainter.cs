// using UnityEngine;
// using System.Collections.Generic;
// using UnityEditor;

// public class TreePainter : MonoBehaviour
// {
//     public GameObject[] treePrefab; // Drag your correctly scaled tree prefab here
//     public Terrain terrain; // Assign your terrain here
//     public LayerMask terrainLayer; // Assign your terrain's layer

//     [Header("Painting Settings")]
//     public float brushRadius = 5f; // How large of an area to place trees in with one click
//     public int treesPerClick = 5; // How many trees to place per click (per continuous paint action)
//     public float treeYLevel = 8.15f;
//     public float treeDensity = 0.5f;

//     [Header("Optional: Randomization")]
//     public Vector2 randomScaleRange = new Vector2(0.8f, 1.2f); // Min and Max random scale multiplier
//     public bool randomRotationY = true; // Randomize Y rotation

//     // This boolean will be controlled by the Editor script
//     [HideInInspector] // Hide this from the default inspector, our custom editor handles it
//     public bool isPaintingMode = false;

//     // List to keep track of trees placed by this tool for easy clearing
//     private List<GameObject> placedTrees = new List<GameObject>();

//     // This method will be called by the Editor script
//     public void PlaceTreesInRadius(Vector3 centerPoint)
//     {
//         if (treePrefab == null)
//         {
//             Debug.LogError("Tree Prefab is not assigned in the TreePainter script!");
//             return;
//         }

//         if (terrain == null)
//         {
//             Debug.LogError("Terrain is not assigned in the TreePainter script. Trees cannot be snapped to terrain.");
//             return;
//         }

//         for (int i = 0; i < treesPerClick; i++)
//         {
//             if (Random.value > treeDensity)
//             {
//                 continue;
//             }
//             // Calculate a random position within the brush radius
//             Vector2 randomOffset = Random.insideUnitCircle * brushRadius;
//             Vector3 spawnPosition = new Vector3(centerPoint.x + randomOffset.x, centerPoint.y, centerPoint.z + randomOffset.y);

//             // Snap to terrain height
//             float yPos = treeYLevel;
//             spawnPosition.y = yPos;

//             // Apply random rotation
//             Quaternion rotation = Quaternion.identity;
//             if (randomRotationY)
//             {
//                 rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
//             }

//             // Instantiate the tree. IMPORTANT: Use `null` as parent initially, then reparent in editor.
//             // Or parent to 'this.transform' and make sure 'this.transform' is static/not moved after generation.
//             // For editor placement, it's often better to just instantiate and then ensure
//             // it's saved correctly as part of the scene.

//             // Get a random tree prefab from the array
//             GameObject treePrefab = this.treePrefab[Random.Range(0, this.treePrefab.Length)];
//             GameObject newTree = Instantiate(treePrefab, spawnPosition, rotation);

//             // Apply random scale
//             float randomScale = Random.Range(randomScaleRange.x, randomScaleRange.y);
//             newTree.transform.localScale *= randomScale;

//             // Parent the new tree to this GameObject for organizational purposes
//             newTree.transform.SetParent(this.transform);

//             // Add to our list for clearing later
//             placedTrees.Add(newTree);

//             // Mark the created object and the scene as dirty so changes are saved
//             Undo.RegisterCreatedObjectUndo(newTree, "Place Tree"); // Allows Ctrl+Z
//             EditorUtility.SetDirty(newTree);
//         }

//         // Mark the painter script itself as dirty
//         EditorUtility.SetDirty(this);
//     }

//     // Method to clear all trees placed by this tool
//     public void ClearAllPlacedTrees()
//     {
//         // Destroy all children of this GameObject that are trees, and clear the list
//         // Iterate backwards to avoid issues when destroying elements from a list
//         for (int i = placedTrees.Count - 1; i >= 0; i--)
//         {
//             if (placedTrees[i] != null)
//             {
//                 Undo.DestroyObjectImmediate(placedTrees[i]); // Allows Ctrl+Z in editor
//             }
//         }
//         placedTrees.Clear();
//         Debug.Log("Cleared all trees placed by this painter.");

//         EditorUtility.SetDirty(this); // Mark dirty to save changes
//     }

//     // Optional: Ensure trees are cleared if the script is destroyed in editor
//     void OnDisable()
//     {
//         // If you want trees to persist even if the painter is deleted, remove this.
//         // ClearAllPlacedTrees();
//     }
// }