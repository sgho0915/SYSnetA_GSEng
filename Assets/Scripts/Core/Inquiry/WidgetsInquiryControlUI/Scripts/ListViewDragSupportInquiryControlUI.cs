namespace UIWidgets.Custom.InquiryControlUINS
{
	/// <summary>
	/// ListView drag support for the InquiryControlUI.
	/// </summary>
	[UnityEngine.RequireComponent(typeof(ListViewComponentInquiryControlUI))]
	public partial class ListViewDragSupportInquiryControlUI : UIWidgets.ListViewCustomDragSupport<ListViewInquiryControlUI, ListViewComponentInquiryControlUI, InquiryControlUI>
	{
		/// <summary>
		/// Get data from specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>Data.</returns>
		protected override InquiryControlUI GetData(ListViewComponentInquiryControlUI component)
		{
			return component.Item;
		}

		/// <summary>
		/// Set data for DragInfo component.
		/// </summary>
		/// <param name="data">Data.</param>
		protected override void SetDragInfoData(InquiryControlUI data)
		{
			DragInfo.SetData(data);
		}
	}
}