namespace UIWidgets.Custom.InquiryAlarmUINS
{
	/// <summary>
	/// TreeGraph component for the InquiryAlarmUI.
	/// </summary>
	public partial class TreeGraphComponentInquiryAlarmUI : UIWidgets.TreeGraphComponent<InquiryAlarmUI>
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
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				foregrounds.Add(UIWidgets.UtilitiesUI.GetGraphic(otime));
				foregrounds.Add(UIWidgets.UtilitiesUI.GetGraphic(ctime));
				foregrounds.Add(UIWidgets.UtilitiesUI.GetGraphic(cname));
				foregrounds.Add(UIWidgets.UtilitiesUI.GetGraphic(desc));

				if (!UIWidgets.UtilitiesCollections.AllNull(foregrounds))
				{
					foregrounds.RemoveAll(x => x == null);
					GraphicsForegroundVersion = 1;
				}
			}

			base.GraphicsForegroundInit();
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="node">Node.</param>
		public override void SetData(UIWidgets.TreeNode<InquiryAlarmUI> node)
		{
			Node = node;

			if (otime != null)
			{
				otime.text = Node.Item.otime;
			}

			if (ctime != null)
			{
				ctime.text = Node.Item.ctime;
			}

			if (cname != null)
			{
				cname.text = Node.Item.cname;
			}

			if (desc != null)
			{
				desc.text = Node.Item.desc;
			}
		}

		/// <summary>
		/// Called when item moved to cache, you can use it free used resources.
		/// </summary>
		public override void MovedToCache()
		{
		}

		/// <inheritdoc/>
		public override void SetThemeImagesPropertiesOwner(UnityEngine.Component owner)
		{
			base.SetThemeImagesPropertiesOwner(owner);
		}
	}
}