namespace UIWidgets.Custom.InquiryAlarmUINS
{
	/// <summary>
	/// Test script for the InquiryAlarmUI.
	/// </summary>
	public partial class TestInquiryAlarmUI : UIWidgets.WidgetGeneration.TestBase<InquiryAlarmUI>
	{
		/// <summary>
		/// Left ListView.
		/// </summary>
		public ListViewInquiryAlarmUI LeftListView;

		/// <summary>
		/// Right ListView.
		/// </summary>
		public ListViewInquiryAlarmUI RightListView;

		/// <summary>
		/// TileView.
		/// </summary>
		public ListViewInquiryAlarmUI TileView;

		/// <summary>
		/// TreeView.
		/// </summary>
		public TreeViewInquiryAlarmUI TreeView;

		/// <summary>
		/// Table.
		/// </summary>
		public ListViewInquiryAlarmUI Table;

		/// <summary>
		/// TreeGraph.
		/// </summary>
		public TreeGraphInquiryAlarmUI TreeGraph;

		/// <summary>
		/// Autocomplete.
		/// </summary>
		public AutocompleteInquiryAlarmUI Autocomplete;

		/// <summary>
		/// AutoCombobox.
		/// </summary>
		public AutoComboboxInquiryAlarmUI AutoCombobox;

		/// <summary>
		/// Combobox.
		/// </summary>
		public ComboboxInquiryAlarmUI Combobox;

		/// <summary>
		/// ComboboxMultiselect.
		/// </summary>
		public ComboboxInquiryAlarmUI ComboboxMultiselect;

		/// <summary>
		/// ListView picker.
		/// </summary>
		public PickerListViewInquiryAlarmUI PickerListView;

		/// <summary>
		/// TreeView picker.
		/// </summary>
		public PickerTreeViewInquiryAlarmUI PickerTreeView;

		UIWidgets.ObservableList<InquiryAlarmUI> pickerListViewData;

		UIWidgets.ObservableList<UIWidgets.TreeNode<InquiryAlarmUI>> pickerTreeViewNodes;

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