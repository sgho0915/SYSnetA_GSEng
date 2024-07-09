namespace UIWidgets
{
	using System;
	using EasyLayoutNS;
	using UIWidgets.Pool;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Layout utilities.
	/// </summary>
	public static class LayoutUtilities
	{
		/// <summary>
		/// Updates the layout.
		/// </summary>
		/// <param name="layout">Layout.</param>
		public static void UpdateLayout(LayoutGroup layout)
		{
			if (layout == null)
			{
				return;
			}

			layout.CalculateLayoutInputHorizontal();
			layout.SetLayoutHorizontal();
			layout.CalculateLayoutInputVertical();
			layout.SetLayoutVertical();
		}

		/// <summary>
		/// Updates the layouts for component and all children components.
		/// </summary>
		/// <param name="component">Component.</param>
		[Obsolete("Use LayoutRebuilder.ForceRebuildLayoutImmediate().")]
		public static void UpdateLayoutsRecursive(Component component)
		{
			if (component == null)
			{
				return;
			}

			using var _ = ListPool<LayoutGroup>.Get(out var layouts);

			component.GetComponentsInChildren(layouts);
			layouts.Reverse();

			foreach (var l in layouts)
			{
				UpdateLayout(l);
			}
		}

		/// <summary>
		/// Set padding left.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		public static void SetPaddingLeft(LayoutGroup layout, float size)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				hv.padding.left = Mathf.RoundToInt(size);
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			if (layout is EasyLayout el)
			{
				var p = el.PaddingInner;
				p.Left = size;
				el.PaddingInner = p;
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Set padding right.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		public static void SetPaddingRight(LayoutGroup layout, float size)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				hv.padding.right = Mathf.RoundToInt(size);
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			if (layout is EasyLayout el)
			{
				var p = el.PaddingInner;
				p.Right = size;
				el.PaddingInner = p;
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Set padding top.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		public static void SetPaddingTop(LayoutGroup layout, float size)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				hv.padding.top = Mathf.RoundToInt(size);
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			if (layout is EasyLayout el)
			{
				var p = el.PaddingInner;
				p.Top = size;
				el.PaddingInner = p;
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Set padding bottom.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		public static void SetPaddingBottom(LayoutGroup layout, float size)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				hv.padding.bottom = Mathf.RoundToInt(size);
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			if (layout is EasyLayout el)
			{
				var p = el.PaddingInner;
				p.Bottom = size;
				el.PaddingInner = p;
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Get padding left.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		public static float GetPaddingLeft(LayoutGroup layout)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				return hv.padding.left;
			}

			if (layout is EasyLayout el)
			{
				return el.PaddingInner.Left;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Get padding right.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		public static float GetPaddingRight(LayoutGroup layout)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				return hv.padding.right;
			}

			if (layout is EasyLayout el)
			{
				return el.PaddingInner.Right;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Get padding top.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		public static float GetPaddingTop(LayoutGroup layout)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				return hv.padding.top;
			}

			if (layout is EasyLayout el)
			{
				return el.PaddingInner.Top;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Get padding bottom.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		public static float GetPaddingBottom(LayoutGroup layout)
		{
			if (layout is HorizontalOrVerticalLayoutGroup hv)
			{
				return hv.padding.bottom;
			}

			if (layout is EasyLayout el)
			{
				return el.PaddingInner.Bottom;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Is target width under layout group or fitter control?
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>true if target width under layout group or fitter control; otherwise false.</returns>
		public static bool IsWidthControlled(RectTransform target)
		{
			var ignorer = target.GetComponent<ILayoutIgnorer>();
			if (ignorer != null && ignorer.ignoreLayout)
			{
				return false;
			}

			var fitter = target.GetComponent<ContentSizeFitter>();
			if ((fitter != null) && (fitter.horizontalFit != ContentSizeFitter.FitMode.Unconstrained))
			{
				return true;
			}

			var parent = target.transform.parent as RectTransform;
			if (parent != null)
			{
				var layout_group = parent.GetComponent<LayoutGroup>();
				if (layout_group == null)
				{
					return false;
				}

				if ((layout_group is GridLayoutGroup g_layout_group) && g_layout_group.enabled)
				{
					return true;
				}

				if ((layout_group is HorizontalOrVerticalLayoutGroup hv_layout_group) && hv_layout_group.enabled)
				{
					return Compatibility.GetLayoutChildControlWidth(hv_layout_group);
				}

				if ((layout_group is EasyLayout e_layout_group) && e_layout_group.enabled)
				{
					return e_layout_group.ChildrenWidth != ChildrenSize.DoNothing;
				}
			}

			return false;
		}

		/// <summary>
		/// Is target height under layout group or fitter control?
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>true if target height under layout group or fitter control; otherwise false.</returns>
		public static bool IsHeightControlled(RectTransform target)
		{
			var ignorer = target.GetComponent<ILayoutIgnorer>();
			if ((ignorer != null) && ignorer.ignoreLayout)
			{
				return false;
			}

			var fitter = target.GetComponent<ContentSizeFitter>();
			if ((fitter != null) && (fitter.verticalFit != ContentSizeFitter.FitMode.Unconstrained))
			{
				return true;
			}

			var parent = target.transform.parent as RectTransform;
			if (parent != null)
			{
				var layout_group = parent.GetComponent<LayoutGroup>();
				if (layout_group == null)
				{
					return false;
				}

				if ((layout_group is GridLayoutGroup g_layout_group) && g_layout_group.enabled)
				{
					return true;
				}

				if ((layout_group is HorizontalOrVerticalLayoutGroup hv_layout_group) && hv_layout_group.enabled)
				{
					return Compatibility.GetLayoutChildControlHeight(hv_layout_group);
				}

				if ((layout_group is EasyLayout e_layout_group) && e_layout_group.enabled)
				{
					return e_layout_group.ChildrenHeight != ChildrenSize.DoNothing;
				}
			}

			return false;
		}
	}
}