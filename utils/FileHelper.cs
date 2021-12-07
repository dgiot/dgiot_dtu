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
    }
}