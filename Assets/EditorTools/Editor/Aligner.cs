using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace NorthLab.EditorTools
{

    public class Aligner : EditorWindow
    {

        private LayerMask layerMask = default;
        private float maxDistance = 10;
        private Vector3 dir = Vector3.down;
        private Vector3 offset = Vector3.zero;
        private bool alignNormal = false;

        private GameObject[] selection;

        [MenuItem("Tools/NorthLab/Aligner")]
        public static void ShowWindow()
        {
            GetWindow(typeof(Aligner), false, "Aligner");
        }

        private void OnGUI()
        {
            selection = Selection.gameObjects;

            LayerMask tempMask = EditorGUILayout.MaskField("Layer mask: ", InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layerMask), InternalEditorUtility.layers);
            layerMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
            maxDistance = EditorGUILayout.FloatField("Max distance:", maxDistance);
            dir = EditorGUILayout.Vector3Field("Direction: ", dir);
            offset = EditorGUILayout.Vector3Field("Offset: ", offset);
            alignNormal = EditorGUILayout.Toggle("Align normal:", alignNormal);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Selected objects: " + selection.Length);
            EditorGUILayout.EndVertical();

            GUI.enabled = selection.Length > 0;

            if (GUILayout.Button("Align!"))
            {
                Align();
            }

            GUI.enabled = true;
        }

        private void Align()
        {
            EditorUtility.DisplayProgressBar("Aligning objects", "", 0);
            for (int i = 0; i < selection.Length; i++)
            {
                Ray ray = new Ray(selection[i].transform.position - dir.normalized * 0.1f, dir);
                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
                {
                    Undo.RecordObject(selection[i].transform, "Aligning");
                    selection[i].transform.position = hit.point + offset;
                    if (alignNormal)
                        selection[i].transform.up = hit.normal;
                }

                if (i % 2 == 0)
                    EditorUtility.DisplayProgressBar("Aligning objects", "", (float)i / (float)selection.Length);
            }
            EditorUtility.ClearProgressBar();
        }
    }
}