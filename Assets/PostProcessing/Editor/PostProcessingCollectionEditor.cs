using UnityEditor;
using UnityEngine;

namespace PostProcessing
{
    [CustomEditor(typeof(PostProcessingCollection))]
    public class PostProcessingCollectionEditor : Editor
    {
        private SerializedObject serObj;

        private SerializedProperty postProcessingMaterials;

        private void OnEnable()
        {
            serObj = new SerializedObject(target);

            postProcessingMaterials = serObj.FindProperty("postProcessingMaterials");
        }

        public override void OnInspectorGUI()
        {
            serObj.Update();
            EditorGUILayout.LabelField("Post-Processing Materials");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            //Material Serialization
            EditorGUI.indentLevel++;
            //Extract all materials from the target list
            PostProcessingCollection targetCollection = (PostProcessingCollection)target;
            EditorGUILayout.Space();
            for (int i = 0; i < targetCollection.postProcessingMaterials.Capacity; i++)
            {
                Material m = targetCollection.postProcessingMaterials[i];
                m = (Material)EditorGUILayout.ObjectField(m, typeof(Material), false);
                //Check Post Processing Attribute/Keyword
                if (m && !m.HasProperty("_PostProcessing"))
                {
                    EditorGUILayout.HelpBox("Material is not a Post-Processing material!", MessageType.Warning);
                }
                else if (m)
                {
                    m.SetFloat("Strength", EditorGUILayout.FloatField("_Value", m.GetFloat("_Value")));
                }
            }
            EditorGUILayout.Separator();
            //List Size editing
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                targetCollection.postProcessingMaterials.Capacity++;
                targetCollection.postProcessingMaterials.Add(null);
            }
            if (GUILayout.Button("-"))
            {
                targetCollection.postProcessingMaterials.RemoveAt(targetCollection.postProcessingMaterials.Count - 1);
                targetCollection.postProcessingMaterials.Capacity--;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            serObj.ApplyModifiedProperties();
        }
    }
}
