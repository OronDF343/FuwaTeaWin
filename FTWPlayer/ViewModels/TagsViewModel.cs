using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FTWPlayer.Views;
using FuwaTea.Metadata.Tags;
using FuwaTea.Playback;
using FuwaTea.Wpf.Helpers;
using GalaSoft.MvvmLight.CommandWpf;
using ModularFramework;

namespace FTWPlayer.ViewModels
{
    [UIPart("Tag editor")]
    public class TagsViewModel : ITab, INotifyPropertyChanged
    {
        public TagsViewModel()
        {
            DiscardChangesCommand = new RelayCommand<RoutedEventArgs>(DiscardChanges);
            SaveChangesCommand = new RelayCommand<RoutedEventArgs>(SaveChanges);
            DeleteTagsCommand = new RelayCommand<RoutedEventArgs>(DeleteTags);
            _playbackManager.PropertyChanged +=
                (sender, args) =>
                {
                    if (args.PropertyName == nameof(IPlaybackManager.Current))
                        OnPropertyChanged(nameof(Tag));
                };
            TabObject = new TagsView(this);
        }

        private readonly IPlaybackManager _playbackManager = ModuleFactory.GetElement<IPlaybackManager>();

        public TabItem TabObject { get; }
        public decimal Index => 0.5m;
        public Tag Tag => _playbackManager.Current?.Tag;

        public ICommand DiscardChangesCommand { get; set; }

        private void DiscardChanges(RoutedEventArgs e)
        {
            (TabObject as TagsView).TagGrid.UpdateBindingTargets(TextBox.TextProperty);
        }

        public ICommand SaveChangesCommand { get; set; }

        private void SaveChanges(RoutedEventArgs e)
        {
            (TabObject as TagsView).TagGrid.UpdateBindingSources(TextBox.TextProperty);
            Tag.SaveTags();
        }

        public ICommand DeleteTagsCommand { get; set; }

        private void DeleteTags(RoutedEventArgs e)
        {
            var res = MessageBox.Show(Application.Current.MainWindow, "Are you sure?", "Confirm tag deletion",
                                      MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res != MessageBoxResult.Yes) return;
            Tag.Clear();
            Tag.SaveTags();
            (TabObject as TagsView).TagGrid.UpdateBindingTargets(TextBox.TextProperty);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
