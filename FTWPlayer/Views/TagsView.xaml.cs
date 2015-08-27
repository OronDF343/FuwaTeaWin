using FTWPlayer.ViewModels;

namespace FTWPlayer.Views
{
    /// <summary>
    /// Interaction logic for TagsView.xaml
    /// </summary>
    public partial class TagsView
    {
        public TagsView(TagsViewModel tvm)
        {
            DataContext = tvm;
            InitializeComponent();
        }
    }
}
