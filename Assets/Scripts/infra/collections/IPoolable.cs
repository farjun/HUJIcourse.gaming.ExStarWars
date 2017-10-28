namespace Infra.Collections {
/// <summary>
/// Poolable items are "returned to the pool" implicitly when disabled.
/// To be used with a GameObjectPool.
/// </summary>
public interface IPoolable {

    /// <summary>
    /// Only needs to initialize the object. No need to SetActive the GO.
    /// Some params can be optional (specified in the overriden methods comment).
    /// A parent's optional params must be supplied when activating the child.
    /// </summary>
    /// <returns>The number of paramaters used from the params array.</returns>
    int Activate(params object[] activateParams);

    void ReturnSelf();
}
}
