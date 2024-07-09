namespace UIWidgets.Custom.InquiryAlarmUINS
{
	/// <summary>
	/// Tooltip InquiryAlarmUI.
	/// </summary>
	public class TooltipInquiryAlarmUI : UIWidgets.Tooltip<InquiryAlarmUI, TooltipInquiryAlarmUI>
	{

		/// <summary>
		/// The otime.
		/// </summary>
		public UIWidgets.TextAdapter otime;

		/// <summary>
		/// The ctime.
		/// </summary>
		public UIWidgets.TextAdapter ctime;

		/// <summary>
		/// The cname.
		/// </summary>
		public UIWidgets.TextAdapter cname;

		/// <summary>
		/// The desc.
		/// </summary>
		public UIWidgets.TextAdapter desc;

		/// <summary>
		/// Gets the current item.
		/// </summary>
		public InquiryAlarmUI Item
		{
			get;
			protected set;
		}

		/// <inheritdoc/>
		protected override void SetData(InquiryAlarmUI item)
		{
			Item = item;

			UpdateView();
		}

		/// <inheritdoc/>
		protected override void UpdateView()
		{
			if (Item == null)
			{
				return;
			}

			if (otime != null)
			{
				otime.text = Item.otime;
			}

			if (ctime != null)
			{
				ctime.text = Item.ctime;
			}

			if (cname != null)
			{
				cname.text = Item.cname;
			}

			if (desc != null)
			{
				desc.text = Item.desc;
			}

		}
	}
}