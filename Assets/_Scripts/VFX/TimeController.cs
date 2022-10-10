using System.Collections;
using UnityEngine;

/// <summary>
/// Call the public methods to transition the time scale to certain value
/// </summary>
public class TimeController : MonoBehaviour
{
    [SerializeField] private float _transitionDuration;

    [SerializeField] private float _timeNormal = 1f;
    [SerializeField] private float _timeSlowed = 0.35f;
    [SerializeField] private float _timeOnDeath = 0.01f;

    private Coroutine _coroutineTransition;



    public void TransitionToSlowMo()
    {
        if (_coroutineTransition != null)
            StopCoroutine(_coroutineTransition);

        _coroutineTransition = StartCoroutine(CoroutineTransition(_timeSlowed, _transitionDuration));
    }

    public void TransitionToNormal()
    {
        if (_coroutineTransition != null)
            StopCoroutine(_coroutineTransition);

        _coroutineTransition = StartCoroutine(CoroutineTransition(_timeNormal, _transitionDuration));
    }

    public void TransitionToDeath()
    {
        if (_coroutineTransition != null)
            StopCoroutine(_coroutineTransition);

        _coroutineTransition = StartCoroutine(CoroutineTransition(_timeOnDeath, _transitionDuration));
    }

    private IEnumerator CoroutineTransition(float target, float duration)
    {
        float current = Time.timeScale;

        float timer = 0;

        while(timer < duration)
        {
            timer += Time.unscaledDeltaTime;

            Time.timeScale = Mathf.Lerp(current, target, timer / duration);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            yield return null;
        }

        Time.timeScale = target;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        _coroutineTransition = null;
    }
}