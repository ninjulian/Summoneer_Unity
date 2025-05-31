using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace TypingFXProFREE
{
    [CustomEditor(typeof(SceneAsset))]
    public class TypingFXProInspectorNote : Editor
    {
        private GUIStyle whiteLabelStyle;
        private GUIStyle whiteBoldStyle;
        private GUIStyle sectionDividerStyle;
        private GUIStyle headerStyle;
        private GUIStyle noteStyle;

        private void InitializeStyles()
        {
            if (whiteLabelStyle == null)
            {
                whiteLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    normal = { textColor = Color.white },
                    wordWrap = true
                };
            }

            if (whiteBoldStyle == null)
            {
                whiteBoldStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    normal = { textColor = Color.white }
                };
            }

            if (sectionDividerStyle == null)
            {
                sectionDividerStyle = new GUIStyle
                {
                    normal = { background = Texture2D.grayTexture },
                    fixedHeight = 2,
                    margin = new RectOffset(0, 0, 10, 10)
                };
            }

            if (headerStyle == null)
            {
                headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 14,
                    normal = { textColor = Color.red },
                    alignment = TextAnchor.MiddleCenter
                };
            }

            if (noteStyle == null)
            {
                noteStyle = new GUIStyle(EditorStyles.label)
                {
                    fontStyle = FontStyle.Italic,
                    wordWrap = true
                };
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            InitializeStyles();

            if (target.name == "DemoSceneFREE")
            {
                GUILayout.Space(10);

                EditorGUILayout.LabelField("Typing FX Pro: Free Edition", headerStyle);
                GUILayout.Space(5);

                GUILayout.Box(GUIContent.none, sectionDividerStyle);

                EditorGUILayout.LabelField("Important:", whiteBoldStyle);

                GUILayout.Space(5);

                EditorGUILayout.LabelField(
                    "Before loading the scene, ensure that TextMeshPro is properly imported into your project.",
                    whiteLabelStyle);

                GUILayout.Space(10);

                EditorGUILayout.LabelField("• Navigate to Window > TextMeshPro > Import TMP Essential Resources.", whiteLabelStyle);
                GUILayout.Space(5);
                EditorGUILayout.LabelField("• Click the Import button to add the package.", whiteLabelStyle);

                GUILayout.Space(10);

                EditorGUILayout.LabelField(
                    "Note: without TextMeshPro, text effects may not function as intended.",
                    noteStyle);

                GUILayout.Space(10);

                GUILayout.Box(GUIContent.none, sectionDividerStyle);

                EditorGUILayout.LabelField("Powered by Intelligent Labs", whiteBoldStyle);
            }
        }
    }
}