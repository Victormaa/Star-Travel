using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding {
#if !ASTAR_NO_RECAST_GRAPH || !ASTAR_NO_NAVMESH_GRAPH
	/** Helper for navmesh cut objects.
	 *
	 * \astarpro
	 * \deprecated Use #AstarPath.navmeshUpdates instead
	 */
	[System.Obsolete("Use AstarPath.navmeshUpdates instead")]
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_tile_handler_helper.php")]
	public class TileHandlerHelper : VersionedMonoBehaviour {
		/** How often to check if an update needs to be done (real seconds between checks).
		 *
		 */
		public float updateInterval {
			get { return AstarPath.active.navmeshUpdates.updateInterval; }
			set { AstarPath.active.navmeshUpdates.updateInterval = value; }
		}

		/** Use the specified handler, will create one at start if not called */
		[System.Obsolete("All navmesh/recast graphs now use navmesh cutting")]
		public void UseSpecifiedHandler (TileHandler newHandler) {
			throw new System.Exception("All navmesh/recast graphs now use navmesh cutting");
		}

		/** Discards all pending updates caused by moved or modified navmesh cuts */
		public void DiscardPending () {
			AstarPath.active.navmeshUpdates.DiscardPending();
		}

		/** Checks all NavmeshCut instances and updates graphs if needed. */
		public void ForceUpdate () {
			AstarPath.active.navmeshUpdates.ForceUpdate();
		}
	}
#endif
}
