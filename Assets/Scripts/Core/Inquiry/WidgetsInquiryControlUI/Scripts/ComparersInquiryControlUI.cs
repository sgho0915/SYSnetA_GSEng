namespace UIWidgets.Custom.InquiryControlUINS
{
	/// <summary>
	/// Sort fields for the for the InquiryControlUI.
	/// </summary>
	public enum ComparersFieldsInquiryControlUI
	{
		/// <summary>
		/// Not sorted list.
		/// </summary>
		None,

		/// <summary>
		/// atime.
		/// </summary>
		atime,

		/// <summary>
		/// cname.
		/// </summary>
		cname,

		/// <summary>
		/// desc.
		/// </summary>
		desc,

		/// <summary>
		/// ctlUser.
		/// </summary>
		ctlUser,

	}

	/// <summary>
	/// Comparer functions for the InquiryControlUI.
	/// </summary>
	public static partial class ComparersInquiryControlUI
	{
		/// <summary>
		/// Comparer.
		/// </summary>
		public static readonly System.Collections.Generic.Dictionary<int, System.Comparison<InquiryControlUI>> Comparers = new System.Collections.Generic.Dictionary<int, System.Comparison<InquiryControlUI>>()
			{
				{ (int)ComparersFieldsInquiryControlUI.atime, atimeComparer },
				{ (int)ComparersFieldsInquiryControlUI.cname, cnameComparer },
				{ (int)ComparersFieldsInquiryControlUI.desc, descComparer },
				{ (int)ComparersFieldsInquiryControlUI.ctlUser, ctlUserComparer },
			};

		#region Items comparer

		/// <summary>
		/// atime comparer.
		/// </summary>
		/// <param name="x">First InquiryControlUI.</param>
		/// <param name="y">Second InquiryControlUI.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int atimeComparer(InquiryControlUI x, InquiryControlUI y)
		{
			return UIWidgets.UtilitiesCompare.Compare(x.atime, y.atime);
		}

		/// <summary>
		/// cname comparer.
		/// </summary>
		/// <param name="x">First InquiryControlUI.</param>
		/// <param name="y">Second InquiryControlUI.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int cnameComparer(InquiryControlUI x, InquiryControlUI y)
		{
			return UIWidgets.UtilitiesCompare.Compare(x.cname, y.cname);
		}

		/// <summary>
		/// desc comparer.
		/// </summary>
		/// <param name="x">First InquiryControlUI.</param>
		/// <param name="y">Second InquiryControlUI.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int descComparer(InquiryControlUI x, InquiryControlUI y)
		{
			return UIWidgets.UtilitiesCompare.Compare(x.desc, y.desc);
		}

		/// <summary>
		/// ctlUser comparer.
		/// </summary>
		/// <param name="x">First InquiryControlUI.</param>
		/// <param name="y">Second InquiryControlUI.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int ctlUserComparer(InquiryControlUI x, InquiryControlUI y)
		{
			return UIWidgets.UtilitiesCompare.Compare(x.ctlUser, y.ctlUser);
		}
		#endregion

		#region Items comparer
		#endregion
	}
}