// <copyright file="FileHelper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Dgiot_dtu
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Windows.Forms;

    public class FileHelper
    {
        private FileHelper()
        {
        }

        private static FileHelper instance = null;
        private static OpenFileDialog openFileDialog = null;

        public static FileHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new FileHelper();
            }

            return instance;
        }

        public static void Init(OpenFileDialog openFileDialog)
        {
            FileHelper.openFileDialog = openFileDialog;
        }

        public static void Config(KeyValueConfigurationCollection config)
        {
        }

        public static List<string> OpenFile()
        {
            List<string> files = new List<string>();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                string filename = openFileDialog.FileName;
                StreamReader sr = new StreamReader(filename);
                LogHelper.Log("filename " + filename);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    files.Add(line);
                    LogHelper.Log("line " + line);
                }

                sr.Close();
            }

            return files;
        }

        /// <summary>
        /// 替换值
        /// </summary>
        /// <param name="strFilePath">txt等文件的路径</param>
        /// <param name="strIndex">索引的字符串，定位到某一行</param>
        /// <param name="newValue">替换新值</param>
        public static void ReplaceValue(string strFilePath, string strIndex, string newValue)
        {
            if (File.Exists(strFilePath))
            {
                string[] lines = System.IO.File.ReadAllLines(strFilePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(strIndex))
                    {
                        string[] str = lines[i].Split('=');
                        str[1] = newValue;
                        lines[i] = str[0] + " = " + str[1];
                    }
                }
                File.WriteAllLines(strFilePath, lines);
            }
        }
    }
}