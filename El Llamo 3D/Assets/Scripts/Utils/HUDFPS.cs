using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Utilities/HUDFPS")]
public class HUDFPS : MonoBehaviour
{
    // Attach this to any object to make a frames/second indicator.
    //
    // It calculates frames/second over each updateInterval,
    // so the display does not keep changing wildly.
    //
    // It is also fairly accurate at very low FPS counts (<10).
    // We do this not by simply counting frames per interval, but
    // by accumulating FPS for each frame. This way we end up with
    // corstartRect overall FPS even if the interval renders something like
    // 5.5 frames.

    public Rect startRect = new Rect(10, 10, 75, 50); // The rect the window is initially displayed at.
    public bool updateColor = true; // Do you want the color to change if the FPS gets low
    public bool allowDrag = true; // Do you want to allow the dragging of the FPS window
    public float frequency = 0.5F; // The update frequency of the fps
    [Range(1, 4)]
    public int nbDecimal = 1; // How many decimal do you want to display
    [Range(0, 20)]
    public float padding = 10f;
    public int fontSize = 32;

    private float accum = 0f; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private Color color = Color.white; // The color of the GUI, depending of the FPS ( R < 10, Y < 30, G >= 30 )
    private string sFPS = ""; // The fps formatted into a string.
    private GUIStyle style; // The style the text will be displayed at, based en defaultSkin.label.

    private string maxsFPS = ""; // The fps formatted into a string.
    private string minsFPS = ""; // The fps formatted into a string.
    private float maxFPS = -1f;
    private float minFPS = 100f;

    void Start()
    {
#if UNITY_EDITOR
        StartCoroutine(FPS());
#else
        Destroy(this);
#endif
    }

    void Update()
    {
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
    }

    IEnumerator FPS()
    {
        // Infinite loop executed every "frenquency" secondes.
        while (true)
        {
            // Update the FPS
            float fps = accum / frames;
            sFPS = fps.ToString("f" + Mathf.Clamp(nbDecimal, 0, 10));

            minsFPS = minFPS.ToString("f" + Mathf.Clamp(nbDecimal, 0, 10));
            maxsFPS = maxFPS.ToString("f" + Mathf.Clamp(nbDecimal, 0, 10));

            if (fps > maxFPS)
            {
                maxFPS = fps;
            }

            if (fps < minFPS)
            {
                minFPS = fps;
            }

            //Update the color
            color = (fps >= 30) ? Color.green : ((fps > 10) ? Color.red : Color.yellow);

            accum = 0.0F;
            frames = 0;

            yield return new WaitForSeconds(frequency);
        }
    }

    void OnGUI()
    {
        // Copy the default label skin, change the color and the alignement
        if (style == null)
        {
            style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = Color.white;
            style.alignment = TextAnchor.UpperLeft;
        }

        style.fontSize = fontSize;

        GUI.color = updateColor ? color : Color.white;
        startRect = GUI.Window(0, startRect, DoMyWindow, "");
    }

    void DoMyWindow(int windowID)
    {
        int height = (int)(startRect.height / 3);
        GUI.Label(new Rect(padding, 0, startRect.width, startRect.height), "Min " + minsFPS + " FPS", style);
        GUI.Label(new Rect(padding, height, startRect.width, startRect.height), "Current: " + sFPS + " FPS", style);
        GUI.Label(new Rect(padding, height * 2, startRect.width, startRect.height), "Max " + maxsFPS + " FPS", style);
        if (allowDrag) GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
    }
}