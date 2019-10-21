using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PackTexture
{
    [MenuItem("Window/Edit Texture 8")]
    public static void Pack()
    {
        var array = new Texture2D[16];
        array[0x0] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01a.png");
        array[0x1] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01b.png");
        array[0x2] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01c.png");
        array[0x3] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01d.png");
        array[0x4] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01e.png");
        array[0x5] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01f.png");
        array[0x6] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01g.png");
        array[0x7] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01h.png");
        array[0x8] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01i.png");
        array[0x9] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01j.png");
        array[0xa] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01k.png");
        array[0xb] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01l.png");
        array[0xc] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01m.png");
        array[0xd] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01n.png");
        array[0xe] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01o.png");
        array[0xf] = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Images/From Pipoya/Simple Enemy Symbol/8/pipo-simpleenemy01p.png");
        var answer = new Texture2D(1024, 512, TextureFormat.RGBA32, false, false);
        for (var i = 0; i < 4; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                for (var xIndex = 0; xIndex < 192; xIndex++)
                {
                    for (var yIndex = 0; yIndex < 128; yIndex++)
                    {
                        answer.SetPixel(192 * j + xIndex, 128 * i + yIndex, array[i * 4 + j].GetPixel(xIndex, yIndex));
                    }
                }                
            }
        }
        AssetDatabase.CreateAsset(answer, "Assets/Images/From Pipoya/Simple Enemy Symbol/8/characters.asset");
    }
}