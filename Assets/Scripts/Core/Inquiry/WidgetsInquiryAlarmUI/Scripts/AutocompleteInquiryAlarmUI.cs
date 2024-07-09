namespace UIWidgets.Custom.InquiryAlarmUINS
{
	/// <summary>
	/// Autocomplete for the InquiryAlarmUI.
	/// </summary>
	public partial class AutocompleteInquiryAlarmUI : UIWidgets.AutocompleteCustom<InquiryAlarmUI, ListViewComponentInquiryAlarmUI, ListViewInquiryAlarmUI>
	{
		/// <summary>
		/// Determines whether the beginning of value matches the Query.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if beginning of value matches the Input; otherwise, false.</returns>
		public override bool Startswith(InquiryAlarmUI value)
		{
			return UIWidgets.UtilitiesCompare.StartsWith(value.otime, Query, CaseSensitive);
		}

		/// <summary>
		/// Returns a value indicating whether Query occurs within specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if the Query occurs within value parameter; otherwise, false.</returns>
		public override bool Contains(InquiryAlarmUI value)
		{
			return UIWidgets.UtilitiesCompare.Contains(value.otime, Query, CaseSensitive);
		}

		/// <summary>
		/// Convert value to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="value">Value.</param>
		protected override string GetStringValue(InquiryAlarmUI value)
		{
			return value.otime;
		}
	}
}