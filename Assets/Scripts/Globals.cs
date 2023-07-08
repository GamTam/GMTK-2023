using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Globals
{
    public static MusicManager MusicManager;
    public static SoundManager SoundManager;

    public static Dictionary<string, ArrayList> LoadTSV(TextAsset file) {
        
        Dictionary<string, ArrayList> dictionary = new Dictionary<string, ArrayList>();
        ArrayList list = new ArrayList();
        
        var content = file.text;
        var lines = content.Split(System.Environment.NewLine);

        for (int i=0; i < lines.Length; i++)
        {
            list = new ArrayList();
            var line = lines[i];
            if (string.IsNullOrEmpty(line)) continue;
            string[] values = line.Split('	');
            for (int j=1; j < values.Length; j++) {
                list.Add(values[j]);
            }

            values[0] = new string(values[0].Where(c => !char.IsControl(c)).ToArray());
            if (values[0] != "") dictionary.Add(values[0], list);
        }

        return dictionary;
    }
    
    public static bool IsAnimationPlaying(Animator anim, string stateName, int animLayer=0)
    {
        if (anim.GetCurrentAnimatorStateInfo(animLayer).IsName(stateName) &&
            anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime < 1.0f)
            return true;
        
        return false;
    }
    
    public static string RemoveRichText(string str)
    {
     
        str = RemoveRichTextDynamicTag(str, "color");
     
        str = RemoveRichTextTag(str, "b");
        str = RemoveRichTextTag(str, "i");
        
        str = RemoveRichTextDynamicTag(str, "align");
        str = RemoveRichTextDynamicTag(str, "size");
        str = RemoveRichTextDynamicTag(str, "cspace");
        str = RemoveRichTextDynamicTag(str, "font");
        str = RemoveRichTextDynamicTag(str, "indent");
        str = RemoveRichTextDynamicTag(str, "line-height");
        str = RemoveRichTextDynamicTag(str, "line-indent");
        str = RemoveRichTextDynamicTag(str, "link");
        str = RemoveRichTextDynamicTag(str, "margin");
        str = RemoveRichTextDynamicTag(str, "margin-left");
        str = RemoveRichTextDynamicTag(str, "margin-right");
        str = RemoveRichTextDynamicTag(str, "mark");
        str = RemoveRichTextDynamicTag(str, "mspace");
        str = RemoveRichTextDynamicTag(str, "noparse");
        str = RemoveRichTextDynamicTag(str, "nobr");
        str = RemoveRichTextDynamicTag(str, "page");
        str = RemoveRichTextDynamicTag(str, "pos");
        str = RemoveRichTextDynamicTag(str, "space");
        str = RemoveRichTextDynamicTag(str, "sprite index");
        str = RemoveRichTextDynamicTag(str, "sprite name");
        str = RemoveRichTextDynamicTag(str, "sprite");
        str = RemoveRichTextDynamicTag(str, "style");
        str = RemoveRichTextDynamicTag(str, "voffset");
        str = RemoveRichTextDynamicTag(str, "width");
     
        str = RemoveRichTextTag(str, "u");
        str = RemoveRichTextTag(str, "s");
        str = RemoveRichTextTag(str, "sup");
        str = RemoveRichTextTag(str, "sub");
        str = RemoveRichTextTag(str, "allcaps");
        str = RemoveRichTextTag(str, "smallcaps");
        str = RemoveRichTextTag(str, "uppercase");
        
        return str;
    }
    
    private static string RemoveRichTextDynamicTag (string str, string tag)
    {
        int index = -1;
        while (true)
        {
            index = str.IndexOf($"<{tag}=");
            if (index != -1)
            {
                int endIndex = str.Substring(index, str.Length - index).IndexOf('>');
                if (endIndex > 0)
                    str = str.Remove(index, endIndex + 1);
                continue;
            }
            str = RemoveRichTextTag(str, tag, false);
            return str;
        }
    }
    private static string RemoveRichTextTag (string str, string tag, bool isStart = true)
    {
        while (true)
        {
            int index = str.IndexOf(isStart ? $"<{tag}>" : $"</{tag}>");
            if (index != -1)
            {
                str = str.Remove(index, 2 + tag.Length + (!isStart).GetHashCode());
                continue;
            }
            if (isStart)
                str = RemoveRichTextTag(str, tag, false);
            return str;
        }
    }
    
    public static string EncryptString(string plainText, string password)
    {
        string privateKey = "hgfedcba";
        byte[] privateKeyByte = Encoding.UTF8.GetBytes(privateKey);
        byte[] keyByte = Encoding.UTF8.GetBytes(password);
        byte[] inputtextbyteArray = Encoding.UTF8.GetBytes(plainText);
        using (DESCryptoServiceProvider dsp = new DESCryptoServiceProvider())
        {
            var memstr = new MemoryStream();
            var crystr = new CryptoStream(memstr, dsp.CreateEncryptor(keyByte, privateKeyByte), CryptoStreamMode.Write);
            crystr.Write(inputtextbyteArray, 0, inputtextbyteArray.Length);
            crystr.FlushFinalBlock();
            return Convert.ToBase64String(memstr.ToArray());
        }
    }
    
    public static string DecryptString(string encrypted, string password)
    {
        try
        {
            string privateKey = "hgfedcba";
            byte[] privateKeyByte  = Encoding.UTF8.GetBytes(privateKey);
            byte[] keyByte = Encoding.UTF8.GetBytes(password);
            byte[] inputtextbyteArray = Convert.FromBase64String(encrypted.Replace(" ", "+"));
            using (DESCryptoServiceProvider dEsp = new DESCryptoServiceProvider())
            {
                var memstr = new MemoryStream();
                var crystr = new CryptoStream(memstr, dEsp.CreateDecryptor(keyByte, privateKeyByte), CryptoStreamMode.Write);
                crystr.Write(inputtextbyteArray, 0, inputtextbyteArray.Length);
                crystr.FlushFinalBlock();
                return Encoding.UTF8.GetString(memstr.ToArray());
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    // public static void CreateAssetInFolder(UnityEngine.Object newAsset, string ParentFolder, string AssetName )
    // {
    //     string[] pathSegments = ParentFolder.Split( new char[] {'/'});
    //     string accumulatedUnityFolder = "Assets";
    //     string accumulatedSystemFolder = Application.dataPath + System.IO.Path.GetDirectoryName("Assets" );
    //     foreach( string folder in pathSegments )
    //     {
    //         if (!System.IO.Directory.Exists( accumulatedSystemFolder + System.IO.Path.GetDirectoryName( accumulatedUnityFolder + "/" + folder )))
    //             AssetDatabase.CreateFolder( accumulatedUnityFolder, folder );
    //         accumulatedSystemFolder += "/"+folder;
    //         accumulatedUnityFolder += "/"+folder;
    //     }
    //     
    //     AssetDatabase.CreateAsset (newAsset, "Assets/"+ParentFolder+"/"+AssetName+".asset");
    // }
    
    public static string ReplaceFirstOccurrence(string source, string find, string replace)
    {
        int place = source.IndexOf(find);
    
        if (place == -1)
            return source;
    
        return source.Remove(place, find.Length).Insert(place, replace);
    }
    
    public static string ReplaceLastOccurrence(string source, string find, string replace)
    {
        int place = source.LastIndexOf(find);
    
        if (place == -1)
            return source;
    
        return source.Remove(place, find.Length).Insert(place, replace);
    }
}