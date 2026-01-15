using System.Collections.Generic;
using UnityEngine;


public class ChangeColor : MonoBehaviour {

    [SerializeField, Range(0,1000)]public int health;
    SpriteRenderer spriteRenderer;

    [SerializeField] bool updateOnGame;
    [SerializeField] float updateSpeed;


    Dictionary<int, string> colors = new Dictionary<int, string>{
            { 0, "#FFFFFF" },
            { 100, "#3939CC" },
            { 200, "#49C5CC" },
            { 300, "#45CC45" },
            { 400, "#E6E077" },
            { 500, "#E6463E" }
        };

    void Start() {
        spriteRenderer = GameObject.Find("Brick_Visual").GetComponent<SpriteRenderer>();
    }

    void Update() {

        if ( updateOnGame ) {
            health = (int)(500 * Mathf.Abs(Mathf.Sin(Time.time * updateSpeed)));
        }

        var keys = new List<int>(colors.Keys);
        keys.Sort();


        for ( int i = 0; i < keys.Count - 1; i++ ) {
            int lowerKey = keys[i];
            int upperKey = keys[i + 1];

            // Find which range health is in
            if ( health >= lowerKey && health <= upperKey ) {
                // Convert hex strings to Color
                Color lowerColor = Hex(colors[lowerKey]);
                Color upperColor = Hex(colors[upperKey]);

                // Calculate blend ratio (0 → 1 between two color stops)
                float t = (health - lowerKey) / (float)(upperKey - lowerKey);

                // Blend colors smoothly
                Color lerpedColor = Color.Lerp(lowerColor, upperColor, t);
                spriteRenderer.color = lerpedColor;

                break;
            }

        }
    }

    public static Color Hex( string hex ) {
        if ( UnityEngine.ColorUtility.TryParseHtmlString(hex, out var color) )
            return color;

        Debug.LogWarning($"Invalid hex color: {hex}");
        return Color.magenta;
    }

}
