namespace UIWidgets.Custom.InquiryControlUINS
{
	/// <summary>
	/// ListView for the InquiryControlUI.
	/// </summary>
	public partial class ListViewInquiryControlUI : UIWidgets.ListViewCustom<ListViewComponentInquiryControlUI, InquiryControlUI>
	{
		ComparersFieldsInquiryControlUI currentSortField = ComparersFieldsInquiryControlUI.None;

		/// <summary>
		/// Toggle sort.
		/// </summary>
		/// <param name="field">Sort field.</param>
		public void ToggleSort(ComparersFieldsInquiryControlUI field)
		{
			if (field == currentSortField)
			{
				DataSource.Reverse();
			}
			else if (ComparersInquiryControlUI.Comparers.ContainsKey((int)field))
			{
				currentSortField = field;

				DataSource.Sort(ComparersInquiryControlUI.Comparers[(int)field]);
			}
		}

		#region used in Button.OnClick()

		/// <summary>
		/// Sort by atime.
		/// </summary>
		public void SortByatime()
		{
			ToggleSort(ComparersFieldsInquiryControlUI.atime);
		}

		/// <summary>
		/// Sort by cname.
		/// </summary>
		public void SortBycname()
		{
			ToggleSort(ComparersFieldsInquiryControlUI.cname);
		}

		/// <summary>
		/// Sort by desc.
		/// </summary>
		public void SortBydesc()
		{
			ToggleSort(ComparersFieldsInquiryControlUI.desc);
		}

		/// <summary>
		/// Sort by ctlUser.
		/// </summary>
		public void SortByctlUser()
		{
			ToggleSort(ComparersFieldsInquiryControlUI.ctlUser);
		}
		#endregion
	}
}