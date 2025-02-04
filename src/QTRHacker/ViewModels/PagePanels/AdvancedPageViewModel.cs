﻿using QTRHacker.ViewModels.Advanced;
using System.Collections.ObjectModel;
using System.Reflection;

namespace QTRHacker.ViewModels.PagePanels;

public class AdvancedPageViewModel : PagePanelViewModel
{
	public ObservableCollection<AdvancedFunction> Functions { get; } = new();

	public AdvancedPageViewModel()
	{
		var afType = typeof(AdvancedFunction);
		string ns = afType.Namespace;
		var types = Assembly.GetExecutingAssembly().DefinedTypes.Where(t => t.Namespace?.StartsWith(ns) == true && t.IsSubclassOf(afType));
		foreach (var type in types)
		{
			if (Activator.CreateInstance(type) is not AdvancedFunction obj)
				continue;
			Functions.Add(obj);
		}
	}
}
