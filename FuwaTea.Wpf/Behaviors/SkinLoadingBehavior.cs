using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Markup;
using log4net;

namespace FuwaTea.Wpf.Behaviors
{
    [ContentProperty("DictionaryIds")]
    public class SkinLoadingBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty DictionaryIdsProperty =
            DependencyProperty.Register("DictionaryIds", typeof(string), typeof(SkinLoadingBehavior));

        public string DictionaryIds
        {
            get { return (string)GetValue(DictionaryIdsProperty); }
            set { SetValue(DictionaryIdsProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            Elements.Add(this);
            UpdateSkinForElement(this);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            Elements.Remove(this);
        }

        private static readonly List<SkinLoadingBehavior> Elements = new List<SkinLoadingBehavior>();
        private static Dictionary<string, ResourceDictionary> _skinDictionaries;

        public static void UpdateSkin(Dictionary<string, ResourceDictionary> skinDictionaries)
        {
            _skinDictionaries = skinDictionaries;
            foreach (var element in Elements)
                UpdateSkinForElement(element);
        }

        private static void UpdateSkinForElement(SkinLoadingBehavior element)
        {
            element.AssociatedObject.Resources.MergedDictionaries.Clear();
            foreach (var id in element.DictionaryIds.ToLowerInvariant().Split(';'))
            {
                try
                {
                    element.AssociatedObject.Resources.MergedDictionaries.Add(_skinDictionaries[id]);
                }
                catch (Exception e)
                {
                    LogManager.GetLogger(typeof(SkinLoadingBehavior))
                              .Error($"Failed to load required skin part \"{id}\" for element \"{element.AssociatedObject.Name}\"", e);
                }
            }
        }
    }
}
