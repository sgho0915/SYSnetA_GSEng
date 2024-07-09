namespace UIWidgets.Custom.InquiryAlarmUINS
{
	/// <summary>
	/// Test script for the InquiryAlarmUI.
	/// </summary>
	public partial class TestInquiryAlarmUI : UIWidgets.WidgetGeneration.TestBase<InquiryAlarmUI>
	{
		/// <summary>
		/// Generate item.
		/// </summary>
		/// <param name="index">Item index.</param>
		/// <returns>Item.</returns>
		protected override InquiryAlarmUI GenerateItem(int index)
		{
			return new InquiryAlarmUI()
			{
				otime = "otime " + index.ToString("0000"),
				ctime = "ctime " + index.ToString("0000"),
				cname = "cname " + index.ToString("0000"),
				desc = "desc " + index.ToString("0000"),
			};
		}

		/// <summary>
		/// Generate item with the specified name.
		/// </summary>
		/// <param name="name">Item name.</param>
		/// <param name="index">Item index.</param>
		/// <returns>Item.</returns>
		protected override InquiryAlarmUI GenerateItem(string name, int index)
		{
			var item = GenerateItem(index);

			item.otime = name;

			return item;
		}
	}
}