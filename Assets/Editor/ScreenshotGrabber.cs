#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ScreenshotGrabber
{
    static int count;

    [MenuItem("Screenshot/Grab")]
    public static void Grab()
    {
        ++count;

        ScreenCapture.CaptureScreenshot("Screenshot_" + count + ".png", 1);

    }
}
#endif