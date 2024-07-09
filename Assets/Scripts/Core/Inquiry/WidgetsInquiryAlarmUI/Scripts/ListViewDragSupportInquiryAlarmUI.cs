namespace UIWidgets.Custom.InquiryAlarmUINS
{
	/// <summary>
	/// ListView drag support for the InquiryAlarmUI.
	/// </summary>
	[UnityEngine.RequireComponent(typeof(ListViewComponentInquiryAlarmUI))]
	public partial class ListViewDragSupportInquiryAlarmUI : UIWidgets.ListViewCustomDragSupport<ListViewInquiryAlarmUI, ListViewComponentInquiryAlarmUI, InquiryAlarmUI>
	{
		/// <summary>
		/// Get data from specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>Data.</returns>
		protected override InquiryAlarmUI GetData(ListViewComponentInquiryAlarmUI component)
		{
			return component.Item;
		}

		/// <summary>
		/// Set data for DragInfo component.
		/// </summary>
		/// <param name="data">Data.</param>
		protected override void SetDragInfoData(InquiryAlarmUI data)
		{
			DragInfo.SetData(data);
		}
	}
}