namespace UIWidgets.Custom.InquiryControlUINS
{
	/// <summary>
	/// TreeGraph component for the InquiryControlUI.
	/// </summary>
	public partial class TreeGraphComponentInquiryControlUI : UIWidgets.TreeGraphComponent<InquiryControlUI>
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
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				foregrounds.Add(UIWidgets.UtilitiesUI.GetGraphic(atime));
				foregrounds.Add(UIWidgets.UtilitiesUI.GetGraphic(cname));
				foregrounds.Add(UIWidgets.UtilitiesUI.GetGraphic(desc));
				foregrounds.Add(UIWidgets.UtilitiesUI.GetGraphic(ctlUser));

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
		public override void SetData(UIWidgets.TreeNode<InquiryControlUI> node)
		{
			Node = node;

			if (atime != null)
			{
				atime.text = Node.Item.atime;
			}

			if (cname != null)
			{
				cname.text = Node.Item.cname;
			}

			if (desc != null)
			{
				desc.text = Node.Item.desc;
			}

			if (ctlUser != null)
			{
				ctlUser.text = Node.Item.ctlUser;
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