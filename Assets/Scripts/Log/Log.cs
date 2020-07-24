using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
public static class Log
{
    public static string time;
    public static string resoultLog;
    public static void Debugs (string text)
    {
        if (time == null)
        {
            time = System.DateTime.Now.ToString("yy-MM-dd-HH-mm").Trim();
            Application.quitting += Quit;
        }
        if (File.Exists(Application.dataPath + "/Log/log " + time + ".txt"))
        {
            resoultLog += text + "\n";
        }
        else
        {
            File.Create(Application.dataPath + "/Log/log " + time + ".txt").Close();
            Debugs(text);
        }
    }

    public static void Quit ()
    {
        using (FileStream file = File.Open(Application.dataPath + "/Log/log " + time + ".txt", FileMode.Open))
        {
            resoultLog = string.Format($"({System.DateTime.Now}): {resoultLog}\n");
            for (int i = 0; i < file.Length; i++)
            {
                file.ReadByte();
            }
            file.Write(Encoding.UTF8.GetBytes(resoultLog), 0, Encoding.UTF8.GetBytes(resoultLog).Length);
        }
    }
}
