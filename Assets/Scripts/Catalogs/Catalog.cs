using System.Collections.Generic;
using Custom;
using UnityEngine;
using Action = Instances.Action;

namespace Catalogs
{
	// public class Catalog : IGeneralCatalogInterface, IActionsCatalog3DInterface, IKnowledgeCatalogInterface
	public class Catalog : IGeneralCatalogInterface, IKnowledgeCatalogInterface, IActionsCatalog3DInterface
	{
		private static Catalog _instance;
		private readonly ActionsCatalog3D _actionsCatalog3D;
		private readonly GeneralCatalog _generalCatalog;
		private readonly KnowledgeCatalog _knowledgeCatalog;

		private Catalog()
		{
			_generalCatalog = new GeneralCatalog();
			_actionsCatalog3D = new ActionsCatalog3D();
			_knowledgeCatalog = new KnowledgeCatalog();
		}

		public static Catalog Instance => _instance ??= new Catalog();

		public void Reset(string args)
		{
			_actionsCatalog3D.Reset(args);
		}

		public void Highlight(string args)
		{
			_actionsCatalog3D.Highlight(args);
		}

		public void Rotate(string args)
		{
			_actionsCatalog3D.Rotate(args);
		}

		public void Scale(string args)
		{
			_actionsCatalog3D.Scale(args);
		}

		public void ShowSide(string args)
		{
			_actionsCatalog3D.ShowSide(args);
		}

		public void SideBySideLook(string args)
		{
			_actionsCatalog3D.SideBySideLook(args);
		}

		public void CloseLook(string args)
		{
			_actionsCatalog3D.CloseLook(args);
		}

		public void Animate(string args)
		{
			_actionsCatalog3D.Animate(args);
		}

		public void Visibility(string args)
		{
			_actionsCatalog3D.Visibility(args);
		}

		public void Attach(string args)
		{
			_actionsCatalog3D.Attach(args);
		}

		public void Detach(string args)
		{
			_actionsCatalog3D.Detach(args);
		}

		public List<Action> CreateActions(string args)
		{
			return _actionsCatalog3D.CreateActions(args);
		}

		public string CheckActionsValidity(string args)
		{
			return _actionsCatalog3D.CheckActionsValidity(args);
		}

		public List<GameObject> Filter3DAttr(string args)
		{
			return _actionsCatalog3D.Filter3DAttr(args);
		}

		public object SaveVal2Var(string args)
		{
			return _generalCatalog.SaveVal2Var(args);
		}

		public int Count(object[] objects)
		{
			return _generalCatalog.Count(objects);
		}

		public bool Exist(object[] objects)
		{
			return _generalCatalog.Exist(objects);
		}

		public System.Object Unique(string args)
		{
			return _generalCatalog.Unique(args);
		}

		public List<string> ExtractNumbers(string value)
		{
			return _generalCatalog.ExtractNumbers(value);
		}

		public List<string> ExtractID(string args)
		{
			return _generalCatalog.ExtractID(args);
		}

		public string Same(string args)
		{
			return _generalCatalog.Same(args);
		}

		public JSONNode FilterAttr(string args)
		{
			return _knowledgeCatalog.FilterAttr(args);
		}

		public List<JSONNode> FilterType(string args)
		{
			return _knowledgeCatalog.FilterType(args);
		}

		public string QueryAttr(string args)
		{
			return _knowledgeCatalog.QueryAttr(args);
		}

		public string ShowInfo(List<JSONNode> dataObjects)
		{
			return _knowledgeCatalog.ShowInfo(dataObjects);
		}
	}
}