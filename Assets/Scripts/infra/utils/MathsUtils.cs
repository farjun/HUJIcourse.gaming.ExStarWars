using UnityEngine;

namespace Infra.Utils {
public static class MathsUtils {

    /// <summary>
    /// Returns the angle between the vector and positive X axis (right).
    /// Angle is in range (-180, 180].
    /// Positive is counter clock wise.
    /// </summary>
    public static float GetAngle(this Vector2 direction) {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle > 180f) {
            angle -= 360f;
        } else if (angle <= -180f) {
            angle += 360f;
        }
        return angle;
    }

    /// <summary>
    /// Returns the angle between the vector and an axis.
    /// Angle is in range (-180, 180].
    /// Positive is counter clock wise.
    /// </summary>
    public static float GetAngle(this Vector2 direction, Vector2 axis) {
        float angle = direction.GetAngle() - axis.GetAngle();
        if (angle > 180f) {
            angle -= 360f;
        } else if (angle <= -180f) {
            angle += 360f;
        }
        return angle;
    }

    /// <summary>
    /// Rotate a vector by specified degrees (in radians).
    /// </summary>
    public static Vector2 Rotate(this Vector2 v, float radians) {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        float tx = (cos * v.x) - (sin * v.y);
        float ty = (sin * v.x) + (cos * v.y);

        return new Vector2(tx, ty);
    }

    public static Vector2 GetWithMagnitude(this Vector2 v, float magnitude) {
        return v.normalized * magnitude;
    }

    public static Vector3 MultiplyChannels(this Vector3 lhs, Vector3 rhs) {
        lhs.x *= rhs.x;
        lhs.y *= rhs.y;
        lhs.z *= rhs.z;
        return lhs;
    }

    public static Vector2 MultiplyChannels(this Vector2 lhs, Vector2 rhs) {
        lhs.x *= rhs.x;
        lhs.y *= rhs.y;
        return lhs;
    }

    public static Vector2 MultiplyChannels(this Vector2 lhs, float x, float y) {
        lhs.x *= x;
        lhs.y *= y;
        return lhs;
    }

    /// <summary>
    /// C# modulus is strange and allows negative numbers. Use this to get
    /// only non-negative values.
    /// </summary>
    public static int Mod(int x, int m) {
        int r = x % m;
        return r < 0 ? r + m : r;
    }

    /// <summary>
    /// C# modulus is strange and allows negative numbers. Use this to get
    /// only non-negative values.
    /// </summary>
    public static float Mod(float x, float m) {
        float r = x % m;
        return r < 0f ? r + m : r;
    }

    public static uint Max(uint lhs, uint rhs) {
        return lhs < rhs ? rhs : lhs;
    }
}
}
