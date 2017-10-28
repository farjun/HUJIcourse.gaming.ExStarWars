using UnityEngine;

namespace Infra.Utils {
public class Disabler : MonoBehaviour {
    public float disableAfterTimeout;

    protected void OnEnable() {
        if (disableAfterTimeout > 0) {
            CoroutineUtils.DelaySeconds(this, Disable, disableAfterTimeout);
        }
    }

    public void Disable() {
        gameObject.SetActive(false);
    }
}
}
