using System.IO;
using System.Linq;
using UnityEditor;

// ReSharper disable InconsistentNaming

public static class BuildWebGLPlayer
{
    private static readonly string[] scenes =
    {
        "Assets/Scenes/Title.unity",
        "Assets/Scenes/Game.unity",
        "Assets/Scenes/Result.unity",
        "Assets/naichilab/unity-simple-ranking/Scenes/Ranking.unity",
    };

    public static void Build()
    {
        BuildPipeline.BuildPlayer(scenes, "../artifact", BuildTarget.WebGL, BuildOptions.None);
    }

    [MenuItem("Window/Utility/Scene Create")]
    public static void CreateSceneCsFile()
    {
        const string path = @"Assets/Scripts/SceneEnum.cs";
        using (var writer = new StreamWriter(path))
        {
            var first = "namespace AoAndSugi\n{\n\tpublic enum SceneEnum\n\t{";
            writer.WriteLine(first);
            foreach (var name in scenes.Select(Path.GetFileNameWithoutExtension))
            {
                writer.Write("\t\t");
                writer.Write(name);
                writer.WriteLine(",");
            }
            writer.Write("\t}\n}");
        }
    }
}
