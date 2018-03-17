//----------------------------------------------------------------------------
//
// THIS WORK IS PROVIDED "AS IS", "WHERE IS" AND "AS AVAILABLE", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED OR CONDITIONS OR GUARANTEES.
// YOU, THE USER, ASSUME ALL RISK IN ITS USE, INCLUDING COPYRIGHT INFRINGEMENT, PATENT INFRINGEMENT, SUITABILITY, ETC.
// AUTHOR EXPRESSLY DISCLAIMS ALL EXPRESS, IMPLIED OR STATUTORY WARRANTIES OR CONDITIONS, INCLUDING WITHOUT LIMITATION,
// WARRANTIES OR CONDITIONS OF MERCHANTABILITY, MERCHANTABLE QUALITY OR FITNESS FOR A PARTICULAR PURPOSE,
// OR ANY WARRANTY OF TITLE OR NON-INFRINGEMENT, OR THAT THE WORK (OR ANY PORTION THEREOF) IS CORRECT, USEFUL, BUG-FREE OR FREE OF VIRUSES.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE
//
//----------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using JetBrains.Annotations;

namespace FuwaTea.Wpf.Controls
{
	public class CustomDataGridTemplateColumn : DataGridTemplateColumn
	{
		private BindingBase _binding;

		protected virtual void OnBindingChanged(BindingBase oldBinding, BindingBase newBinding)
		{
			NotifyPropertyChanged("Binding");
		}

		public virtual BindingBase Binding
		{
			get => _binding;
		    set
			{
			    if (_binding == value) return;
			    var oldBinding = _binding;
			    _binding = value;
			    CoerceValue(SortMemberPathProperty);
			    OnBindingChanged(oldBinding, _binding);
			}
		}

		public override BindingBase ClipboardContentBinding
		{
			get => base.ClipboardContentBinding ?? Binding;
		    set => base.ClipboardContentBinding = value;
		}

        [CanBeNull]
        private DataTemplate ChooseCellTemplate(bool isEditing)
		{
		    return isEditing ? CellEditingTemplate ?? CellTemplate : CellTemplate;
		}

        [CanBeNull]
		private DataTemplateSelector ChooseCellTemplateSelector(bool isEditing)
		{
            return isEditing ? CellEditingTemplateSelector ?? CellTemplateSelector : CellTemplateSelector;
        }

		protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
		{
			return LoadTemplateContent(true, dataItem, cell);
		}

		protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
		{
			return LoadTemplateContent(false, dataItem, cell);
		}

		private void ApplyBinding(DependencyObject target, DependencyProperty property)
		{
			var binding = Binding;
		    BindingOperations.SetBinding(target, property, binding ?? new Binding());
		}

        [CanBeNull]
        private FrameworkElement LoadTemplateContent(bool isEditing, object dataItem, DataGridCell cell)
		{
			var template = ChooseCellTemplate(isEditing);
			var templateSelector = ChooseCellTemplateSelector(isEditing);
		    if ((template == null) && (templateSelector == null)) return null;
		    var contentPresenter = new ContentPresenter();
			ApplyBinding(contentPresenter, ContentPresenter.ContentProperty);
			contentPresenter.ContentTemplate = template;
			contentPresenter.ContentTemplateSelector = templateSelector;
			return contentPresenter;
		}
	}
}
