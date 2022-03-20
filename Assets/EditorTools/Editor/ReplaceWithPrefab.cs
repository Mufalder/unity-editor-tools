using UnityEngine;
using UnityEditor;

namespace NorthLab.EditorTools
{
    public class ReplaceWithPrefab : EditorWindow
    {

        private GameObject prefab;
        private bool keepScale = true;
        private bool randomize = false;
        private int random = 50;

        [MenuItem("Tools/NorthLab/Replace With Prefab")]
        public static void CreateReplaceWithPrefab()
        {
            GetWindow(typeof(ReplaceWithPrefab), false, "Replace With Prefab");
        }

        private void OnGUI()
        {
            prefab = (GameObject)EditorGUILayout.ObjectField("Prefab:", prefab, typeof(GameObject), false);
            keepScale = EditorGUILayout.Toggle("Keep scale:", keepScale);
            randomize = EditorGUILayout.Toggle("Randomize:", randomize);
            if (randomize)
                random = EditorGUILayout.IntSlider("Chance:", random, 0, 100);//EditorGUILayout.IntField("Random:", random);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);

            if (Selection.gameObjects.Length == 0)
            {
                EditorGUILayout.HelpBox("No objects selected", MessageType.Info);
                GUI.enabled = false;
            }
            else
            {
                GUI.enabled = true;
            }

            if (GUILayout.Button("Replace"))
            {
                Replace();
            }

            GUI.enabled = true;
        }

        private void Replace()
        {
            PrefabType prefabType = PrefabUtility.GetPrefabType(prefab);

            for (int i = Selection.gameObjects.Length - 1; i >= 0; --i)
            {
                if (randomize && random <= Random.Range(0, 101))
                    continue;

                GameObject selected = Selection.gameObjects[i];
                GameObject newObject;

                if (prefabType == PrefabType.Prefab)
                {
                    newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                }
                else
                {
                    newObject = Instantiate(prefab);
                    newObject.name = prefab.name;
                }

                if (newObject == null)
                {
                    Debug.LogError("Error instantiating prefab");
                    break;
                }

                Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
                newObject.transform.parent = selected.transform.parent;
                newObject.transform.localPosition = selected.transform.localPosition;
                newObject.transform.localRotation = selected.transform.localRotation;
                if (keepScale)
                    newObject.transform.localScale = selected.transform.localScale;
                newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
                Undo.DestroyObjectImmediate(selected);
            }
        }
    }
}