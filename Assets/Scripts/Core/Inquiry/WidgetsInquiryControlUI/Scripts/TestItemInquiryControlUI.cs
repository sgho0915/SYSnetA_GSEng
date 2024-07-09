namespace UIWidgets.Custom.InquiryControlUINS
{
	/// <summary>
	/// Test script for the InquiryControlUI.
	/// </summary>
	public partial class TestInquiryControlUI : UIWidgets.WidgetGeneration.TestBase<InquiryControlUI>
	{
		/// <summary>
		/// Generate item.
		/// </summary>
		/// <param name="index">Item index.</param>
		/// <returns>Item.</returns>
		protected override InquiryControlUI GenerateItem(int index)
		{
			return new InquiryControlUI()
			{
				atime = "atime " + index.ToString("0000"),
				cname = "cname " + index.ToString("0000"),
				desc = "desc " + index.ToString("0000"),
				ctlUser = "ctlUser " + index.ToString("0000"),
			};
		}

		/// <summary>
		/// Generate item with the specified name.
		/// </summary>
		/// <param name="name">Item name.</param>
		/// <param name="index">Item index.</param>
		/// <returns>Item.</returns>
		protected override InquiryControlUI GenerateItem(string name, int index)
		{
			var item = GenerateItem(index);

			item.atime = name;

			return item;
		}
	}
}