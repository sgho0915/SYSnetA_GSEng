namespace UIWidgets.Custom.InquiryAlarmUINS
{
	/// <summary>
	/// Sort fields for the for the InquiryAlarmUI.
	/// </summary>
	public enum ComparersFieldsInquiryAlarmUI
	{
		/// <summary>
		/// Not sorted list.
		/// </summary>
		None,

		/// <summary>
		/// otime.
		/// </summary>
		otime,

		/// <summary>
		/// ctime.
		/// </summary>
		ctime,

		/// <summary>
		/// cname.
		/// </summary>
		cname,

		/// <summary>
		/// desc.
		/// </summary>
		desc,

	}

	/// <summary>
	/// Comparer functions for the InquiryAlarmUI.
	/// </summary>
	public static partial class ComparersInquiryAlarmUI
	{
		/// <summary>
		/// Comparer.
		/// </summary>
		public static readonly System.Collections.Generic.Dictionary<int, System.Comparison<InquiryAlarmUI>> Comparers = new System.Collections.Generic.Dictionary<int, System.Comparison<InquiryAlarmUI>>()
			{
				{ (int)ComparersFieldsInquiryAlarmUI.otime, otimeComparer },
				{ (int)ComparersFieldsInquiryAlarmUI.ctime, ctimeComparer },
				{ (int)ComparersFieldsInquiryAlarmUI.cname, cnameComparer },
				{ (int)ComparersFieldsInquiryAlarmUI.desc, descComparer },
			};

		#region Items comparer

		/// <summary>
		/// otime comparer.
		/// </summary>
		/// <param name="x">First InquiryAlarmUI.</param>
		/// <param name="y">Second InquiryAlarmUI.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int otimeComparer(InquiryAlarmUI x, InquiryAlarmUI y)
		{
			return UIWidgets.UtilitiesCompare.Compare(x.otime, y.otime);
		}

		/// <summary>
		/// ctime comparer.
		/// </summary>
		/// <param name="x">First InquiryAlarmUI.</param>
		/// <param name="y">Second InquiryAlarmUI.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int ctimeComparer(InquiryAlarmUI x, InquiryAlarmUI y)
		{
			return UIWidgets.UtilitiesCompare.Compare(x.ctime, y.ctime);
		}

		/// <summary>
		/// cname comparer.
		/// </summary>
		/// <param name="x">First InquiryAlarmUI.</param>
		/// <param name="y">Second InquiryAlarmUI.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int cnameComparer(InquiryAlarmUI x, InquiryAlarmUI y)
		{
			return UIWidgets.UtilitiesCompare.Compare(x.cname, y.cname);
		}

		/// <summary>
		/// desc comparer.
		/// </summary>
		/// <param name="x">First InquiryAlarmUI.</param>
		/// <param name="y">Second InquiryAlarmUI.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		static int descComparer(InquiryAlarmUI x, InquiryAlarmUI y)
		{
			return UIWidgets.UtilitiesCompare.Compare(x.desc, y.desc);
		}
		#endregion

		#region Items comparer
		#endregion
	}
}