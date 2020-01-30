using UnityEngine;
using UnityEditor;

namespace Pathfinding {
	[CustomEditor(typeof(NavmeshCut))]
	[CanEditMultipleObjects]
	public class NavmeshCutEditor : EditorBase {
		protected override void Inspector () {
			EditorGUI.BeginChangeCheck();
			var type = FindProperty("type");
			var circleResolution = FindProperty("circleResolution");
			PropertyField("type");

			if (!type.hasMultipleDifferentValues) {
				switch ((NavmeshCut.MeshType)type.intValue) {
				case NavmeshCut.MeshType.Circle:
					PropertyField("circleRadius");
					PropertyField("circleResolution");

					if (circleResolution.intValue >= 20) {
						EditorGUILayout.HelpBox("Be careful with large values. It is often better with a relatively low resolution since it generates cleaner navmeshes with fewer nodes.", MessageType.Warning);
					}
					break;
				case NavmeshCut.MeshType.Rectangle:
					PropertyField("rectangleSize");
					break;
				case NavmeshCut.MeshType.CustomMesh:
					PropertyField("mesh");
					PropertyField("meshScale");
					EditorGUILayout.HelpBox("This mesh should be a planar surface. Take a look at the documentation for an example.", MessageType.Info);
					break;
				}
			}

			PropertyField("height");
			Clamp("height", 0);

			PropertyField("center");

			EditorGUILayout.Separator();
			PropertyField("updateDistance");
			if (PropertyField("useRotationAndScale")) {
				EditorGUI.indentLevel++;
				PropertyField("updateRotationDistance");
				Clamp("updateRotationDistance", 0, 180);
				EditorGUI.indentLevel--;
			}

			PropertyField("isDual");
			PropertyField("cutsAddedGeom");

			serializedObject.ApplyModifiedProperties();

			if (EditorGUI.EndChangeCheck()) {
				foreach (NavmeshCut tg in targets) {
					tg.ForceUpdate();
				}
			}
		}
	}
}
