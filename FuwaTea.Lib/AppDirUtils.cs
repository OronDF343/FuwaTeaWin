#region License
//     This file is part of FuwaTeaWin.
// 
//     FuwaTeaWin is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     FuwaTeaWin is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with FuwaTeaWin.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
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
            var exeFile = a.Location;
            return Path.GetDirectoryName(exeFile);
        }

        public static bool IsInstalledCopy(this Assembly a, string company = null, string appName = null)
        {
            // Get info about the assembly
            var exeDir = a.GetExeFolder();
            var com = company ?? ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(a, typeof(AssemblyCompanyAttribute), false)).Company;
            var product = appName ?? ((AssemblyProductAttribute)Attribute.GetCustomAttribute(a, typeof(AssemblyProductAttribute), false)).Product;
            // Check if a copy is installed
            var key = Registry.LocalMachine.OpenSubKey($@"Software\{com}\{product}");
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
