using System.Collections.Generic;

namespace Instances
{
	public class Figure
	{
		public string title;
		public Dictionary<string, string> figureItems;

		public Figure(string title, Dictionary<string, string> figureItems)
		{
			this.title = title;
			this.figureItems = figureItems;
		}
	}
}