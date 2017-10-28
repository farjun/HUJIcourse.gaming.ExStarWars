using UnityEngine;
using StarWars.Brains;
using Infra;

namespace StarWars {
/// <summary>
/// The visible body of the spaceship. Can move and rotate.
/// </summary>
public class SpaceshipBody : Body {
    public enum Type {
        XWing,
        TieFighter,
    }

    public Color primaryColor = Color.white;
    public Color secondaryColor = Color.white;
    public Type spaceshipType;

    public SpriteRenderer primaryImage;
    public SpriteRenderer secondaryImage;
    public SpriteRenderer shieldImage;
    public ParticleSystem energyEmitter;

    public SpaceshipBrain Brain {
        get {
            return brain;
        }
    }

    private SpaceshipBrain brain;

    public void SetSecondaryColor(Color color) {
        secondaryColor = color;
        secondaryImage.color = secondaryColor;

        var main = energyEmitter.main;
        main.startColor = secondaryColor / secondaryColor.maxColorComponent;
    }

    public static Color NormalizeColor(Color color, int lowPass = 0x10) {
        // Don't allow transparency.
        color.a = 1f;
        // Make sure the color has 0xFF in at least one channel.
        var max = Mathf.Max(color.r, color.g, color.b);
        var min = Mathf.Min(color.r, color.g, color.b);
        // The color is in grayscale. Normalize to white.
        if (Mathf.Approximately(max, min)) return Color.white;
        var diff = max - min;
        var desiredDiff = 1f - min;
        var factor = desiredDiff / diff;
        color.r = (color.r - min) * factor;
        color.g = (color.g - min) * factor;
        color.b = (color.b - min) * factor;
        // All channels must be 0x?F (if lowPass is 0x10).
        color.r = (Mathf.Floor(color.r * 0xFF / lowPass) * lowPass + lowPass - 1) / 0xFF;
        color.g = (Mathf.Floor(color.g * 0xFF / lowPass) * lowPass + lowPass - 1) / 0xFF;
        color.b = (Mathf.Floor(color.b * 0xFF / lowPass) * lowPass + lowPass - 1) / 0xFF;
        return color;
    }

    public void Activate(
            Vector2 position,
            float angle,
            Space.Mutable space) {
        Position = position;
        Rotation = angle;
        brain = GetComponent<SpaceshipBrain>();
        DebugUtils.Assert(
            brain != null,
            name + " does not have a brain!\n" +
                "Did you add the _SpaceshipTemplate_ to the scene " +
                "or forgot to add your brain script to the prefab?");
        space.RegisterSpaceship(this, brain);

        name = brain.Name;

        // Set visuals.
        var imagesConfig = Game.spaceshipImages[brain.BodyType];
        primaryImage.sprite = imagesConfig.primaryImage;
        secondaryImage.sprite = imagesConfig.secondaryImage;

        primaryColor = NormalizeColor(brain.PrimaryColor);
        primaryImage.color = primaryColor;

        var shieldColor = primaryColor / primaryColor.maxColorComponent;
        shieldColor.a = 0.3f;
        shieldImage.color = shieldColor;
    }
}
}
