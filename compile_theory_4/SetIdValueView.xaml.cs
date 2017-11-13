using compile_theory_4.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace compile_theory_4
{
	/// <summary>
	/// SetIdValueView.xaml 的交互逻辑
	/// </summary>
	public partial class SetIdValueView : Window
	{
		public SetIdValueView()
		{
			InitializeComponent();
		}

		static public void SetValue()
		{
			SetIdValueView svv = new SetIdValueView();
			svv.ShowDialog();
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			//Close();
		}
	}
}
