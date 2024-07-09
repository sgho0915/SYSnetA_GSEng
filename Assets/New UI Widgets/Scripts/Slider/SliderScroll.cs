namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Changes Slider value on the mouse scroll.
	/// </summary>
	[RequireComponent(typeof(Slider))]
	public class SliderScroll : MonoBehaviourConditional,
		IScrollHandler
	{
		/// <summary>
		/// Scroll modes.
		/// </summary>
		public enum ScrollModes
		{
			/// <summary>
			/// Ignore.
			/// </summary>
			Ignore = 0,

			/// <summary>
			/// Increase on scroll up.
			/// </summary>
			UpIncrease = 1,

			/// <summary>
			/// Increase on scroll down.
			/// </summary>
			UpDecrease = 2,
		}

		/// <summary>
		/// Scroll mode.
		/// </summary>
		[SerializeField]
		public ScrollModes ScrollMode = ScrollModes.UpIncrease;

		/// <summary>
		/// Scroll step.
		/// </summary>
		[SerializeField]
		public float Step = 0.1f;

		Slider slider;

		/// <summary>
		/// Slider.
		/// </summary>
		public Slider Slider
		{
			get
			{
				if (slider == null)
				{
					slider = GetComponent<Slider>();
				}

				return slider;
			}
		}

		/// <summary>
		/// Process the scroll event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnScroll(PointerEventData eventData)
		{
			if (Slider == null)
			{
				return;
			}

			switch (ScrollMode)
			{
				case ScrollModes.UpIncrease:
					Slider.value += Step * Mathf.Sign(eventData.scrollDelta.y);
					break;
				case ScrollModes.UpDecrease:
					Slider.value -= Step * Mathf.Sign(eventData.scrollDelta.y);
					break;
				default:
					break;
			}
		}
	}
}