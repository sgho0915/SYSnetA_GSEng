namespace UIWidgets.Custom.InquiryControlUINS
{
	/// <summary>
	/// AutoCombobox for the InquiryControlUI.
	/// </summary>
	public partial class AutoComboboxInquiryControlUI : UIWidgets.AutoCombobox<InquiryControlUI, ListViewInquiryControlUI, ListViewComponentInquiryControlUI, AutocompleteInquiryControlUI, ComboboxInquiryControlUI>
	{
		/// <inheritdoc/>
		protected override string GetStringValue(InquiryControlUI item)
		{
			return item.atime;
		}
	}
}