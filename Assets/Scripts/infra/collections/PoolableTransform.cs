using UnityEngine;

namespace Infra.Collections {
public class PoolableTransform : MonoBehaviour, IPoolable {
    public virtual void ReturnSelf() {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Expected params: (Vector2)position, [optional](float)angle
    /// </summary>
    public virtual int Activate(params object[] activateParams) {
        float angle = 0;

        var index = 0;
        var position = (Vector2)activateParams[index++];
        if (activateParams.Length > index) {
            angle = (float)activateParams[index++];
        }

        transform.localPosition = position;
        transform.localRotation = Quaternion.Euler(0, 0, angle);

        Vector3 scale = transform.localScale;
        if (angle > 90 || angle < -90) {
            // Flip the graphics.
            scale.y = -Mathf.Abs(scale.y);
        } else {
            scale.y = Mathf.Abs(scale.y);
        }
        transform.localScale = scale;

        return index;
    }
}
}
