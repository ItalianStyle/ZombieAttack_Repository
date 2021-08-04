using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ZombieAttack
{
    [RequireComponent(typeof(Image))]
    public class TriggerPoisoningTimer : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            PoisoningEffect.OnPoisoningEffectStarted += (duration) => StartCoroutine(nameof(UpdatePoisoningTimeBar), duration);           
            PoisoningEffect.OnPoisoningEffectFinished += () => StopCoroutine(nameof(UpdatePoisoningTimeBar)); ;
        }

        IEnumerator UpdatePoisoningTimeBar(float maxDuration)
        {
            Image timeBar = GetComponent<Image>();
            float timeElapsed = maxDuration;
            while (timeElapsed >= 0f)
            {
                timeElapsed -= Time.deltaTime;
                timeBar.fillAmount = timeElapsed / maxDuration;
                yield return null;
            }           
        }
    }
}