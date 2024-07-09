#if UNITY_EDITOR
namespace UIThemes.Editor
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Project settings.
	/// </summary>
	[InitializeOnLoad]
	public static class ProjectSettings
	{
		class Init : AssetPostprocessor
		{
			[DomainReloadExclude]
			public static bool IsFirstRun;

			#if UNITY_2022_3_OR_NEWER
			public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
			#else
			public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
			#endif
			{
				if (IsFirstRun)
				{
					FirstRun();
					IsFirstRun = false;
				}
			}
		}

		static void FirstRun()
		{
			var installed = ScriptingDefineSymbols.GetState("UITHEMES_INSTALLED");
			installed.Enable();

			if (TMPro.Available && !TMPro.State.All)
			{
				TMPro.EnableForAll();
			}

			AssemblyDefinitionGenerator.Create();

			ThemesReferences.Instance.AssemblyDefinitions = ThemesReferences.Instance.AssemblyDefinitions;

			if (ThemesOnly)
			{
				AssetDatabase.Refresh();
			}
		}

		static ProjectSettings()
		{
			var installed = ScriptingDefineSymbols.GetState("UITHEMES_INSTALLED");
			var first_run = !ThemesReferences.InstanceExists;
			if (AssemblyDefinitionGenerator.IsCreated || installed.Any)
			{
				first_run = false;
			}

			if (first_run)
			{
				Init.IsFirstRun = true;
				return;
			}

			var assemblies = ScriptingDefineSymbols.GetState("UIWIDGETS_ASMDEF");
			if (assemblies.Any)
			{
				ThemesReferences.Instance.AssemblyDefinitions = true;
				assemblies.Disable();
				AssetDatabase.Refresh();
			}

			var settings = ThemesReferences.Instance;
			if (ThemesOnly && (settings != null))
			{
				if (AssemblyDefinitionGenerator.IsCreated)
				{
					settings.AssemblyDefinitions = true;
				}
				else if (settings.AssemblyDefinitions)
				{
					AssemblyDefinitionGenerator.Create();
				}
			}

			if (!installed.All)
			{
				installed.Enable();
			}
		}

		class Labels
		{
			private Labels()
			{
			}

			[DomainReloadExclude]
			public static readonly GUIContent AssemblyDefinitions = new GUIContent("Assembly Definitions");

			/// <summary>
			/// TextMeshPro label.
			/// </summary>
			[DomainReloadExclude]
			public static readonly GUIContent TMPro = new GUIContent("TextMeshPro Support");

			[DomainReloadExclude]
			public static readonly GUIContent WrappersSettings = new GUIContent("Wrappers Settings");

			[DomainReloadExclude]
			public static readonly GUIContent WrappersFolder = new GUIContent("   Folder");

			[DomainReloadExclude]
			public static readonly GUIContent WrappersNamespace = new GUIContent("   Namespace");

			[DomainReloadExclude]
			public static readonly GUIContent WrappersGenerate = new GUIContent("   Generate");
		}

		class Block
		{
			readonly ISetting symbol;

			readonly GUIContent label;

			readonly Action<ISetting> info;

			readonly string buttonEnable;

			readonly string buttonDisable;

			public Block(ISetting symbol, GUIContent label, Action<ISetting> info = null, string buttonEnable = "Enable", string buttonDisable = "Disable")
			{
				this.symbol = symbol;
				this.label = label;
				this.info = info;

				this.buttonEnable = buttonEnable;
				this.buttonDisable = buttonDisable;
			}

			void EnableForAll()
			{
				if (symbol.Available && symbol.Enabled && !symbol.IsFullSupport)
				{
					EditorGUILayout.BeginVertical();
					ShowHelpBox("Feature is not enabled for all BuildTargets.", MessageType.Info);

					if (GUILayout.Button("Enable for All", GUILayout.ExpandWidth(true)))
					{
						symbol.EnableForAll();
					}

					EditorGUILayout.EndVertical();
				}
			}

			public void Show()
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(label, NameOptions);

				if (symbol.IsFullSupport)
				{
					var status = new GUIContent(symbol.Status);
					EditorGUILayout.LabelField(status, StatusOptions);
				}
				else
				{
					var color = EditorStyles.label.normal.textColor;
					EditorStyles.label.normal.textColor = Color.red;

					var status = new GUIContent(symbol.Status, "Support is not enabled for all BuildTargets.");
					EditorGUILayout.LabelField(status, StatusOptions);

					EditorStyles.label.normal.textColor = color;
				}

				if (symbol.Available)
				{
					if (symbol.Enabled)
					{
						if (GUILayout.Button(buttonDisable))
						{
							symbol.Enabled = false;
						}
					}
					else
					{
						if (GUILayout.Button(buttonEnable))
						{
							symbol.Enabled = true;
						}
					}
				}

				EditorGUILayout.EndHorizontal();

				if (symbol.Available)
				{
					EnableForAll();

					if (info != null)
					{
						info(symbol);
					}
				}
			}
		}

		[DomainReloadExclude]
		static readonly bool ThemesOnly = false;

		/// <summary>
		/// Enable/disable assembly definitions.
		/// </summary>
		[DomainReloadExclude]
		public static readonly ScriptableSetting AssemblyDefinitions = new ScriptableSetting(
			() => ThemesReferences.Instance.AssemblyDefinitions && AssemblyDefinitionGenerator.IsCreated,
			state =>
			{
				ThemesReferences.Instance.AssemblyDefinitions = state;
				if (state)
				{
					AssemblyDefinitionGenerator.Create();
				}
				else
				{
					AssemblyDefinitionGenerator.Delete();
				}
			});

		/// <summary>
		/// Enable/disable TextMeshPro support.
		/// </summary>
		[DomainReloadExclude]
		public static readonly ScriptingDefineSymbol TMPro = new ScriptingDefineSymbol(
			"UIWIDGETS_TMPRO_SUPPORT",
			UtilitiesEditor.GetType("TMPro.TextMeshProUGUI") != null);

		[DomainReloadExclude]
		static readonly GUILayoutOption[] NameOptions = new GUILayoutOption[] { GUILayout.Width(170) };

		[DomainReloadExclude]
		static readonly GUILayoutOption[] StatusOptions = new GUILayoutOption[] { GUILayout.Width(170) };

		[DomainReloadExclude]
		static readonly List<Block> Blocks = new List<Block>()
		{
			new Block(AssemblyDefinitions, Labels.AssemblyDefinitions),
			new Block(TMPro, Labels.TMPro),
		};

		static void ShowHelpBox(string text, MessageType type)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUI.indentLevel += 1;
			EditorGUILayout.HelpBox(text, type);
			EditorGUI.indentLevel -= 1;
			EditorGUILayout.EndHorizontal();
		}

		static void WrappersSettings()
		{
			var settings = ThemesReferences.Instance;
			var valid = true;

			EditorGUILayout.LabelField(Labels.WrappersSettings, NameOptions);

			EditorGUILayout.BeginHorizontal();
			settings.WrappersFolder = EditorGUILayout.TextField(Labels.WrappersFolder, settings.WrappersFolder);
			if (GUILayout.Button("...", GUILayout.Width(30f)))
			{
				var folder = UIThemes.Editor.Utilities.SelectAssetsPath(settings.WrappersFolder, "Select a directory for wrappers scripts");
				if (string.IsNullOrEmpty(folder))
				{
					valid = false;
				}
				else
				{
					settings.WrappersFolder = folder;
				}
			}

			EditorGUILayout.EndHorizontal();

			if (!System.IO.Directory.Exists(settings.WrappersFolder))
			{
				ShowHelpBox("Specified directory is not exists.", MessageType.Error);
			}

			settings.WrappersNamespace = EditorGUILayout.TextField(Labels.WrappersNamespace, settings.WrappersNamespace);
			if (string.IsNullOrEmpty(settings.WrappersNamespace))
			{
				valid = false;
				ShowHelpBox("Namespace is not specified.", MessageType.Error);
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup(!valid);
			settings.GenerateWrappers = EditorGUILayout.Toggle(Labels.WrappersGenerate, settings.GenerateWrappers);

			var style = GUI.skin.GetStyle("HelpBox");
			style.richText = true;
			EditorGUILayout.LabelField(string.Empty, "Automatically generate wrapper scripts for properties which available only via reflection after using the <i>Theme Attach</i> command.", style);
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}

		/// <summary>
		/// Get required assemblies.
		/// </summary>
		/// <param name="output">Assemblies name list.</param>
		public static void GetAssemblies(List<string> output)
		{
			if (TMPro.Available)
			{
				output.Add("Unity.TextMeshPro");
			}
		}

		/// <summary>
		/// Create settings provider.
		/// </summary>
		/// <returns>Settings provider.</returns>
		#if !UIWIDGETS_INSTALLED
		[SettingsProvider]
		#endif
		public static SettingsProvider CreateSettingsProvider()
		{
			var provider = new SettingsProvider("Project/UI Themes", SettingsScope.Project)
			{
				guiHandler = (searchContext) =>
				{
					foreach (var block in Blocks)
					{
						block.Show();
						EditorGUILayout.Space(6);
					}

					WrappersSettings();
				},

				keywords = SettingsProvider.GetSearchKeywordsFromGUIContentProperties<Labels>(),
			};

			return provider;
		}
	}
}
#endif