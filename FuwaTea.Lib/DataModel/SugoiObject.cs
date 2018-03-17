using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace FuwaTea.Lib.DataModel
{
    /// <summary>
    /// An awesome object :smile:
    /// </summary>
    /// <remarks>Identified by an <see cref="int"/></remarks>
    public abstract class SugoiObject : SugoiObject<int> { }

    /// <summary>
    /// An awesome object :smile:
    /// </summary>
    public abstract class SugoiObject<TKey> : IKeyedElement<TKey>, ICloneable, IEditableObject, INotifyPropertyChanged
    {
        private TKey _key;

        /// <summary>
        /// Gets the unique identifier for this object.
        /// Override me for XML serialization.
        /// WARNING: The setter only exists for deserialization! Changing the key could lead to unexpected results!
        /// </summary>
        [XmlIgnore]
        public virtual TKey Key
        {
            get => _key;
            set
            {
                if (Equals(_key, value)) return;
                _key = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Creates a copy of this object. Needs to be overriden in non-abstract derived classes.
        /// </summary>
        /// <returns>A copy of this object</returns>
        public abstract object Clone();

        /// <summary>
        /// A backup of this object created by <see cref="BeginEdit"/>
        /// </summary>
        [XmlIgnore]
        protected SugoiObject<TKey> Backup { get; set; }

        /// <summary>
        /// Begin editing this object
        /// </summary>
        public virtual void BeginEdit()
        {
            if (Backup != null) EndEdit();
            Backup = (SugoiObject<TKey>)Clone();
        }

        /// <summary>
        /// End editing and save changes
        /// </summary>
        public virtual void EndEdit()
        {
            if (Backup == null) return;
            Backup = null;
        }

        /// <summary>
        /// End editing and discard changes
        /// </summary>
        public virtual void CancelEdit()
        {
            if (Backup == null) return;
            RestoreValues(Backup);
            Backup = null;
        }

        /// <summary>
        /// Restore this object from a copy. Needs to be overriden in derived classes.
        /// </summary>
        /// <param name="so">The copy to restore from</param>
        public virtual void RestoreValues(SugoiObject<TKey> so)
        {
            Key = so.Key;
        }

        /// <summary>
        /// See <see cref="INotifyPropertyChanged"/>
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fires the <see cref="PropertyChanged"/> event
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
