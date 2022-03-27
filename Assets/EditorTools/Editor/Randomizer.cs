using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;

namespace NorthLab.EditorTools
{
    public class Randomizer : EditorWindow
    {

        private GameObject[] selection;
        private bool randomPosition;
        private Vector2 xPos;
        private Vector2 yPos;
        private Vector2 zPos;
        private bool randomRotation;
        private Vector2 xRot;
        private Vector2 yRot;
        private Vector2 zRot;
        private bool randomScale;
        private float minScale = 0.8f;
        private float maxScale = 1;

        [MenuItem("Tools/NorthLab/Randomizer")]
        public static void ShowWindow()
        {
            GetWindow(typeof(Randomizer), false, "Randomizer");
        }

        private void OnGUI()
        {
            selection = Selection.gameObjects;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Selected objects: " + selection.Length);

            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            randomPosition = EditorGUILayout.Toggle("Random position: ", randomPosition);
            if (randomPosition)
            {
                xPos = EditorGUILayout.Vector2Field("X Pos range: ", xPos);
                yPos = EditorGUILayout.Vector2Field("Y Pos range: ", yPos);
                zPos = EditorGUILayout.Vector2Field("Z Pos range: ", zPos);
            }
            randomRotation = EditorGUILayout.Toggle("Random rotation: ", randomRotation);
            if (randomRotation)
            {
                xRot = EditorGUILayout.Vector2Field("X Rot range: ", xRot);
                yRot = EditorGUILayout.Vector2Field("Y Rot range: ", yRot);
                zRot = EditorGUILayout.Vector2Field("Z Rot range: ", zRot);
            }
            randomScale = EditorGUILayout.Toggle("Random scale: ", randomScale);
            if (randomScale)
            {
                EditorGUILayout.LabelField("Scale from " + minScale.ToString("F2") + " to " + maxScale.ToString("F2"));
                EditorGUILayout.MinMaxSlider(ref minScale, ref maxScale, 0, 5);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            if (selection.Length == 0)
            {
                EditorGUILayout.HelpBox("No objects selected", MessageType.Info);
                GUI.enabled = false;
            }
            else GUI.enabled = true;

            PrefabStage scene = PrefabStageUtility.GetCurrentPrefabStage();

            if (GUILayout.Button("Randomize!"))
            {
                for (int i = 0; i < selection.Length; i++)
                {
                    Undo.RecordObject(selection[i].transform, "Randomize");
                    if (randomPosition)
                    {
                        Vector3 newPos;
                        newPos.x = Random.Range(xPos.x, xPos.y);
                        newPos.y = Random.Range(yPos.x, yPos.y);
                        newPos.z = Random.Range(zPos.x, zPos.y);

                        selection[i].transform.position = newPos;
                    }

                    if (randomRotation)
                    {
                        selection[i].transform.eulerAngles = new Vector3(Random.Range(xRot.x, xRot.y), Random.Range(yRot.x, yRot.y), Random.Range(zRot.x, zRot.y));
                    }

                    if (randomScale)
                        selection[i].transform.localScale = Vector3.one * Random.Range(minScale, maxScale);

                    if (scene != null)
                        EditorSceneManager.MarkSceneDirty(scene.scene);
                }
            }
        }
    }
}