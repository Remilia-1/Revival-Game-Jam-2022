using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Call the public methods to transition the "slowmo" add post process
/// </summary>
public class PostProcessController : MonoBehaviour
{
    [SerializeField] private Volume _postProcessSlowMo;

    [SerializeField] private float _transitionDuration;

    private Coroutine coroutine_Transition;



    public void TransitionToSlowMo()
    {
        if (coroutine_Transition != null)
            StopCoroutine(coroutine_Transition);

        coroutine_Transition = StartCoroutine(CoroutineTransition(1, _transitionDuration));
    }

    public void TransitionToNormal()
    {
        if (coroutine_Transition != null)
            StopCoroutine(coroutine_Transition);

        coroutine_Transition = StartCoroutine(CoroutineTransition(0, _transitionDuration));
    }

    private IEnumerator CoroutineTransition(float target, float duration)
    {
        float current = _postProcessSlowMo.weight;

        float timer = 0;

        while(timer < duration)
        {
            timer += Time.deltaTime;

            _postProcessSlowMo.weight = Mathf.Lerp(current, target, timer / duration);

            yield return null;
        }

        _postProcessSlowMo.weight = target;
    }
}
