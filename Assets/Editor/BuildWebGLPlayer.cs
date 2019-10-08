using UnityEditor;

// ReSharper disable InconsistentNaming

public static class BuildWebGLPlayer
{
    public static void Build()
    {
        string[] scenes =
        {
            "Assets/Scenes/Title.unity"
        };
        BuildPipeline.BuildPlayer(scenes, "../artifact/", BuildTarget.WebGL, BuildOptions.None);
    }
}
