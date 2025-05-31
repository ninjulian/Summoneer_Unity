using UnityEngine;
using TMPro;
using System.Collections;

namespace TypingFXProFREE
{
    public class EffectManager : MonoBehaviour
    {
        public void ApplyEffect(string fullText, TextMeshProUGUI uiText,
        TextDisplay.EffectType effectType, float effectDuration)

        {
            // Reset text properties before applying any effect
            ResetTextProperties(uiText);

            switch (effectType)
            {
                case TextDisplay.EffectType.Typewriter:
                    StartCoroutine(TypewriterEffect(fullText, uiText, effectDuration));
                    break;

                case TextDisplay.EffectType.SlideIn:
                    StartCoroutine(SlideInEffect(fullText, uiText, effectDuration, GetComponent<TextDisplay>().slideDirection));
                    break;

                case TextDisplay.EffectType.FadeIn:
                    StartCoroutine(FadeInEffect(fullText, uiText, effectDuration));
                    break;

                case TextDisplay.EffectType.FadeOut:
                    StartCoroutine(FadeOutEffect(fullText, uiText, effectDuration));
                    break;

                case TextDisplay.EffectType.Glitch:
                    StartCoroutine(GlitchEffect(fullText, uiText, GetComponent<TextDisplay>().glitchDuration, GetComponent<TextDisplay>().glitchIntensity));
                    break;

                case TextDisplay.EffectType.ZoomIn:
                    StartCoroutine(ZoomInEffect(fullText, uiText, effectDuration));
                    break;

                case TextDisplay.EffectType.ZoomOut:
                    StartCoroutine(ZoomOutEffect(fullText, uiText, GetComponent<TextDisplay>().zoomOutDuration));
                    break;
            }
        }

        private void ResetTextProperties(TextMeshProUGUI uiText)
        {
            // Reset scale and other properties to defaults
            uiText.transform.localScale = Vector3.one;
            uiText.enabled = true;

            uiText.color = Color.white;
            StopAllCoroutines();
        }

        // Typewriter Effect
        private IEnumerator TypewriterEffect(string fullText, TextMeshProUGUI uiText, float typingDuration)
        {
            uiText.text = "";
            float delayPerCharacter = typingDuration / fullText.Length;

            foreach (char c in fullText.ToCharArray())
            {
                uiText.text += c;
                yield return new WaitForSeconds(delayPerCharacter);
            }
        }

        // Fade In Effect
        private IEnumerator FadeInEffect(string fullText, TextMeshProUGUI uiText, float fadeDuration)
        {
            uiText.text = fullText;
            Color originalColor = uiText.color;
            Color fadeColor = originalColor;
            fadeColor.a = 0;
            uiText.color = fadeColor;

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                fadeColor.a = alpha;
                uiText.color = fadeColor;
                yield return null;
            }
            uiText.color = originalColor;
        }

        // Fade Out Effect
        private IEnumerator FadeOutEffect(string fullText, TextMeshProUGUI uiText, float fadeDuration)
        {
            uiText.text = fullText;
            Color originalColor = uiText.color;

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
                originalColor.a = alpha;
                uiText.color = originalColor;
                yield return null;
            }
            originalColor.a = 0f;
            uiText.color = originalColor;
        }

        // Zoom In Effect
        private IEnumerator ZoomInEffect(string fullText, TextMeshProUGUI uiText, float zoomDuration)
        {
            uiText.text = fullText;
            uiText.transform.localScale = new Vector3(0.1f, 0.1f, 1f);

            float elapsedTime = 0f;
            while (elapsedTime < zoomDuration)
            {
                elapsedTime += Time.deltaTime;
                float scale = Mathf.Lerp(0.1f, 1f, elapsedTime / zoomDuration);
                uiText.transform.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }
            uiText.transform.localScale = Vector3.one;
        }

        // Zoom Out Effect
        private IEnumerator ZoomOutEffect(string fullText, TextMeshProUGUI uiText, float zoomOutDuration)
        {
            uiText.text = fullText;
            uiText.transform.localScale = Vector3.one;

            float elapsedTime = 0f;
            while (elapsedTime < zoomOutDuration)
            {
                elapsedTime += Time.deltaTime;
                float scale = Mathf.Lerp(1f, 0f, elapsedTime / zoomOutDuration);
                uiText.transform.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }
            uiText.transform.localScale = Vector3.zero;
        }

        // Slide In Effect
        private IEnumerator SlideInEffect(string fullText, TextMeshProUGUI uiText, float slideDuration, TextDisplay.SlideDirection direction)
        {
            uiText.text = fullText;
            Vector3 originalPosition = uiText.transform.localPosition;

            Vector3 startPosition = Vector3.zero;
            RectTransform rectTransform = uiText.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                switch (direction)
                {
                    case TextDisplay.SlideDirection.FromTop:
                        startPosition = new Vector3(originalPosition.x, Screen.height, originalPosition.z);
                        break;
                    case TextDisplay.SlideDirection.FromLeft:
                        startPosition = new Vector3(-Screen.width, originalPosition.y, originalPosition.z);
                        break;
                    case TextDisplay.SlideDirection.FromRight:
                        startPosition = new Vector3(Screen.width, originalPosition.y, originalPosition.z);
                        break;
                    case TextDisplay.SlideDirection.FromBottom:
                        startPosition = new Vector3(originalPosition.x, -Screen.height, originalPosition.z);
                        break;
                }
            }
            uiText.transform.localPosition = startPosition;

            float elapsedTime = 0f;
            while (elapsedTime < slideDuration)
            {
                elapsedTime += Time.deltaTime;
                uiText.transform.localPosition = Vector3.Lerp(startPosition, originalPosition, elapsedTime / slideDuration);
                yield return null;
            }
            uiText.transform.localPosition = originalPosition;
        }

        // Glitch Effect
        private IEnumerator GlitchEffect(string fullText, TextMeshProUGUI uiText, float glitchDuration, float glitchIntensity)
        {
            uiText.text = fullText;

            float elapsedTime = 0f;
            while (elapsedTime < glitchDuration)
            {
                elapsedTime += Time.deltaTime;
                if (Random.value < glitchIntensity)
                {
                    uiText.enabled = !uiText.enabled;  // Random flicker
                }
                yield return null;
            }
            uiText.enabled = true;
        }
    }
}