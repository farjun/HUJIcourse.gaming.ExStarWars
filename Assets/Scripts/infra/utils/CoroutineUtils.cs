using UnityEngine;
using System;
using System.Collections;

namespace Infra.Utils {
public static class CoroutineUtils {

    public static Coroutine WaitForRealSeconds(MonoBehaviour context, float time) {
        return context.StartCoroutine(WaitForRealSeconds(time));
    }

    public static IEnumerator WaitForRealSeconds(float time) {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time) {
            yield return 0;
        }
    }

    public static Coroutine WaitForCondition(MonoBehaviour context, Func<bool> condition, Action action = null) {
        return context.StartCoroutine(WaitForCondition(condition, action));
    }

    public static IEnumerator WaitForCondition(Func<bool> condition, Action action = null) {
        DebugUtils.Log("Waiting for condition");
        while (!condition()) {
            yield return 0;
        }
        DebugUtils.Log("Condition met");

        if (action != null) {
            action();
        }
    }

    public static Coroutine DelaySeconds(MonoBehaviour context, Action action, float delay) {
        return context.StartCoroutine(DelaySeconds(action, delay));
    }

    /**
     * Usage: StartCoroutine(CoroutineUtils.DelaySeconds(action, delay))
     * For example:
     *     StartCoroutine(CoroutineUtils.DelaySeconds(
     *         () => DebugUtils.Log("2 seconds past"),
     *         2);
     */
    public static IEnumerator DelaySeconds(Action action, float delay) {
        yield return new WaitForSeconds(delay);
        action();
    }

    public static IEnumerator DelayRealSeconds(MonoBehaviour context, Action action, float delay) {
        yield return context.StartCoroutine(CoroutineUtils.WaitForRealSeconds(delay));
        action();
    }

    /**
     * Usage: StartCoroutine(CoroutineUtils.Chain(...))
     * For example:
     *     StartCoroutine(CoroutineUtils.Chain(
     *         CoroutineUtils.Do(() => DebugUtils.Log("A")),
     *         CoroutineUtils.WaitForRealSeconds(2),
     *         CoroutineUtils.Do(() => DebugUtils.Log("B"))));
     */
    public static IEnumerator Chain(MonoBehaviour context, params IEnumerator[] actions) {
        foreach (IEnumerator action in actions) {
            yield return context.StartCoroutine(action);
        }
    }

    public static IEnumerator Do(Action action) {
        action();
        yield return 0;
    }
}
}