namespace UIWidgets.Custom.InquiryControlUINS
{
	/// <summary>
	/// Tooltip InquiryControlUI.
	/// </summary>
	public class TooltipInquiryControlUI : UIWidgets.Tooltip<InquiryControlUI, TooltipInquiryControlUI>
	{

		/// <summary>
		/// The atime.
		/// </summary>
		public UIWidgets.TextAdapter atime;

		/// <summary>
		/// The cname.
		/// </summary>
		public UIWidgets.TextAdapter cname;

		/// <summary>
		/// The desc.
		/// </summary>
		public UIWidgets.TextAdapter desc;

		/// <summary>
		/// The ctlUser.
		/// </summary>
		public UIWidgets.TextAdapter ctlUser;

		/// <summary>
		/// Gets the current item.
		/// </summary>
		public InquiryControlUI Item
		{
			get;
			protected set;
		}

		/// <inheritdoc/>
		protected override void SetData(InquiryControlUI item)
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

			if (atime != null)
			{
				atime.text = Item.atime;
			}

			if (cname != null)
			{
				cname.text = Item.cname;
			}

			if (desc != null)
			{
				desc.text = Item.desc;
			}

			if (ctlUser != null)
			{
				ctlUser.text = Item.ctlUser;
			}

		}
	}
}