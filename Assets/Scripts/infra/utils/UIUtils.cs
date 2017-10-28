using UnityEngine;

namespace Infra.Utils {
public static class UIUtils {
    public static Vector2 GetParentPosition(this RectTransform transform) {
        return transform.parent.GetComponent<RectTransform>().anchoredPosition;
    }

    public static void DecreaseSizeToFitAspect(this RectTransform transform, float aspect) {
        Rect maxSize = transform.rect;
        float originalAspect = maxSize.width / maxSize.height;
        DebugUtils.Log("aspect " + aspect);
        DebugUtils.Log("originalAspect " + originalAspect);
        if (originalAspect > aspect) {
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxSize.width * aspect / originalAspect);
        } else if (originalAspect < aspect) {
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxSize.height * originalAspect / aspect);
        }
    }

    public static void SetBottom(this RectTransform rect, float value) {
        var tempVec = rect.offsetMin;
        tempVec.y = value;
        rect.offsetMin = tempVec;
    }

    public static void SetLeft(this RectTransform rect, float value) {
        var tempVec = rect.offsetMin;
        tempVec.x = value;
        rect.offsetMin = tempVec;
    }

    public static void SetTop(this RectTransform rect, float value) {
        var tempVec = rect.offsetMax;
        tempVec.y = value;
        rect.offsetMax = tempVec;
    }

    public static void SetRight(this RectTransform rect, float value) {
        var tempVec = rect.offsetMax;
        tempVec.x = value;
        rect.offsetMax = tempVec;
    }

    public static void SetX(this RectTransform rect, float value) {
        var tempVec = rect.anchoredPosition;
        tempVec.x = value;
        rect.anchoredPosition = tempVec;
    }

    public static void SetY(this RectTransform rect, float value) {
        var tempVec = rect.anchoredPosition;
        tempVec.y = value;
        rect.anchoredPosition = tempVec;
    }

    public static void SetHeight(this RectTransform rect, float value) {
        var tempVec = rect.sizeDelta;
        tempVec.y = value;
        rect.sizeDelta = tempVec;
    }

    public static void SetWidth(this RectTransform rect, float value) {
        var tempVec = rect.sizeDelta;
        tempVec.x = value;
        rect.sizeDelta = tempVec;
    }

    public static Resolution ScreenResolution {
        get {
#if UNITY_EDITOR
            var T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            var GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var res = (Vector2)GetSizeOfMainGameView.Invoke(null,null);
            var resolution = new Resolution();
            resolution.width = (int)res.x;
            resolution.height = (int)res.y;
            return resolution;
#else
            return Screen.currentResolution;
#endif
        }
    }
}
}
