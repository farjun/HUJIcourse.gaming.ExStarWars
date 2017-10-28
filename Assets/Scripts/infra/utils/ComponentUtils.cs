#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;

namespace Infra.Utils {
public static class ComponentUtils {
    /// <summary>
    /// Finds a child component by type and name.
    /// Allows to search for inactive game objects as well.
    /// </summary>
    public static T FindChildComponent<T>(GameObject parent, string name, bool includeInactive = false) where T : Behaviour {
        foreach(T comp in parent.GetComponentsInChildren<T>(includeInactive)) {
            if (comp.name == name) {
                return comp;
            }
        }
        return null;
    }


    /// <summary>
    /// Find a child component by type.
    /// Searches also in inactive game objects and in components of the parent
    /// itself.
    /// </summary>
    public static T FindChildComponent<T>(Transform parentTransform) where T : Behaviour {
        var comp = parentTransform.GetComponent<T>();
        if (comp != null) {
            return comp;
        }

        foreach (Transform child in parentTransform) {
            comp = FindChildComponent<T>(child);
            if (comp != null) {
                return comp;
            }
        }
        return null;
    }

    /// <summary>
    /// Find a child game object by name. Can also match the parent itself.
    /// </summary>
    public static GameObject FindChild(Transform parentTransform, string name) {
        foreach (Transform child in parentTransform) {
            if (child.name == name) {
                return child.gameObject;
            }
        }
        return null;
    }

    /// <summary>
    /// Returns a list of children components of the given type.
    /// Searches also in inactive game objects and in components of the parent
    /// itself.
    /// </summary>
    public static List<T> GetChildrenWithComponent<T>(Transform parentTransform) where T : Component {
        var children = new List<T>();
        foreach (Transform child in parentTransform) {
            var childComponent = child.GetComponent<T>();
            if (childComponent != null) {
                children.Add(childComponent);
            }
            children.AddRange(GetChildrenWithComponent<T>(child));
        }
        return children;
    }

    /// <summary>
    /// Instantiates a prefab under a parent.
    /// </summary>
    public static T InstantiatePrefab<T>(T prefab, Transform parentTransform, bool createActive = true) where T : Component {
        bool isActive = prefab.gameObject.activeSelf;
#if UNITY_EDITOR
        T obj;
        if (Application.isPlaying) {
            // For some reason, this does not work in Edit mode...
            prefab.gameObject.SetActive(createActive);
            obj = PrefabUtility.InstantiatePrefab(prefab) as T;
            DebugUtils.Assert(obj != null, "Failed to instantiate prefab. Use Duplicate if you are just trying to duplicate an object that is not a prefab");
            prefab.gameObject.SetActive(isActive);
        } else {
            obj = PrefabUtility.InstantiatePrefab(prefab) as T;
        }
#else
        prefab.gameObject.SetActive(createActive);
        T obj = UnityEngine.Object.Instantiate<T>(prefab);
        prefab.gameObject.SetActive(isActive);
#endif
        obj.transform.SetParent(parentTransform, false);
        return obj;
    }

    /// <summary>
    /// Duplicate an object and place it under a parent.
    /// </summary>
    public static T Duplicate<T>(T prototype, Transform parentTransform, bool createActive = true) where T : Component {
        bool isActive = prototype.gameObject.activeSelf;
        prototype.gameObject.SetActive(createActive);
        T obj = Object.Instantiate<T>(prototype);
        prototype.gameObject.SetActive(isActive);
        obj.transform.SetParent(parentTransform, false);
        return obj;
    }
}
}
