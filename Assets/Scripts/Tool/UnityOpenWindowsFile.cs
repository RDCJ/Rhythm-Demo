using System;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class OpenFileName
{
    public int structSize = 0;
    public IntPtr dlgOwner = IntPtr.Zero;
    public IntPtr instance = IntPtr.Zero;
    public String filter = null;
    public String customFilter = null;
    public int maxCustFilter = 0;
    public int filterIndex = 0;
    public String file = null;
    public int maxFile = 0;
    public String fileTitle = null;
    public int maxFileTitle = 0;
    public String initialDir = null;
    public String title = null;
    public int flags = 0;
    public short fileOffset = 0;
    public short fileExtension = 0;
    public String defExt = null;
    public IntPtr custData = IntPtr.Zero;
    public IntPtr hook = IntPtr.Zero;
    public String templateName = null;
    public IntPtr reservedPtr = IntPtr.Zero;
    public int reservedInt = 0;
    public int flagsEx = 0;
}

public class LocalDialog
{
    //链接指定系统函数       打开文件对话框
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);
    public static bool GetOFN([In, Out] OpenFileName ofn)
    {
        return GetOpenFileName(ofn);
    }

    //链接指定系统函数        另存为对话框
    [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);
    public static bool GetSFN([In, Out] OpenFileName ofn)
    {
        return GetSaveFileName(ofn);
    }
}


public class UnityOpenWindowsFile
{
    public enum FileType
    {
        None,
        Text,
        Texture,
        Video,
        Music
    }

    public static bool OpenFIleDialog(FileType file_type, string open_path=null, Action<OpenFileName> callback=null)
    {
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        string filter = string.Empty;
        switch (file_type)
        {
            case FileType.None:
                filter = "All File(*.*)\0*.*\0\0";
                break;
            case FileType.Text:
                filter = "Text File(*文本文件)\0*.txt\0";
                break;
            case FileType.Texture:
                filter = "Texture File(*图片文件)\0*.png;*.jpg\0";
                break;
            case FileType.Video:
                filter = "Video File(*视频文件)\0*.mp4\0";
                break;
            case FileType.Music:
                filter = "Music File(*音频文件)\0*.wav;*.mp3\0";
                break;
        }
        ofn.filter = filter;
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        if (string.IsNullOrEmpty(open_path))
            ofn.initialDir = System.Environment.CurrentDirectory;
        else
            ofn.initialDir = open_path;
        ofn.title = "选择文件";
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        if (LocalDialog.GetOFN(ofn))
        {
            Debug.Log(ofn.file);
            callback?.Invoke(ofn);
            return true;
        }
        return false;
    }
}
