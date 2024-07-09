namespace UIWidgets.Custom.InquiryAlarmUINS
{
	/// <summary>
	/// TreeView component for the InquiryAlarmUI.
	/// </summary>
	public partial class TreeViewComponentInquiryAlarmUI : UIWidgets.TreeViewComponentBase<InquiryAlarmUI>
	{
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
		}

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

		InquiryAlarmUI item;

		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		public InquiryAlarmUI Item
		{
			get
			{
				return item;
			}

			set
			{
				item = value;

				UpdateView();
			}
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="depth">Depth.</param>
		public override void SetData(UIWidgets.TreeNode<InquiryAlarmUI> node, int depth)
		{
			Node = node;

			base.SetData(Node, depth);

			Item = (Node == null) ? null : Node.Item;
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
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