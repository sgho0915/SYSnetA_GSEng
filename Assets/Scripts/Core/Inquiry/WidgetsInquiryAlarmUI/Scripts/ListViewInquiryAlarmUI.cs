namespace UIWidgets.Custom.InquiryAlarmUINS
{
	/// <summary>
	/// ListView for the InquiryAlarmUI.
	/// </summary>
	public partial class ListViewInquiryAlarmUI : UIWidgets.ListViewCustom<ListViewComponentInquiryAlarmUI, InquiryAlarmUI>
	{
		ComparersFieldsInquiryAlarmUI currentSortField = ComparersFieldsInquiryAlarmUI.None;

		/// <summary>
		/// Toggle sort.
		/// </summary>
		/// <param name="field">Sort field.</param>
		public void ToggleSort(ComparersFieldsInquiryAlarmUI field)
		{
			if (field == currentSortField)
			{
				DataSource.Reverse();
			}
			else if (ComparersInquiryAlarmUI.Comparers.ContainsKey((int)field))
			{
				currentSortField = field;

				DataSource.Sort(ComparersInquiryAlarmUI.Comparers[(int)field]);
			}
		}

		#region used in Button.OnClick()

		/// <summary>
		/// Sort by otime.
		/// </summary>
		public void SortByotime()
		{
			ToggleSort(ComparersFieldsInquiryAlarmUI.otime);
		}

		/// <summary>
		/// Sort by ctime.
		/// </summary>
		public void SortByctime()
		{
			ToggleSort(ComparersFieldsInquiryAlarmUI.ctime);
		}

		/// <summary>
		/// Sort by cname.
		/// </summary>
		public void SortBycname()
		{
			ToggleSort(ComparersFieldsInquiryAlarmUI.cname);
		}

		/// <summary>
		/// Sort by desc.
		/// </summary>
		public void SortBydesc()
		{
			ToggleSort(ComparersFieldsInquiryAlarmUI.desc);
		}
		#endregion
	}
}