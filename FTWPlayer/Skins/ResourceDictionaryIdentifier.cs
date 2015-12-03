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

namespace FTWPlayer.Skins
{
    public class ResourceDictionaryIdentifier
    {
        /// <summary>
        /// Unique ID for this skin. Please use the format "author:name" to avoid duplicate IDs. THIS VALUE IS REQUIRED!
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// What is this skin called?
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The path to the parent skin which will be loaded before this one. Set if relevant.
        /// </summary>
        public string Parent { get; set; }
        /// <summary>
        /// What is this skin?
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Version number.
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Who made this?
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Where can this skin be found online?
        /// </summary>
        public string Homepage { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)} = {Id}";
        }
    }
}
