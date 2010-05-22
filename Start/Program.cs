﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.Windows.Forms;
using System.Net;

namespace Start
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                RunLink(args[0]);
            }
            else
            {
                var f = new ManicDigger.ServerSelector();
                System.Windows.Forms.Application.Run(f);
                System.Windows.Forms.Application.Exit();
                if (File.Exists("tmp.mdlink"))
                {
                    File.Delete("tmp.mdlink");
                }
                if (f.SelectedServer != null)
                {
                    try
                    {
                        new WebClient().DownloadFile("http://fragmer.net/md/play.php?server=" + f.SelectedServer, "tmp.mdlink");
                        RunLink("tmp.mdlink");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }
        }
        private static void RunLink(string filename)
        {
            string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            if (filename.EndsWith(".mdlink", StringComparison.InvariantCultureIgnoreCase))
            {
                XmlDocument d = new XmlDocument();
                d.Load(filename);
                string mode = XmlTool.XmlVal(d, "/ManicDiggerLink/GameMode");
                if (mode.Equals("Fortress", StringComparison.InvariantCultureIgnoreCase))
                {
                    Process.Start(Path.Combine(appPath, "GameModeFortress"), "\"" + filename + "\"");
                }
                else if (mode.Equals("Mine", StringComparison.InvariantCultureIgnoreCase))
                {
                    Process.Start(Path.Combine(appPath, "GameModeMine"), "\"" + filename + "\"");
                }
                else
                {
                    throw new Exception("Invalid game mode: " + mode);
                }
            }
        }
    }
    public class XmlTool
    {
        public static string XmlVal(XmlDocument d, string path)
        {
            XPathNavigator navigator = d.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select(path);
            foreach (XPathNavigator n in iterator)
            {
                return n.Value;
            }
            return null;
        }
        public static IEnumerable<string> XmlVals(XmlDocument d, string path)
        {
            XPathNavigator navigator = d.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select(path);
            foreach (XPathNavigator n in iterator)
            {
                yield return n.Value;
            }
        }
    }
}
