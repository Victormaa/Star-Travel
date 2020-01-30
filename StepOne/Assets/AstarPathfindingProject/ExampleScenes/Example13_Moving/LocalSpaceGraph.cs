using UnityEngine;
namespace Pathfinding {
	using Pathfinding.Util;

	/** Helper for #Pathfinding.Examples.LocalSpaceRichAI */
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_local_space_graph.php")]
	public class LocalSpaceGraph : VersionedMonoBehaviour {
		Matrix4x4 originalMatrix;
		MutableGraphTransform graphTransform;
		public GraphTransform transformation { get { return graphTransform; } }

		void Start () {
			originalMatrix = transform.worldToLocalMatrix;
			transform.hasChanged = true;
			Refresh();
		}

		public void Refresh () {
			// Avoid updating the GraphTransform if the object has not moved
			if (transform.hasChanged) {
				graphTransform.SetMatrix(transform.localToWorldMatrix * originalMatrix);
				transform.hasChanged = false;
			}
		}
	}
}
