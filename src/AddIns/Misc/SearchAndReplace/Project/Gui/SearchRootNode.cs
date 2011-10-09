﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.Search;

namespace SearchAndReplace
{
	sealed class SearchRootNode : SearchNode
	{
		ObservableCollection<SearchResultNode> resultNodes;
		ObservableCollection<SearchFileNode> fileNodes;
		
		public string Title { get; private set; }
		
		public SearchRootNode(string title, IList<SearchResultMatch> results)
		{
			this.Title = title;
			this.resultNodes = new ObservableCollection<SearchResultNode>(results.Select(r => new SearchResultNode(r)));
			this.fileNodes = new ObservableCollection<SearchFileNode>(resultNodes.GroupBy(r => r.FileName).Select(g => new SearchFileNode(g.Key, g.ToList())));
			this.IsExpanded = true;
		}

		public void Add(SearchResultMatch match)
		{
			var matchNode = new SearchResultNode(match);
			resultNodes.Add(matchNode);
			int index = fileNodes.FindIndex(node => node.FileName.Equals(match.FileName));
			if (index == -1)
				fileNodes.Add(new SearchFileNode(match.FileName, new List<SearchResultNode> { matchNode }));
			else {
				fileNodes[index].Children = resultNodes.Where(node => node.FileName.Equals(match.FileName)).ToArray();
				fileNodes[index].InvalidateText();
			}
			InvalidateText();
		}
		
		public void GroupResultsByFile(bool perFile)
		{
			if (perFile)
				this.Children = fileNodes;
			else
				this.Children = resultNodes;
			foreach (SearchResultNode node in resultNodes) {
				node.ShowFileName = !perFile;
			}
		}
		
		public int Occurrences {
			get { return resultNodes.Count; }
		}
		
		protected override object CreateText()
		{
			return new TextBlock {
				Inlines = {
					new Bold(new Run(this.Title)),
					new Run(" (" + GetOccurrencesString(resultNodes.Count)
					        + StringParser.Parse(" ${res:MainWindow.Windows.SearchResultPanel.In} ")
					        + GetFileCountString(fileNodes.Count) + ")")
				}
			};
		}
		
		public static string GetOccurrencesString(int count)
		{
			if (count == 1) {
				return StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.OneOccurrence}");
			} else {
				return StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.OccurrencesCount}",
				                          new StringTagPair("Count", count.ToString()));
			}
		}
		
		public static string GetFileCountString(int count)
		{
			if (count == 1) {
				return StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.OneFile}");
			} else {
				return StringParser.Parse("${res:MainWindow.Windows.SearchResultPanel.FileCount}",
				                          new StringTagPair("Count", count.ToString()));
			}
		}
	}
}
