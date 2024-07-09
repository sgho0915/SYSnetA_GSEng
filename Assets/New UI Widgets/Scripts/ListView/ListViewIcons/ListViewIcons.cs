namespace UIWidgets
{
	using System;
	using UIWidgets.l10n;
	using UnityEngine;

	/// <summary>
	/// ListViewIcons.
	/// </summary>
	public class ListViewIcons : ListViewCustom<ListViewIconsItemComponent, ListViewIconsItemDescription>, ILocalizationSupport
	{
		[SerializeField]
		[Tooltip("If enabled translates items names using Localization.GetTranslation().")]
		bool localizationSupport = true;

		/// <summary>
		/// Localization support.
		/// </summary>
		public bool LocalizationSupport
		{
			get => localizationSupport;

			set => localizationSupport = value;
		}

		/// <summary>
		/// Sort items.
		/// Deprecated. Replaced with DataSource.Comparison.
		/// </summary>
		[Obsolete("Replaced with DataSource.Comparison.")]
		public override bool Sort
		{
			get => DataSource.Comparison == ItemsComparison;

			set
			{
				if (value)
				{
					DataSource.Comparison = ItemsComparison;
				}
				else
				{
					DataSource.Comparison = null;
				}
			}
		}

		static string GetLocalizedItemName(ListViewIconsItemDescription item)
		{
			if (item == null)
			{
				return string.Empty;
			}

			return item.LocalizedName ?? Localization.GetTranslation(item.Name);
		}

		static string GetItemName(ListViewIconsItemDescription item)
		{
			if (item == null)
			{
				return string.Empty;
			}

			return item.LocalizedName ?? item.Name;
		}

		[NonSerialized]
		bool isListViewIconsInited = false;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isListViewIconsInited)
			{
				return;
			}

			isListViewIconsInited = true;

			base.Init();

#pragma warning disable 0618
			if (base.Sort)
			{
				if (LocalizationSupport)
				{
					DataSource.Comparison = LocalizedItemsComparison;
				}
				else
				{
					DataSource.Comparison = ItemsComparison;
				}
			}
#pragma warning restore 0618
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public override void LocaleChanged()
		{
			base.LocaleChanged();

			if (DataSource.Comparison != null)
			{
				DataSource.CollectionChanged();
			}
		}

		/// <summary>
		/// Items comparison by localized names.
		/// </summary>
		public static Comparison<ListViewIconsItemDescription> LocalizedItemsComparison = (x, y) => UtilitiesCompare.Compare(GetLocalizedItemName(x), GetLocalizedItemName(y));

		/// <summary>
		/// Items comparison.
		/// </summary>
		public static Comparison<ListViewIconsItemDescription> ItemsComparison = (x, y) => UtilitiesCompare.Compare(GetItemName(x), GetItemName(y));
	}
}