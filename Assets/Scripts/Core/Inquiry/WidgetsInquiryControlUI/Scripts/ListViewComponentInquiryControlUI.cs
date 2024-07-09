namespace UIWidgets.Custom.InquiryControlUINS
{
	/// <summary>
	/// ListView component for the InquiryControlUI.
	/// </summary>
	public partial class ListViewComponentInquiryControlUI : UIWidgets.ListViewItem, UIWidgets.IViewData<InquiryControlUI>
	{
		/// <inheritdoc/>
		protected override void GraphicsBackgroundInit()
		{
			base.GraphicsBackgroundInit();

			if (CellsBackgroundVersion == 0)
			{
				using var _ = UIWidgets.Pool.ListPool<UnityEngine.UI.Graphic>.Get(out var result);

				#if UIWIDGETS_LEGACY_STYLE
				foreach (UnityEngine.Transform child in transform)
				{
					var graphic = child.GetComponent<UnityEngine.UI.Graphic>();
					if (graphic != null)
					{
						result.Add(graphic);
					}
				}
				#else
				result.Add(GetComponent<UnityEngine.UI.Graphic>());
				#endif

				if (result.Count > 0)
				{
					cellsBackgrounds.Clear();
					cellsBackgrounds.AddRange(result);

					CellsBackgroundVersion = 1;
				}
			}
		}

		/// <inheritdoc/>
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

		/// <summary>
		/// Tooltip.
		/// </summary>
		[UnityEngine.SerializeField]
		protected TooltipInquiryControlUI tooltip;

		/// <summary>
		/// Tooltip.
		/// </summary>
		public TooltipInquiryControlUI Tooltip
		{
			get
			{
				return tooltip;
			}

			set
			{
				if (tooltip != null)
				{
					Tooltip.Unregister(gameObject);
				}

				tooltip = value;

				if ((tooltip != null) && (Item != null))
				{
					Tooltip.Register(gameObject, Item, TooltipSettings);
				}
			}
		}

		/// <summary>
		/// Tooltip settings.
		/// </summary>
		[UnityEngine.SerializeField]
		protected UIWidgets.TooltipSettings TooltipSettings = new UIWidgets.TooltipSettings(UIWidgets.TooltipPosition.TopCenter);

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(InquiryControlUI item)
		{
			Item = item;

			#if UNITY_EDITOR
			name = Item == null ? "DefaultItem " + Index.ToString() : Item.atime;
			#endif

			if ((Tooltip != null) && (Item != null))
			{
				Tooltip.Register(gameObject, Item, TooltipSettings);
			}

			UpdateView();
		}

		/// <inheritdoc/>
		public override void LocaleChanged()
		{
			UpdateView();
		}

		/// <summary>
		/// Update view.
		/// </summary>
		protected virtual void UpdateView()
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

		/// <inheritdoc/>
		public override void MovedToCache()
		{
			if (Tooltip != null)
			{
				Tooltip.Unregister(gameObject);
			}
		}

		/// <inheritdoc/>
		public override void SetThemeImagesPropertiesOwner(UnityEngine.Component owner)
		{
			base.SetThemeImagesPropertiesOwner(owner);
		}
	}
}