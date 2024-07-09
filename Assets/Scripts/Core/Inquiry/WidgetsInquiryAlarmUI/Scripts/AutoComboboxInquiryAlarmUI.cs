namespace UIWidgets.Custom.InquiryAlarmUINS
{
	/// <summary>
	/// AutoCombobox for the InquiryAlarmUI.
	/// </summary>
	public partial class AutoComboboxInquiryAlarmUI : UIWidgets.AutoCombobox<InquiryAlarmUI, ListViewInquiryAlarmUI, ListViewComponentInquiryAlarmUI, AutocompleteInquiryAlarmUI, ComboboxInquiryAlarmUI>
	{
		/// <inheritdoc/>
		protected override string GetStringValue(InquiryAlarmUI item)
		{
			return item.otime;
		}
	}
}