#if UNITY_EDITOR && UNITY_2018_3_OR_NEWER
namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEditor;
	using UnityEngine;
#if UIWIDGETS_TMPRO_SUPPORT && UIWIDGETS_TMPRO_4_0_OR_NEWER
	using FontAsset = UnityEngine.TextCore.Text.FontAsset;
#elif UIWIDGETS_TMPRO_SUPPORT
	using FontAsset = TMPro.TMP_FontAsset;
#else
	using FontAsset = UnityEngine.ScriptableObject;
#endif

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
			var installed_symbol = ScriptingDefineSymbols.GetState("UIWIDGETS_INSTALLED");

			installed_symbol.Enable();

			if (TMPro.Available && !TMPro.State.All)
			{
				TMPro.EnableForAll();
			}

			AssemblyDefinitionGenerator.Create();

			WidgetsReferences.Instance.InstantiateWidgets = WidgetsReferences.Instance.InstantiateWidgets;

			AssetDatabase.Refresh();
		}

		static ProjectSettings()
		{
			var installed = ScriptingDefineSymbols.GetState("UIWIDGETS_INSTALLED");
			var first_run = !WidgetsReferences.InstanceExists;
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
				WidgetsReferences.Instance.AssemblyDefinitions = true;
				assemblies.Disable();
				AssetDatabase.Refresh();
			}

			var instantiate = ScriptingDefineSymbols.GetState("UIWIDGETS_INSTANTIATE_PREFABS");
			if (instantiate.Any)
			{
				instantiate.Disable();
				WidgetsReferences.Instance.InstantiateWidgets = true;
				AssetDatabase.Refresh();
			}

			ScriptingDefineSymbols.Rename("I2_LOCALIZATION_SUPPORT", "UIWIDGETS_I2LOCALIZATION_SUPPORT");

			var settings = WidgetsReferences.Instance;
			if (settings != null)
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

			[DomainReloadExclude]
			public static readonly GUIContent InstantiateWidgets = new GUIContent("Instantiate Widgets");

			[DomainReloadExclude]
			public static readonly GUIContent StylesLabel = new GUIContent("Styles or Themes");

			[DomainReloadExclude]
			public static readonly GUIContent AttachThemeLabel = new GUIContent("Attach Default Theme", "Attach default Theme to the widgets created from the menu.");

			[DomainReloadExclude]
			public static readonly GUIContent UseWhiteSprite = new GUIContent("Use White Sprite");

			[DomainReloadExclude]
			public static readonly GUIContent TMPro = new GUIContent("TextMeshPro Support");

			[DomainReloadExclude]
			public static readonly GUIContent DataBind = new GUIContent("Data Bind for Unity Support");

			[DomainReloadExclude]
			public static readonly GUIContent I2Localization = new GUIContent("I2 Localization Support");

			[DomainReloadExclude]
			public static readonly GUIContent UIThemesWrappersSettings = new GUIContent("UI Themes Wrappers Settings");

			[DomainReloadExclude]
			public static readonly GUIContent UIThemesWrappersFolder = new GUIContent("   Folder");

			[DomainReloadExclude]
			public static readonly GUIContent UIThemesWrappersNamespace = new GUIContent("   Namespace");

			[DomainReloadExclude]
			public static readonly GUIContent UIThemesWrappersGenerate = new GUIContent("   Generate");
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

		/// <summary>
		/// Enable/disable assembly definitions.
		/// </summary>
		[DomainReloadExclude]
		public static readonly ScriptableSetting AssemblyDefinitions = new ScriptableSetting(
			() => WidgetsReferences.Instance.AssemblyDefinitions && AssemblyDefinitionGenerator.IsCreated,
			state =>
			{
				WidgetsReferences.Instance.AssemblyDefinitions = state;
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
		/// Toggle widgets instance type.
		/// </summary>
		[DomainReloadExclude]
		public static readonly ScriptableSetting InstantiateWidgets = new ScriptableSetting(
			() => WidgetsReferences.Instance.InstantiateWidgets,
			x => WidgetsReferences.Instance.InstantiateWidgets = x,
			enabledText: "Prefabs",
			disabledText: "Copies");

		/// <summary>
		/// Enable/disable legacy styles.
		/// </summary>
		[DomainReloadExclude]
		public static readonly ScriptingDefineSymbol LegacyStyles = new ScriptingDefineSymbol(
			"UIWIDGETS_LEGACY_STYLE",
			true,
			enabledText: "Styles (obsolete)",
			disabledText: "UI Themes");

		/// <summary>
		/// Enable/disable TextMeshPro support.
		/// </summary>
		[DomainReloadExclude]
		public static readonly ScriptingDefineSymbol TMPro = new ScriptingDefineSymbol(
			"UIWIDGETS_TMPRO_SUPPORT",
			UtilitiesEditor.GetType("TMPro.TextMeshProUGUI") != null);

		/// <summary>
		/// Enable/disable DataBind support.
		/// </summary>
		[DomainReloadExclude]
		public static readonly ScriptingDefineSymbol DataBind = new ScriptingDefineSymbol(
			"UIWIDGETS_DATABIND_SUPPORT",
			UtilitiesEditor.GetType("Slash.Unity.DataBind.Core.Presentation.DataProvider") != null);

		[DomainReloadExclude]
		static readonly string DataBindWarning = "Data Bind for Unity does not have assembly definitions by default.\n" +
			"You must create them and add references to UIWidgets.asmdef," +
			" UIWidgets.Editor.asmdef, and UIWidgets.Samples.asmdef.\n" +
			"Or you can disable assembly definitions.";

		/// <summary>
		/// Enable/disable I2Localization support.
		/// </summary>
		[DomainReloadExclude]
		public static readonly ScriptingDefineSymbol I2Localization = new ScriptingDefineSymbol(
			"UIWIDGETS_I2LOCALIZATION_SUPPORT",
			UtilitiesEditor.GetType("I2.Loc.LocalizationManager") != null);

		[DomainReloadExclude]
		static readonly string I2LocalizationWarning = "I2 Localization does not have assembly definitions by default.\n" +
			"You must create them and add references to UIWidgets.asmdef," +
			" UIWidgets.Editor.asmdef, and UIWidgets.Samples.asmdef.\n" +
			"Or you can disable assembly definitions.";

		[DomainReloadExclude]
		static readonly string UseWhiteSpriteDescription = "Sets white sprite for the Image components without sprite.\n" +
			"Prevents rare bugs when such Images are displayed as black.";

		/// <summary>
		/// Attach default theme to the widgets created from menu.
		/// </summary>
		[DomainReloadExclude]
		public static readonly ScriptableSetting AttachTheme = new ScriptableSetting(
			() => WidgetsReferences.Instance.AttachTheme,
			x => WidgetsReferences.Instance.AttachTheme = x);

		/// <summary>
		/// Use white sprite.
		/// </summary>
		[DomainReloadExclude]
		public static readonly ScriptableSetting UseWhiteSprite = new ScriptableSetting(
			() => WidgetsReferences.Instance.UseWhiteSprite,
			x => WidgetsReferences.Instance.UseWhiteSprite = x);

		[DomainReloadExclude]
		static readonly GUILayoutOption[] NameOptions = new GUILayoutOption[] { GUILayout.Width(170) };

		[DomainReloadExclude]
		static readonly GUILayoutOption[] StatusOptions = new GUILayoutOption[] { GUILayout.Width(170) };

		[DomainReloadExclude]
		static readonly Action<ISetting> TMProInfo = (ISetting setting) =>
		{
			if (setting.Available && setting.Enabled)
			{
				WidgetsReferences.Instance.DefaultFont = EditorGUILayout.ObjectField("Default Font", WidgetsReferences.Instance.DefaultFont, typeof(FontAsset), false) as FontAsset;

				ShowHelpBox(
					"You can replace all Unity text with TMPro text by using" +
					" the context menu \"UI / New UI Widgets / Replace Unity Text with TextMeshPro\"" +
					" or by using the menu \"Window / New UI Widgets / Replace Unity Text with TextMeshPro\".",
					MessageType.Info);
			}
		};

		[DomainReloadExclude]
		static readonly Action<ISetting> DataBindInfo = (ISetting setting) =>
		{
			if (!setting.Available)
			{
				return;
			}

			if (!AssemblyDefinitions.Enabled)
			{
				return;
			}

			ShowHelpBox(DataBindWarning, MessageType.Warning);
		};

		[DomainReloadExclude]
		static readonly Action<ISetting> I2LocalizationInfo = (ISetting setting) =>
		{
			if (!setting.Available)
			{
				return;
			}

			if (!AssemblyDefinitions.Enabled)
			{
				return;
			}

			ShowHelpBox(I2LocalizationWarning, MessageType.Warning);
		};

		[DomainReloadExclude]
		static readonly Action<ISetting> UseWhiteSpriteInfo = (ISetting setting) =>
		{
			ShowHelpBox(UseWhiteSpriteDescription, MessageType.Info);
		};

		[DomainReloadExclude]
		static readonly List<Block> Blocks = new List<Block>()
		{
			new Block(AssemblyDefinitions, Labels.AssemblyDefinitions),
			new Block(InstantiateWidgets, Labels.InstantiateWidgets, buttonDisable: "Create Copies", buttonEnable: "Create Prefabs"),
			new Block(LegacyStyles, Labels.StylesLabel, buttonDisable: "Use UI Themes", buttonEnable: "Use Legacy Styles"),
			new Block(AttachTheme, Labels.AttachThemeLabel),
			new Block(UseWhiteSprite, Labels.UseWhiteSprite, UseWhiteSpriteInfo),

			// ScriptsRecompile.SetStatus(ReferenceGUID.TMProStatus, ScriptsRecompile.StatusSymbolsAdded);
			new Block(TMPro, Labels.TMPro, TMProInfo),

			// ScriptsRecompile.SetStatus(ReferenceGUID.DataBindStatus, ScriptsRecompile.StatusSymbolsAdded);
			new Block(DataBind, Labels.DataBind, DataBindInfo),

			// ScriptsRecompile.SetStatus(ReferenceGUID.I2LocalizationStatus, ScriptsRecompile.StatusSymbolsAdded);
			new Block(I2Localization, Labels.I2Localization, I2LocalizationInfo),
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
			var settings = UIThemes.Editor.ThemesReferences.Instance;
			var valid = true;

			EditorGUILayout.LabelField(Labels.UIThemesWrappersSettings, NameOptions);

			EditorGUILayout.BeginHorizontal();
			settings.WrappersFolder = EditorGUILayout.TextField(Labels.UIThemesWrappersFolder, settings.WrappersFolder);
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
				EditorGUI.indentLevel += 1;
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.HelpBox("Specified directory is not exists.", MessageType.Error);
				if (GUILayout.Button("Create Directory"))
				{
					System.IO.Directory.CreateDirectory(settings.WrappersFolder);
					AssetDatabase.Refresh();
				}

				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel -= 1;
			}

			settings.WrappersNamespace = EditorGUILayout.TextField(Labels.UIThemesWrappersNamespace, settings.WrappersNamespace);
			if (string.IsNullOrEmpty(settings.WrappersNamespace))
			{
				valid = false;
				ShowHelpBox("Namespace is not specified.", MessageType.Error);
			}

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup(!valid);
			settings.GenerateWrappers = EditorGUILayout.Toggle(Labels.UIThemesWrappersGenerate, settings.GenerateWrappers);

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
			output.Add("UIThemes");

			if (TMPro.Available)
			{
				output.Add("Unity.TextMeshPro");
			}

			if (DataBind.Enabled)
			{
				Debug.LogWarning(DataBindWarning);
			}

			if (I2Localization.Enabled)
			{
				Debug.LogWarning(I2LocalizationWarning);
			}

			if (UtilitiesEditor.GetType("UnityEngine.InputSystem.InputSystem") != null)
			{
				output.Add("Unity.InputSystem");
			}
		}

		/// <summary>
		/// Create settings provider.
		/// </summary>
		/// <returns>Settings provider.</returns>
		[SettingsProvider]
		public static SettingsProvider CreateSettingsProvider()
		{
			var provider = new SettingsProvider("Project/New UI Widgets", SettingsScope.Project)
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