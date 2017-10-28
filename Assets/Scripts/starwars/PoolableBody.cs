using UnityEngine;
using Infra.Collections;
using Infra.Utils;

namespace StarWars {
public abstract class PoolableBody : Body, IPoolable {
    /// <summary>
    /// Expected params: (Vector2)position, (float)angle.
    /// </summary>
    public virtual int Activate(params object[] activateParams) {
        var index = 0;
        Position = (Vector2)activateParams[index++];
        Rotation = (float)activateParams[index++];

        return index;
    }

    public void ReturnSelf() {
        gameObject.SetActive(false);
    }
}
}
