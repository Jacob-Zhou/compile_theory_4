using compile_theory_4.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compile_theory_4.Model
{
	class Detail
	{
		public Detail(string kind, string production)
		{
			this.kind = kind;
			this.production = production;
		}

		public string kind { get; set; }
		public string production { get; set; }
	}

	class Process : INotifyPropertyChanged
	{
		public Process(string kind, string production)
		{
			this.kind = kind;
			this.production = production;
			detail = new ObservableCollection<Process>();
			//detail.Add(new Process(production));
		}

		public Process(string kind)
		{
			this.kind = kind;
		}

		public void addDetail(Process process)
		{
			detail.Add(process);
			OnPropertyChanged(new PropertyChangedEventArgs("detail"));
		}

		private bool IsExpanded;
		public bool isExpanded
		{
			get { return IsExpanded; }
			set
			{
				IsExpanded = value;
				if(detail != null)
				{
					foreach(var d in detail)
					{
						d.isExpanded = ProcessViewModel.IsExpanded;
					}
				}
				OnPropertyChanged(new PropertyChangedEventArgs("isExpanded"));
			}
		}

		private string Kind;
		public string kind
		{
			get { return Kind; }
			set
			{
				Kind = value;
				OnPropertyChanged(new PropertyChangedEventArgs("kind"));
			}
		}

		private string Production;
		public string production
		{
			get { return Production; }
			set
			{
				Production = value;
				OnPropertyChanged(new PropertyChangedEventArgs("production"));
			}
		}

		private ObservableCollection<Process> Detail;
		public ObservableCollection<Process> detail
		{
			get { return Detail; }
			set
			{
				Detail = value;
				OnPropertyChanged(new PropertyChangedEventArgs("detail"));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}
	}
}
