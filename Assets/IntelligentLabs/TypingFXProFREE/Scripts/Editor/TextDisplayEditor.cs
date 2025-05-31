using TMPro;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace TypingFXProFREE
{
    [CustomEditor(typeof(TextDisplay))]
    public class TextDisplayEditor : Editor
    {
        // Reference to the Ultimate Edition image (free edition only)
        private Texture2D ultimatePreviewImage;

        void OnEnable()
        {
            // Load the image from the specific folder
            ultimatePreviewImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/IntelligentLabs/TypingFXProFREE/Sprites/UltimatePreview.png", typeof(Texture2D));
        }

        public override void OnInspectorGUI()
        {
            TextDisplay textDisplay = (TextDisplay)target;

            textDisplay.fullText = EditorGUILayout.TextArea(textDisplay.fullText, GUILayout.Height(75));
            textDisplay.uiText = (TextMeshProUGUI)EditorGUILayout.ObjectField("UI Text", textDisplay.uiText, typeof(TextMeshProUGUI), true);

            // Define Free Edition effects
            var freeEditionEffects = new[] {
                "FadeIn", "FadeOut", "Glitch", "SlideIn", "Typewriter", "ZoomIn", "ZoomOut"
            };

            // Define Ultimate Edition effects and append " (Get Ultimate)"
            var ultimateEditionEffects = new[] {
                "BlurIn (Get Ultimate)", "BlurNFall (Get Ultimate)", "CinematicDistortion (Get Ultimate)",
                "FlashReveal (Get Ultimate)", "GradientFade (Get Ultimate)", "Hurricane (Get Ultimate)",
                "Jitter (Get Ultimate)", "LightSweep (Get Ultimate)", "MeteorShower (Get Ultimate)",
                "Rainbow (Get Ultimate)", "Ripple (Get Ultimate)", "Shimmer (Get Ultimate)",
                "Storm (Get Ultimate)", "TypingBounce (Get Ultimate)"
            };

            // Combine and sort the effect names (Free effects first, then Ultimate effects)
            var effectNames = freeEditionEffects.Concat(ultimateEditionEffects).ToArray();

            // Find the current effect name (if it’s an ultimate effect, strip the "(Get Ultimate)" part for comparison)
            string currentEffectName = textDisplay.effectType.ToString();
            if (ultimateEditionEffects.Any(e => e.StartsWith(currentEffectName)))
            {
                currentEffectName += " (Get Ultimate)";
            }

            // Get current effect index
            var currentEffectIndex = System.Array.IndexOf(effectNames, currentEffectName);

            // Display dropdown with sorted effect names
            currentEffectIndex = EditorGUILayout.Popup("Effect Type", currentEffectIndex, effectNames);

            // Parse selected effect (remove " (Get Ultimate)" suffix for parsing back to enum)
            string selectedEffect = effectNames[currentEffectIndex].Replace(" (Get Ultimate)", "");
            textDisplay.effectType = (TextDisplay.EffectType)System.Enum.Parse(typeof(TextDisplay.EffectType), selectedEffect);

            // Draw the relevant setup for the selected effect
            switch (textDisplay.effectType)
            {
                case TextDisplay.EffectType.Typewriter:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Typewriter Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.typingDuration = EditorGUILayout.FloatField("Typing Duration", textDisplay.typingDuration);
                    break;

                case TextDisplay.EffectType.SlideIn:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Slide In Setup:", EditorStyles.boldLabel);
                    textDisplay.slideInStartDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.slideInStartDelay);
                    textDisplay.slideDirection = (TextDisplay.SlideDirection)EditorGUILayout.EnumPopup("Slide Direction", textDisplay.slideDirection);
                    textDisplay.slideInDuration = EditorGUILayout.FloatField("Duration", textDisplay.slideInDuration);
                    break;

                case TextDisplay.EffectType.FadeIn:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Fade In Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.fadeInDuration = EditorGUILayout.FloatField("Duration", textDisplay.fadeInDuration);
                    break;

                case TextDisplay.EffectType.FadeOut:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Fade Out Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.fadeOutDuration = EditorGUILayout.FloatField("Duration", textDisplay.fadeOutDuration);
                    break;

                case TextDisplay.EffectType.Glitch:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Glitch Setup:", EditorStyles.boldLabel);
                    textDisplay.startDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.startDelay);
                    textDisplay.glitchDuration = EditorGUILayout.FloatField("Duration", textDisplay.glitchDuration);
                    textDisplay.glitchIntensity = EditorGUILayout.FloatField("Intensity", textDisplay.glitchIntensity);
                    break;

                case TextDisplay.EffectType.ZoomIn:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Zoom In Setup:", EditorStyles.boldLabel);
                    textDisplay.zoomInStartDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.zoomInStartDelay);
                    textDisplay.zoomInDuration = EditorGUILayout.FloatField("Duration", textDisplay.zoomInDuration);
                    break;

                case TextDisplay.EffectType.ZoomOut:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Zoom Out Setup:", EditorStyles.boldLabel);
                    textDisplay.zoomOutStartDelay = EditorGUILayout.FloatField("Start Delay", textDisplay.zoomOutStartDelay);
                    textDisplay.zoomOutDuration = EditorGUILayout.FloatField("Duration", textDisplay.zoomOutDuration);
                    break;
            }

            // Show Ultimate Preview image for Ultimate effects
            if (ultimateEditionEffects.Contains(effectNames[currentEffectIndex]) && ultimatePreviewImage != null)
            {
                GUILayout.Space(15);
                GUILayout.Label("    * Unlock 21+ Exclusive Effects - Click Below *", EditorStyles.boldLabel);
                GUILayout.Space(5);

                // Make the image clickable and open the URL when clicked
                if (ultimatePreviewImage != null)
                {
                    if (GUILayout.Button(ultimatePreviewImage, GUILayout.Width(300), GUILayout.Height(205)))
                    {
                        Application.OpenURL("https://assetstore.unity.com/packages/tools/animation/typing-fx-pro-ultimate-edition-296433");
                    }
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(textDisplay);
            }
        }
    }
}
