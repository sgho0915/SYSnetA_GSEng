#if UNITY_EDITOR
namespace UIWidgets.Custom.InquiryAlarmUINS
{
	using UIWidgets;
	using UnityEditor;

	/// <summary>
	/// Menu options.
	/// </summary>
	public static partial class MenuOptionsInquiryAlarmUI
	{
		static readonly string GUID = "a34bcbad662f548449b8ce478c570b26";

		static PrefabsMenuGenerated prefabs;

		static PrefabsMenuGenerated Prefabs
		{
			get
			{
				if (prefabs == null)
				{
					prefabs = UtilitiesEditor.LoadAssetWithGUID<PrefabsMenuGenerated>(GUID);
				}

				return prefabs;
			}
		}

		static void Create(UnityEngine.GameObject prefab) => Widgets.CreateFromPrefab(prefab);

		/// <summary>
		/// Create Autocomplete.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - InquiryAlarmUI/Autocomplete", false, 2000)]
		public static void CreateAutocomplete() => Create(Prefabs.Autocomplete);

		/// <summary>
		/// Create AutoCombobox.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - InquiryAlarmUI/AutoCombobox", false, 2005)]
		public static void CreateAutoCombobox() => Create(Prefabs.AutoCombobox);

		/// <summary>
		/// Create Combobox.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - InquiryAlarmUI/Combobox", false, 2010)]
		public static void CreateCombobox() => Create(Prefabs.Combobox);

		/// <summary>
		/// Create ComboboxMultiselect.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - InquiryAlarmUI/ComboboxMultiselect", false, 2020)]
		public static void CreateComboboxMultiselect() => Create(Prefabs.ComboboxMultiselect);

		/// <summary>
		/// Create DragInfo.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - InquiryAlarmUI/DragInfo", false, 2030)]
		public static void CreateDragInfo() => Create(Prefabs.DragInfo);

		/// <summary>
		/// Create ListView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - InquiryAlarmUI/ListView", false, 2040)]
		public static void CreateListView() => Create(Prefabs.ListView);

		/// <summary>
		/// Create Table.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - InquiryAlarmUI/Table", false, 2050)]
		public static void CreateTable() => Create(Prefabs.Table);

		/// <summary>
		/// Create TileView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - InquiryAlarmUI/TileView", false, 2060)]
		public static void CreateTileView() => Create(Prefabs.TileView);

		/// <summary>
		/// Create Tooltip.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - InquiryAlarmUI/Tooltip", false, 2065)]
		public static void CreateTooltip() => Create(Prefabs.Tooltip);

		/// <summary>
		/// Create TreeGraph.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - InquiryAlarmUI/TreeGraph", false, 2070)]
		public static void CreateTreeGraph() => Create(Prefabs.TreeGraph);

		/// <summary>
		/// Create TreeView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - InquiryAlarmUI/TreeView", false, 2080)]
		public static void CreateTreeView() => Create(Prefabs.TreeView);

		/// <summary>
		/// Create PickerListView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - InquiryAlarmUI/PickerListView", false, 2090)]
		public static void CreatePickerListView() => Create(Prefabs.PickerListView);

		/// <summary>
		/// Create PickerTreeView.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets - InquiryAlarmUI/PickerTreeView", false, 2100)]
		public static void CreatePickerTreeView() => Create(Prefabs.PickerTreeView);
	}
}
#endif