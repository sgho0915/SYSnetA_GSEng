namespace UIWidgets.Custom.InquiryAlarmUINS
{
	/// <summary>
	/// ListView component for the InquiryAlarmUI.
	/// </summary>
	public partial class ListViewComponentInquiryAlarmUI : UIWidgets.ListViewItem, UIWidgets.IViewData<InquiryAlarmUI>
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
		/// Gets the current item.
		/// </summary>
		public InquiryAlarmUI Item
		{
			get;
			protected set;
		}

		/// <summary>
		/// Tooltip.
		/// </summary>
		[UnityEngine.SerializeField]
		protected TooltipInquiryAlarmUI tooltip;

		/// <summary>
		/// Tooltip.
		/// </summary>
		public TooltipInquiryAlarmUI Tooltip
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
		public virtual void SetData(InquiryAlarmUI item)
		{
			Item = item;

			#if UNITY_EDITOR
			name = Item == null ? "DefaultItem " + Index.ToString() : Item.otime;
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