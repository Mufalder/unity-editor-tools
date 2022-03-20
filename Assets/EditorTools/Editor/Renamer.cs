using UnityEngine;
using UnityEditor;
using System.Linq;

namespace NorthLab.EditorTools
{
    public class Renamer : EditorWindow
    {

        private enum Modes { Scene, Assets };
        private Modes mode = Modes.Scene;
        private GameObject[] selection;
        private enum Types { Simple, Incremental, Replace };
        private Types type;

        private string simpleName;

        private string initName;
        private int initNumber;
        private string numberFormat;

        private string search;
        private string replaceWith;
        private bool addToBeginning;

        [MenuItem("Tools/NorthLab/Renamer")]
        public static void ShowWindow()
        {
            GetWindow(typeof(Renamer), false, "Renamer");
        }

        private void OnGUI()
        {
            selection = Selection.gameObjects;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            mode = (Modes)EditorGUILayout.EnumPopup("Rename mode:", mode);

            EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
            if (mode == Modes.Scene)
                EditorGUILayout.LabelField("Selected objects: " + selection.Length);
            else EditorGUILayout.LabelField("Selected objects: " + Selection.assetGUIDs.Length);

            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
            type = (Types)EditorGUILayout.EnumPopup("Type: ", type);
            if (type == Types.Simple)
            {
                simpleName = EditorGUILayout.TextField("Name: ", simpleName);
            }
            else if (type == Types.Incremental)
            {
                initName = EditorGUILayout.TextField("Name: ", initName);
                initNumber = EditorGUILayout.IntField("Initial number: ", initNumber);
                numberFormat = EditorGUILayout.TextField(new GUIContent("Format: ", "Integer ToString() format."), numberFormat);
            }
            else
            {
                search = EditorGUILayout.TextField("Search: ", search);
                replaceWith = EditorGUILayout.TextField("Replace with: ", replaceWith);
                addToBeginning = EditorGUILayout.Toggle("Else add to beginning: ", addToBeginning);
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            if (mode == Modes.Scene)
            {
                if (selection.Length == 0)
                {
                    EditorGUILayout.HelpBox("No objects selected", MessageType.Info);
                    GUI.enabled = false;
                }
                else
                {
                    GUI.enabled = true;
                }
            }
            else
            {
                if (Selection.assetGUIDs.Length == 0)
                {
                    EditorGUILayout.HelpBox("No objects selected", MessageType.Info);
                    GUI.enabled = false;
                }
                else
                {
                    GUI.enabled = true;
                }
            }

            if (GUILayout.Button("Rename!"))
            {
                if (mode == Modes.Scene)
                    RenameSceneObjects();
                else
                    RenameAssets();
            }

            GUI.enabled = true;
        }

        private string GetNameFromPath(string path)
        {
            int separatorIndex = path.LastIndexOf('/');
            int dotIndex = path.LastIndexOf('.');
            return path.Substring(separatorIndex + 1, dotIndex - separatorIndex - 1);
        }

        private string GetNewName(int i, string inputName)
        {
            string result = inputName;

            if (type == Types.Simple)
            {
                result = simpleName;
            }
            else if (type == Types.Incremental)
            {
                result = initName + (i + initNumber).ToString(numberFormat);
            }
            else
            {
                if (inputName.Contains(search))
                    result = inputName.Replace(search, replaceWith);
                else if (addToBeginning)
                    result = replaceWith + inputName;
            }

            return result;
        }

        private void RenameSceneObjects()
        {
            selection = selection.OrderBy(c => c.transform.GetSiblingIndex()).ToArray();

            for (int i = 0; i < selection.Length; i++)
            {
                Undo.RegisterFullObjectHierarchyUndo(selection[i].transform, "Renaming selection");
                selection[i].transform.name = GetNewName(i, selection[i].transform.name);
                //EditorUtility.SetDirty(selection[i].transform);
            }
        }

        private void RenameAssets()
        {
            for (int i = 0; i < Selection.assetGUIDs.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]);
                string initName = GetNameFromPath(path);

                string renameResult = AssetDatabase.RenameAsset(path, GetNewName(i, initName));
                if (!string.IsNullOrEmpty(renameResult))
                    Debug.LogError(renameResult);
            }
            AssetDatabase.Refresh();
        }

    }
}