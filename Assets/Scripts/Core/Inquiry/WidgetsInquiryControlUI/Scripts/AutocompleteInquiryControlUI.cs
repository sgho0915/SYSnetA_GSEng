namespace UIWidgets.Custom.InquiryControlUINS
{
	/// <summary>
	/// Autocomplete for the InquiryControlUI.
	/// </summary>
	public partial class AutocompleteInquiryControlUI : UIWidgets.AutocompleteCustom<InquiryControlUI, ListViewComponentInquiryControlUI, ListViewInquiryControlUI>
	{
		/// <summary>
		/// Determines whether the beginning of value matches the Query.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if beginning of value matches the Input; otherwise, false.</returns>
		public override bool Startswith(InquiryControlUI value)
		{
			return UIWidgets.UtilitiesCompare.StartsWith(value.atime, Query, CaseSensitive);
		}

		/// <summary>
		/// Returns a value indicating whether Query occurs within specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if the Query occurs within value parameter; otherwise, false.</returns>
		public override bool Contains(InquiryControlUI value)
		{
			return UIWidgets.UtilitiesCompare.Contains(value.atime, Query, CaseSensitive);
		}

		/// <summary>
		/// Convert value to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="value">Value.</param>
		protected override string GetStringValue(InquiryControlUI value)
		{
			return value.atime;
		}
	}
}