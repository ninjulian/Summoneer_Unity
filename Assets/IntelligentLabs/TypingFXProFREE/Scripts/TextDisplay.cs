using TMPro;
using TypingFXProFREE;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace TypingFXProFREE
{
    public class TextDisplay : MonoBehaviour
    {
        [TextArea]
        public string fullText;
        public TextMeshProUGUI uiText;

        public float typingDuration = 2.0f;

        public float slideInDuration = 2.0f;
        public float slideInStartDelay = 0.5f;

        public float fadeInDuration = 2.0f;
        public float fadeOutDuration = 2.0f;

        public float glitchDuration = 2.0f;
        public float glitchIntensity = 0.1f;

        public float zoomInDuration = 2.0f;
        public float zoomInStartDelay = 1.0f;
        public float zoomOutDuration = 2.0f;
        public float zoomOutStartDelay = 1.0f;

        public float startDelay = 1.0f;

        public enum EffectType
        {
            // Free Edition Effects
            FadeIn,
            FadeOut,
            Glitch,
            SlideIn,
            Typewriter,
            ZoomIn,
            ZoomOut,

            // Ultimate Edition Effects
            BlurIn,
            BlurNFall,
            CinematicDistortion,
            FlashReveal,
            GradientFade,
            Hurricane,
            Jitter,
            LightSweep,
            MeteorShower,
            Rainbow,
            Ripple,
            Shimmer,
            Storm,
            TypingBounce
        }
        public EffectType effectType;

        public enum SlideDirection { FromTop, FromLeft, FromRight, FromBottom }
        public SlideDirection slideDirection;

        private EffectManager effectManager;

        private const string ultimateEditionUrl = "https://assetstore.unity.com/packages/tools/animation/typing-fx-pro-ultimate-edition-296433";

        void Start()
        {
            uiText.text = "";
            effectManager = GetComponent<EffectManager>();
            StartCoroutine(DisplayTextWithEffect());
        }

        IEnumerator DisplayTextWithEffect()
        {
            yield return new WaitForSeconds(GetStartDelay());

            // Apply effect with necessary parameters for all supported effects
            effectManager.ApplyEffect(fullText, uiText, effectType, GetEffectDuration());
        }

        private float GetEffectDuration()
        {
            switch (effectType)
            {
                case EffectType.Typewriter:
                    return typingDuration;

                case EffectType.FadeIn:
                    return fadeInDuration;

                case EffectType.FadeOut:
                    return fadeOutDuration;

                case EffectType.ZoomIn:
                    return zoomInDuration;

                case EffectType.ZoomOut:
                    return zoomOutDuration;

                case EffectType.SlideIn:
                    return slideInDuration;

                case EffectType.Glitch:
                    return glitchDuration;

                default:
                    return typingDuration;
            }
        }

        private float GetStartDelay()
        {
            switch (effectType)
            {
                case EffectType.ZoomIn:
                    return zoomInStartDelay;

                case EffectType.ZoomOut:
                    return zoomOutStartDelay;

                case EffectType.SlideIn:
                    return slideInStartDelay;

                default:
                    return startDelay;
            }
        }

        // Action button event
        public void TriggerSelectedEffect()
        {
            // Check if the selected effect is a Free or Ultimate effect
            if (IsUltimateEffect(effectType))
            {
                // Open the Ultimate Edition page
                Application.OpenURL(ultimateEditionUrl);
            }
            else
            {
                // Play the selected effect
                StopAllCoroutines();
                uiText.text = "";
                StartCoroutine(DisplayTextWithEffect());
            }
        }

        // Helper method to check if an effect is from the Ultimate edition (demo use only)
        private bool IsUltimateEffect(EffectType effect)
        {
            switch (effect)
            {
                case EffectType.BlurIn:
                case EffectType.BlurNFall:
                case EffectType.CinematicDistortion:
                case EffectType.FlashReveal:
                case EffectType.GradientFade:
                case EffectType.Hurricane:
                case EffectType.Jitter:
                case EffectType.LightSweep:
                case EffectType.MeteorShower:
                case EffectType.Rainbow:
                case EffectType.Ripple:
                case EffectType.Shimmer:
                case EffectType.Storm:
                case EffectType.TypingBounce:
                    return true;
                default:
                    return false;
            }
        }
    }
}