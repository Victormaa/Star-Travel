using UnityEditor;
using UnityEngine;

namespace Pathfinding {
	[CustomEditor(typeof(AILerp), true)]
	[CanEditMultipleObjects]
	public class AILerpEditor : EditorBase {
		protected override void Inspector () {
			PropertyField("speed");
			PropertyField("repathRate");
			PropertyField("canSearch");
			PropertyField("canMove");
			if (PropertyField("enableRotation")) {
				EditorGUI.indentLevel++;
				Popup("orientation", new [] { new GUIContent("ZAxisForward (for 3D games)"), new GUIContent("YAxisForward (for 2D games)") });
				PropertyField("rotationSpeed");
				EditorGUI.indentLevel--;
			}

			if (PropertyField("interpolatePathSwitches")) {
				EditorGUI.indentLevel++;
				PropertyField("switchPathInterpolationSpeed");
				EditorGUI.indentLevel--;
			}
		}
	}
}
