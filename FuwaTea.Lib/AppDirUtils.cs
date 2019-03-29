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
using System.Security;
using JetBrains.Annotations;
using Microsoft.Win32;
using Sage.Extensibility;
using Serilog;

namespace FuwaTea.Lib
{
    /// <summary>
    /// Utilities for applications which have installed and portable versions.
    /// </summary>
    public static class AppDirUtils
    {
        /// <summary>
        /// Gets the path of the <see cref="Assembly"/>'s executable file.
        /// </summary>
        /// <param name="a">The <see cref="Assembly"/> object</param>
        /// <returns>The path to the executable file</returns>
        [CanBeNull]
        public static string GetExeFolder(this Assembly a)
        {
            var exeFile = a.Location;
            return Path.GetDirectoryName(exeFile);
        }

        /// <summary>
        /// Checks whether the <see cref="Assembly"/> copy was installed on this system with an installer.
        /// </summary>
        /// <remarks>
        /// Checks if the registry value HKLM\Software\(company name)\(product name)\InstallLocation contains the path to the assembly.
        /// The company name and product name are taken from the attributes of the <see cref="Assembly"/> by default.
        /// </remarks>
        /// <param name="a">The <see cref="Assembly"/> object</param>
        /// <param name="company">Optionally overrides the company name</param>
        /// <param name="appName">Optionally overrides the product name</param>
        /// <returns>True if this copy was installed</returns>
        public static bool IsInstalledCopy(this Assembly a, string company = null, string appName = null)
        {
            // Get info about the assembly
            var exeDir = a.GetExeFolder();
            if (exeDir == null) return false;
            var com = company ?? a.GetCompany();
            var product = appName ?? a.GetProduct();
            try
            {
                // Check if a copy is installed
                var key = Registry.LocalMachine.OpenSubKey($@"Software\{com}\{product}");
                // If no key exists, it isn't installed
                if (key == null) return false;
                // Get location installed to
                var loc = key.GetValue("InstallLocation");
                // Return if this is the installed copy
                return ((string)loc).Trim('\"', '\\').Equals(exeDir.TrimEnd('\\'));
            }
            catch (SecurityException se)
            {
                Log.ForContext(typeof(AppDirUtils)).Error("I'm missing permissions for the registry key! Unable to determine if this is an installed copy!", se);
                return false;
            }
            catch (IOException ie)
            {
                Log.ForContext(typeof(AppDirUtils)).Warning("Registry key marked for deletion. Assuming that the installed version was uninstalled", ie);
                return false;
            }
            catch (UnauthorizedAccessException uae)
            {
                Log.ForContext(typeof(AppDirUtils)).Error("I don't have read access rights to the registry key! Unable to determine if this is an installed copy!", uae);
                return false;
            }
        }

        /// <summary>
        /// Gets a path to the location where user data should be stored for the <see cref="Assembly"/>.
        /// </summary>
        /// <remarks>This requires the installer to set the value InstallLocation in the key HKLM\Software\Company\Product.</remarks>
        /// <param name="a"></param>
        /// <param name="company">Optionally override the company name.</param>
        /// <param name="appName">Optionally override the product name.</param>
        /// <returns></returns>
        /// <exception cref="IOException">The userdata directory is a file, or the network name is not known.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission to access the userdata directory. </exception>
        /// <exception cref="DirectoryNotFoundException">The userdata directory path is invalid (for example, it is on an unmapped drive). </exception>
        public static string GetUserDataPath(this Assembly a, string company = null, string appName = null)
        {
            string userDataDir;
            // Get info about the assembly
            var com = company ?? a.GetCompany();
            var product = appName ?? a.GetProduct();
            // Check if installed
            if (IsInstalledCopy(a, company, appName))
            {
                // Ensure the directory in %AppData% exists
                userDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                           com, product);
            }
            else
            {
                // Otherwise, this is a portable copy
                userDataDir = a.GetSpecificPath(false, "userdata", true);
            }
            return userDataDir;
        }

        /// <summary>
        /// Gets the company name set on an <see cref="Assembly"/>
        /// </summary>
        /// <param name="a">The <see cref="Assembly"/></param>
        /// <returns>The company name</returns>
        public static string GetCompany(this Assembly a)
        {
            return a.GetAttribute<AssemblyCompanyAttribute>(false).Company;
        }

        /// <summary>
        /// Gets the product name set on an <see cref="Assembly"/>
        /// </summary>
        /// <param name="a">The <see cref="Assembly"/></param>
        /// <returns>The product name</returns>
        public static string GetProduct(this Assembly a)
        {
            return a.GetAttribute<AssemblyProductAttribute>(false).Product;
        }

        /// <summary>
        /// Get a special path relative to the location of the <see cref="Assembly"/>.
        /// </summary>
        /// <param name="a">The <see cref="Assembly"/> object</param>
        /// <param name="isUserData">Specifies whether this directory should be in the userdata location rather than the <see cref="Assembly"/>'s location.</param>
        /// <param name="name">The relative path of the directory</param>
        /// <param name="createDirIfMissing">Specifies whether to create the directory if it doesn't exist yet</param>
        /// <returns>An absolute path to the directory</returns>
        /// <exception cref="IOException">The directory specified by <paramref name="name" /> is a file, or the network name is not known.</exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="DirectoryNotFoundException">The directory path is invalid (for example, it is on an unmapped drive). </exception>
        public static string GetSpecificPath(this Assembly a, bool isUserData, string name, bool createDirIfMissing)
        {
            var ed = a.GetExeFolder();
            if (!isUserData && ed == null) throw new DirectoryNotFoundException("Can't find the location of the specified assembly!");
            var p = Path.Combine(isUserData ? a.GetUserDataPath() : ed, name);
            if (createDirIfMissing && !Directory.Exists(p)) Directory.CreateDirectory(p); // TODO: What if admin rights are needed?
            return p;
        }
    }
}
