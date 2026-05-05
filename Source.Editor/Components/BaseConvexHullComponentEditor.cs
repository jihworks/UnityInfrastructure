// © 2026 Jong-il Hong
// Licensed under the MIT License. See LICENSE in the project root for license information.
//
// SPDX-License-Identifier: MIT

#nullable enable

using Jih.Unity.Infrastructure.Components;
using Jih.Unity.Infrastructure.Geometries;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Jih.Unity.Infrastructure.Editor.Components
{
    public abstract class BaseConvexHullComponentEditor : UnityEditor.Editor
    {
        GUIStyle? _invalidStyle;
        GUIStyle InvalidStyle
        {
            get
            {
                if (_invalidStyle is null)
                {
                    _invalidStyle = new GUIStyle(GUI.skin.label);
                    _invalidStyle.normal.textColor = Color.red;
                }
                return _invalidStyle;
            }
        }

        GUIStyle? _validStyle;
        GUIStyle ValidStyle
        {
            get
            {
                if (_validStyle is null)
                {
                    _validStyle = new GUIStyle(GUI.skin.label);
                    _validStyle.normal.textColor = Color.green;
                }
                return _validStyle;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BaseConvexHullComponent convexHullComponent = (BaseConvexHullComponent)target;

            string message;
            GUIStyle messageStyle;
            if (convexHullComponent.HasValidConvexHull())
            {
                message = "Valid";
                messageStyle = ValidStyle;
            }
            else
            {
                message = "Invalid";
                messageStyle = InvalidStyle;
            }

            GUILayout.Label(message, messageStyle);

            if (GUILayout.Button("Build"))
            {
                Undo.RecordObject(convexHullComponent, "Build ConvexHull");

                EmbeddableMesh convexHull = BaseConvexHullComponent.Editor_Build(convexHullComponent.gameObject, GenerateHull);
                convexHullComponent.Editor_SetConvexHull(convexHull);

                EditorUtility.SetDirty(convexHullComponent);
                PrefabUtility.RecordPrefabInstancePropertyModifications(convexHullComponent);
            }
        }

        protected abstract void GenerateHull(Vector3[] points, out List<Vector3> hullVertices, out List<int> hullTriangles);
    }
}
