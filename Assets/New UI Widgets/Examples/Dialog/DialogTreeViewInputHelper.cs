namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// DialogTreeViewInputHelper
	/// </summary>
	public class DialogTreeViewInputHelper : MonoBehaviour
	{
		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		public TreeView Folders;

		ObservableList<TreeNode<TreeViewItem>> nodes;

		bool isInited;

		/// <summary>
		/// Init.
		/// </summary>
		public void Refresh()
		{
			if (isInited)
			{
				return;
			}

			nodes = TestTreeView.GenerateTreeNodes(new int[] { 5, 5, 2 }, isExpanded: true);

			// Set nodes
			Folders.Init();
			Folders.Nodes = nodes;

			isInited = true;
		}
	}
}