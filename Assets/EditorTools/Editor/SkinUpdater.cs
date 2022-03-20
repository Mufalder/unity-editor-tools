using UnityEngine;
using UnityEditor;

namespace NorthLab.EditorTools
{
    public class SkinUpdater : EditorWindow
    {

        private SkinnedMeshRenderer oldSkin;
        private SkinnedMeshRenderer referenceSkin;

        private bool bonesFoldout;

        private Vector2 scrollPos;

        [MenuItem("Tools/NorthLab/Skin Updater")]
        public static void OpenWindow()
        {
            GetWindow(typeof(SkinUpdater), false, "Skin Updater");
        }

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            oldSkin = EditorGUILayout.ObjectField("Old Skin", oldSkin, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
            referenceSkin = EditorGUILayout.ObjectField("Reference Skin", referenceSkin, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
            EditorGUILayout.Space();

            bonesFoldout = EditorGUILayout.Foldout(bonesFoldout, "Bones comparison:");
            EditorGUILayout.BeginHorizontal();
            if (bonesFoldout)
            {
                if (oldSkin)
                {
                    DrawBonesHierarchy(oldSkin);
                }
                if (referenceSkin)
                {
                    DrawBonesHierarchy(referenceSkin);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            GUI.enabled = oldSkin != null && referenceSkin != null;
            if (GUILayout.Button("Update Skinned Mesh Renderer"))
            {

                Transform[] newBones = new Transform[referenceSkin.bones.Length];
                for (int i = 0; i < referenceSkin.bones.Length; i++)
                {
                    Transform bone = Find(oldSkin.rootBone, referenceSkin.bones[i].name);
                    if (bone != null)
                    {
                        newBones[i] = bone;
                    }
                }
                oldSkin.bones = newBones;
            }
            GUI.enabled = true;

            EditorGUILayout.EndScrollView();
        }

        private static void DrawBonesHierarchy(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            EditorGUILayout.BeginVertical();
            GUI.enabled = false;
            for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
            {
                if (skinnedMeshRenderer.bones[i])
                    EditorGUILayout.LabelField(i + ": " + skinnedMeshRenderer.bones[i].name);
                else EditorGUILayout.LabelField(i + ": null");
            }
            GUI.enabled = true;
            EditorGUILayout.EndVertical();
        }

        private static Transform Find(Transform target, string name)
        {
            if (target.name == name)
                return target;

            return FindRecursive(target, name);
        }

        private static Transform FindRecursive(Transform target, string name)
        {
            foreach (Transform child in target)
            {
                if (child.name == name)
                {
                    return child;
                }
                else
                {
                    Transform search = Find(child, name);
                    if (search != null)
                        return search;
                }
            }
            return null;
        }
    }
}