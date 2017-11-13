﻿using compile_theory_4.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace compile_theory_4.ViewModel
{
	class ProcessViewModel
	{
		private static ObservableCollection<Process> Processes = new ObservableCollection<Process>();
		private static TreeView processesTreeView;
		private static bool isExpanded = false;

		public static bool IsExpanded
		{
			get
			{
				return isExpanded;
			}
		}

		public static void ChangeMode()
		{
			if (isExpanded)
			{
				isExpanded = false;
			}
			else
			{
				isExpanded = true;
			}
			for(int i = 0; i < Processes.Count; ++i)
			{
				Processes[i].isExpanded = isExpanded;
			}
		}

		public static void Init(TreeView ProcessesTreeView)
		{
			processesTreeView = ProcessesTreeView;
			if (processesTreeView != null)
			{
				processesTreeView.ItemsSource = Processes;
			}
		}

		public static void Add(Process process)
		{
			process.isExpanded = isExpanded;
			Processes.Add(process);
		}

		public static void Clear()
		{
			Processes.Clear();
		}
	}
}
