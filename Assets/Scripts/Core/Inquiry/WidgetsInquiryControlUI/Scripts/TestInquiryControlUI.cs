namespace UIWidgets.Custom.InquiryControlUINS
{
	/// <summary>
	/// Test script for the InquiryControlUI.
	/// </summary>
	public partial class TestInquiryControlUI : UIWidgets.WidgetGeneration.TestBase<InquiryControlUI>
	{
		/// <summary>
		/// Left ListView.
		/// </summary>
		public ListViewInquiryControlUI LeftListView;

		/// <summary>
		/// Right ListView.
		/// </summary>
		public ListViewInquiryControlUI RightListView;

		/// <summary>
		/// TileView.
		/// </summary>
		public ListViewInquiryControlUI TileView;

		/// <summary>
		/// TreeView.
		/// </summary>
		public TreeViewInquiryControlUI TreeView;

		/// <summary>
		/// Table.
		/// </summary>
		public ListViewInquiryControlUI Table;

		/// <summary>
		/// TreeGraph.
		/// </summary>
		public TreeGraphInquiryControlUI TreeGraph;

		/// <summary>
		/// Autocomplete.
		/// </summary>
		public AutocompleteInquiryControlUI Autocomplete;

		/// <summary>
		/// AutoCombobox.
		/// </summary>
		public AutoComboboxInquiryControlUI AutoCombobox;

		/// <summary>
		/// Combobox.
		/// </summary>
		public ComboboxInquiryControlUI Combobox;

		/// <summary>
		/// ComboboxMultiselect.
		/// </summary>
		public ComboboxInquiryControlUI ComboboxMultiselect;

		/// <summary>
		/// ListView picker.
		/// </summary>
		public PickerListViewInquiryControlUI PickerListView;

		/// <summary>
		/// TreeView picker.
		/// </summary>
		public PickerTreeViewInquiryControlUI PickerTreeView;

		UIWidgets.ObservableList<InquiryControlUI> pickerListViewData;

		UIWidgets.ObservableList<UIWidgets.TreeNode<InquiryControlUI>> pickerTreeViewNodes;

		/// <summary>
		/// Init.
		/// </summary>
		public void Start()
		{
			var list = GenerateList(8);

			LeftListView.DataSource = list;
			TileView.DataSource = list;

			RightListView.DataSource = GenerateList(15);

			Table.DataSource = GenerateList(50);

			TreeView.Nodes = GenerateNodes(new System.Collections.Generic.List<int>() { 10, 5, 5, });

			TreeGraph.Nodes = GenerateNodes(new System.Collections.Generic.List<int>() { 2, 3, 2, });

			Autocomplete.DataSource = GenerateList(50).ToList();

			var ac_list = GenerateList(50);
			AutoCombobox.Combobox.ListView.DataSource = ac_list;
			AutoCombobox.Autocomplete.DataSource = ac_list.ListReference();

			Combobox.ListView.DataSource = GenerateList(20);
			ComboboxMultiselect.ListView.DataSource = GenerateList(20);

			pickerListViewData = GenerateList(20);

			pickerTreeViewNodes = GenerateNodes(new System.Collections.Generic.List<int>() { 10, 5, 3, });
		}

		/// <summary>
		/// Show ListView picker.
		/// </summary>
		public async void ShowPickerListView()
		{
			var picker = PickerListView.Clone();
			picker.ListView.DataSource = pickerListViewData;
			var item = await picker.ShowAsync(null);
			if (item.Success)
			{
				LeftListView.DataSource.Insert(0, item.Value);
			}
		}

		/// <summary>
		/// Show TreeView picker.
		/// </summary>
		public async void ShowPickerTreeView()
		{
			var picker = PickerTreeView.Clone();
			picker.TreeView.Nodes = pickerTreeViewNodes;
			var item = await picker.ShowAsync(null);
			if (item.Success)
			{
				TreeView.Nodes.Insert(0, item.Value);
			}
		}
	}
}