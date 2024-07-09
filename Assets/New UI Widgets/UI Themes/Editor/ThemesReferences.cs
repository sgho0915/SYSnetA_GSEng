#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using System;
	using System.IO;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Themes references.
	/// </summary>
	[Serializable]
	public class ThemesReferences : ScriptableObject
	{
		[SerializeField]
		[HideInInspector]
		bool assemblyDefinitions = true;

		/// <summary>
		/// Assembly definitions.
		/// </summary>
		public bool AssemblyDefinitions
		{
			get => assemblyDefinitions;

			set
			{
				if (assemblyDefinitions != value)
				{
					assemblyDefinitions = value;
					EditorUtility.SetDirty(this);
				}
			}
		}

		[SerializeField]
		[FormerlySerializedAs("Current")]
		Theme current;

		/// <summary>
		/// Current theme.
		/// </summary>
		public Theme Current
		{
			get => current;

			set
			{
				if (value != current)
				{
					current = value;
					EditorUtility.SetDirty(this);
				}
			}
		}

		[SerializeField]
		string wrappersFolder = "Assets/UI Themes Wrappers";

		/// <summary>
		/// Folder to write generated wrappers files.
		/// </summary>
		public string WrappersFolder
		{
			get => wrappersFolder;

			set
			{
				if (value != wrappersFolder)
				{
					wrappersFolder = value;
					EditorUtility.SetDirty(this);
				}
			}
		}

		[SerializeField]
		string wrappersNamespace = "UIThemesWrappers";

		/// <summary>
		/// Wrappers namespace.
		/// </summary>
		public string WrappersNamespace
		{
			get => wrappersNamespace;

			set
			{
				if (value != wrappersNamespace)
				{
					wrappersNamespace = value;
					EditorUtility.SetDirty(this);
				}
			}
		}

		[SerializeField]
		bool generateWrappers = false;

		/// <summary>
		/// Generate wrappers.
		/// </summary>
		public bool GenerateWrappers
		{
			get => generateWrappers;

			set
			{
				if (value != generateWrappers)
				{
					generateWrappers = value;
					EditorUtility.SetDirty(this);
				}
			}
		}

		/// <summary>
		/// Should generate wrappers?
		/// </summary>
		public bool ShouldGenerateWrappers => generateWrappers && Directory.Exists(WrappersFolder) && !string.IsNullOrEmpty(WrappersNamespace);

		/// <summary>
		/// Is themes references exists?
		/// </summary>
		public static bool InstanceExists
		{
			get
			{
				var refs = Resources.FindObjectsOfTypeAll<ThemesReferences>();
				if (refs.Length > 0)
				{
					return true;
				}

				var guids = AssetDatabase.FindAssets("t:" + typeof(ThemesReferences).FullName);
				if (guids.Length > 0)
				{
					return true;
				}

				return false;
			}
		}

		/// <summary>
		/// Themes references.
		/// </summary>
		public static ThemesReferences Instance
		{
			get
			{
				var refs = Resources.FindObjectsOfTypeAll<ThemesReferences>();
				if (refs.Length > 0)
				{
					return refs[0];
				}

				var guids = AssetDatabase.FindAssets("t:" + typeof(ThemesReferences).FullName);
				if (guids.Length > 0)
				{
					var path = AssetDatabase.GUIDToAssetPath(guids[0]);
					return AssetDatabase.LoadAssetAtPath<ThemesReferences>(path);
				}

				return Create();
			}
		}

		/// <summary>
		/// Themes references.
		/// </summary>
		[Obsolete("Renamed to Instance.")]
		public static ThemesReferences Default => Instance;

		/// <summary>
		/// Create ThemesReferences.
		/// </summary>
		/// <returns>Created instance.</returns>
		static ThemesReferences Create()
		{
			var folder = ReferencesGUIDs.AssetsFolder;
			if (string.IsNullOrEmpty(folder))
			{
				return null;
			}

			var path = folder + Path.DirectorySeparatorChar + "UI Themes References.asset";
			return UtilitiesEditor.CreateScriptableObjectAsset<ThemesReferences>(path);
		}
	}
}
#endif