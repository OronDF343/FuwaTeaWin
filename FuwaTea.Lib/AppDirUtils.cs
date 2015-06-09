﻿using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace FuwaTea.Lib
{
    public static class AppDirUtils
    {
        /// <summary>
        /// Gets the path of the <see cref="Assembly"/>'s executable file.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string GetExeFolder(this Assembly a)
        {
            var exeFile = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(exeFile);
        }

        public static bool IsInstalledCopy(this Assembly a, string company = null, string appName = null)
        {
            // Get info about the assembly
            var exeDir = a.GetExeFolder();
            var com = company ?? ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(a, typeof(AssemblyCompanyAttribute), false)).Company;
            var product = appName ?? ((AssemblyProductAttribute)Attribute.GetCustomAttribute(a, typeof(AssemblyProductAttribute), false)).Product;
            // Check if a copy is installed
            var key = Registry.LocalMachine.OpenSubKey(string.Format(@"Software\{0}\{1}", com, product));
            // If no key exists, it isn't installed
            if (key == null) return false;
            // Get location installed to
            var loc = key.GetValue("InstallLocation");
            // Return if this is the installed copy
            return ((string)loc).Trim('\"', '\\').Equals(exeDir.TrimEnd('\\'));
        }

        /// <summary>
        /// Gets a path to the location where user data should be stored for the <see cref="Assembly"/>.
        /// </summary>
        /// <remarks>This requires the installer to set the value InstallLocation in the key HKLM\Software\Company\Product.</remarks>
        /// <param name="a"></param>
        /// <param name="company">Optionally override the company name.</param>
        /// <param name="appName">Optionally override the product name.</param>
        /// <returns></returns>
        public static string GetUserDataPath(this Assembly a, string company = null, string appName = null)
        {
            string userDataDir;
            // Get info about the assembly
            var exeDir = a.GetExeFolder();
            var com = company ?? ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(a, typeof(AssemblyCompanyAttribute), false)).Company;
            var product = appName ?? ((AssemblyProductAttribute)Attribute.GetCustomAttribute(a, typeof(AssemblyProductAttribute), false)).Product;
            // Check if installed
            if (IsInstalledCopy(a, company, appName))
            {
                // Ensure the directory in %AppData% exists
                userDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                           com, product);
            }
            else
            {
                // Otherwise, this is a portable copy
                userDataDir = Path.Combine(exeDir, "userdata");
                if (!Directory.Exists(userDataDir)) Directory.CreateDirectory(userDataDir);
            }
            return userDataDir;
        }

        public static string GetCompany(this Assembly a)
        {
            return ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(a, typeof(AssemblyCompanyAttribute), false)).Company;
        }

        public static string GetProduct(this Assembly a)
        {
            return ((AssemblyProductAttribute)Attribute.GetCustomAttribute(a, typeof(AssemblyProductAttribute), false)).Product;
        }
    }
}
