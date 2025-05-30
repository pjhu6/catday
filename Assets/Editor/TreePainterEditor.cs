// using UnityEngine;
// using UnityEditor; // This namespace is essential for Editor scripting

// // This attribute links our custom editor to the TreePainter component
// [CustomEditor(typeof(TreePainter))]
// public class TreePainterEditor : Editor
// {
//     // Reference to the target script (TreePainter)
//     TreePainter treePainter;

//     // Called when the Inspector for TreePainter is drawn or script is selected
//     void OnEnable()
//     {
//         treePainter = (TreePainter)target; // Get the instance of our TreePainter script
//     }

//     // This method overrides the default Inspector GUI
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector(); // Draw all the default public variables

//         EditorGUILayout.Space(); // Add some space

//         // Button to enable/disable painting mode
//         if (treePainter.isPaintingMode)
//         {
//             if (GUILayout.Button("Disable Tree Painting Mode"))
//             {
//                 treePainter.isPaintingMode = false;
//                 SceneView.RepaintAll(); // Refresh Scene View to remove highlight
//             }
//         }
//         else
//         {
//             if (GUILayout.Button("Enable Tree Painting Mode"))
//             {
//                 treePainter.isPaintingMode = true;
//                 SceneView.RepaintAll(); // Refresh Scene View
//             }
//         }

//         // Button to clear all placed trees
//         if (GUILayout.Button("Clear All Placed Trees (Editor)"))
//         {
//             // Display a confirmation dialog
//             if (EditorUtility.DisplayDialog("Clear Trees",
//                                             "Are you sure you want to clear ALL trees placed by this painter in the editor?",
//                                             "Yes", "No"))
//             {
//                 treePainter.ClearAllPlacedTrees();
//             }
//         }
//     }

//     // This method is called by the editor whenever the Scene View is updated
//     void OnSceneGUI()
//     {
//         if (treePainter.isPaintingMode)
//         {
//             // Ensure the main camera is available
//             if (Camera.current == null) return;

//             // Create a ray from the mouse position in the Scene View
//             Event guiEvent = Event.current;
//             Vector2 mousePos = guiEvent.mousePosition;
//             Ray ray = HandleUtility.GUIPointToWorldRay(mousePos); // Ray from mouse in scene view

//             RaycastHit hit;

//             // Ensure the terrain exists and has a collider and is on the correct layer
//             if (treePainter.terrain == null)
//             {
//                 Debug.LogWarning("Assign your Terrain to the TreePainter script for snapping!");
//                 return;
//             }
//             if (treePainter.terrainLayer.value == 0) // Check if any layer is set
//             {
//                  Debug.LogWarning("Assign a Terrain Layer to the TreePainter script for accurate raycasting!");
//                  return;
//             }

//             // Raycast to hit the terrain
//             if (Physics.Raycast(ray, out hit, Mathf.Infinity, treePainter.terrainLayer))
//             {
//                 // Draw a visual indicator (sphere) where the mouse is pointing
//                 Handles.color = Color.green;
//                 Handles.DrawWireDisc(hit.point, Vector3.up, treePainter.brushRadius);
//                 Handles.DrawSolidDisc(hit.point, Vector3.up, treePainter.brushRadius * 0.1f);


//                 // Allow mouse dragging to paint multiple trees
//                 if (guiEvent.type == EventType.MouseDrag || guiEvent.type == EventType.MouseDown)
//                 {
//                     if (guiEvent.button == 0) // Left mouse button
//                     {
//                         // Prevent default selection behavior
//                         guiEvent.Use();
//                         treePainter.PlaceTreesInRadius(hit.point);
//                     }
//                 }
//             }
//             else
//             {
//                 // If not hitting terrain, maybe draw a general indicator
//                 Handles.color = Color.red;
//                 Handles.Label(ray.origin + ray.direction * 10, "Click not on Terrain");
//             }

//             // Mark the scene view as dirty to ensure updates are saved
//             if (GUI.changed)
//             {
//                 EditorUtility.SetDirty(treePainter);
//             }
//         }
//     }
// }