// Adapted From https://github.com/marijnz/unity-toolbar-extender
// 23 January 2023


using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEngine.UIElements;

namespace JamKit
{
	public static class UnityToolbarExtender
	{
		private static Type _toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");

		private	static ScriptableObject _currentToolbar;

		private static Action _onToolbarGUIFarLeft;
		private static Action _onToolbarGUILeftOfPlayButton;
		private static Action _onToolbarGUIRightOfPlayButton;
		private static Action _onToolbarGUIFarRight;

		public static readonly List<Action> farLeft = new List<Action>();
		public static readonly List<Action> farRight = new List<Action>();
		public static readonly List<Action> rightOfPlayButton = new List<Action>();
		public static readonly List<Action> leftOfPlayButton = new List<Action>();

		[InitializeOnLoadMethod]
		static void InitializeOnLoad()
		{
			EditorApplication.update -= OnUpdate;
			EditorApplication.update += OnUpdate;

			_onToolbarGUIFarLeft = DrawGUIFarLeft;
			_onToolbarGUILeftOfPlayButton = DrawGUILeftOfPlayButton;
			_onToolbarGUIFarRight = DrawGUIFarRight;
			_onToolbarGUIRightOfPlayButton = DrawGUIRightOfPlayButton;
		}

		private static void OnUpdate()
		{
			if (_currentToolbar == null)
			{
				var toolbars = Resources.FindObjectsOfTypeAll(_toolbarType);
				_currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
				if (_currentToolbar != null)
				{
					var root = _currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
					if (root != null)
					{
						object rawRoot = root.GetValue(_currentToolbar);
						if (rawRoot != null)
						{
							VisualElement mRoot = rawRoot as VisualElement;
							if (mRoot != null)
							{
								RegisterCallback("ToolbarZoneLeftAlign", mRoot, Justify.FlexStart,
									_onToolbarGUIFarLeft);
								RegisterCallback("ToolbarZoneLeftAlign", mRoot, Justify.FlexEnd,
									_onToolbarGUILeftOfPlayButton);
								RegisterCallback("ToolbarZoneRightAlign", mRoot, Justify.FlexEnd,
									_onToolbarGUIFarRight);
								RegisterCallback("ToolbarZoneRightAlign", mRoot, Justify.FlexStart,
									_onToolbarGUIRightOfPlayButton);
							}
						}
					}
				}
			}
		}

		private	static void RegisterCallback(string root,	VisualElement mRoot, Justify justify, Action cb)
		{
			var toolbarZone = mRoot.Q(root);

			var parent = new VisualElement()
			{
				style =
				{
					flexGrow = 1,
					flexDirection = FlexDirection.Row,
					justifyContent = justify
				}
			};
			var container = new IMGUIContainer();
			container.onGUIHandler += () => { cb?.Invoke(); };
			parent.Add(container);
			toolbarZone.Add(parent);
		}

		private static void DrawGUIFarLeft()
		{
			GUILayout.BeginHorizontal();
			foreach (var callback in farLeft)
			{
				callback?.Invoke();
			}
			GUILayout.EndHorizontal();
		}

		private static void DrawGUIFarRight()
		{
			GUILayout.BeginHorizontal();
			foreach (var callback in farRight)
			{
				callback?.Invoke();
			}
			GUILayout.EndHorizontal();
		}

		private static void DrawGUIRightOfPlayButton()
		{
			GUILayout.BeginHorizontal();
			foreach (var callback in rightOfPlayButton)
			{
				callback?.Invoke();
			}
			GUILayout.EndHorizontal();
		}

		private static void DrawGUILeftOfPlayButton()
		{
			GUILayout.BeginHorizontal();
			foreach (var callback in leftOfPlayButton)
			{
				callback?.Invoke();
			}
			GUILayout.EndHorizontal();
		}
	}
}
