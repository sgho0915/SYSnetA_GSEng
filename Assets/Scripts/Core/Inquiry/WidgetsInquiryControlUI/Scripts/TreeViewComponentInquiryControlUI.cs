namespace UIWidgets.Custom.InquiryControlUINS
{
	/// <summary>
	/// TreeView component for the InquiryControlUI.
	/// </summary>
	public partial class TreeViewComponentInquiryControlUI : UIWidgets.TreeViewComponentBase<InquiryControlUI>
	{
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
		}

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

		InquiryControlUI item;

		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		public InquiryControlUI Item
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
		public override void SetData(UIWidgets.TreeNode<InquiryControlUI> node, int depth)
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