using UnityEngine;
using System.Collections.Generic;
using Pathfinding.Util;
using Pathfinding.Serialization;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Pathfinding {
	/** Helper for navmesh cut objects.
	 * Responsible for keeping track of which navmesh cuts have moved and coordinating graph updates to account for those changes.
	 *
	 * \see \ref navmeshcutting
	 * \see #AstarPath.navmeshUpdates
	 * \see #Pathfinding.NavmeshBase.enableNavmeshCutting
	 *
	 * \astarpro
	 */
	[System.Serializable]
	public class NavmeshUpdates {
		/** How often to check if an update needs to be done (real seconds between checks).
		 * For worlds with a very large number of NavmeshCut objects, it might be bad for performance to do this check every frame.
		 * If you think this is a performance penalty, increase this number to check less often.
		 *
		 * For almost all games, this can be kept at 0.
		 *
		 * If negative, no updates will be done. They must be manually triggered using #ForceUpdate.
		 *
		 * \snippet MiscSnippets.cs NavmeshUpdates.updateInterval
		 *
		 * You can also find this in the AstarPath inspector under Settings.
		 * \shadowimage{navmeshcut_update_interval.png}
		 */
		public float updateInterval;

#if (!ASTAR_NO_RECAST_GRAPH || !ASTAR_NO_NAVMESH_GRAPH) && !AstarFree
		/** Last time navmesh cuts were applied */
		float lastUpdateTime = float.NegativeInfinity;

		/** Stores navmesh cutting related data for a single graph */
		internal class NavmeshUpdateSettings {
			public TileHandler handler;
			public readonly List<IntRect> forcedReloadRects = new List<IntRect>();
			readonly NavmeshBase graph;

			public NavmeshUpdateSettings(NavmeshBase graph) {
				this.graph = graph;
			}

			public void Refresh (bool forceCreate = false) {
				if (!graph.enableNavmeshCutting) {
					if (handler != null) {
						handler.cuts.Clear();
						handler.ReloadInBounds(new IntRect(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue));
						// Make sure the updates are applied immediately.
						// This is important because if navmesh cutting is enabled immediately after this
						// then it will call CreateTileTypesFromGraph, and we need to ensure that it is not
						// calling that when the graph still has cuts in it as they will then be baked in.
						AstarPath.active.FlushGraphUpdates();
						AstarPath.active.FlushWorkItems();

						forcedReloadRects.ClearFast();
						handler = null;
					}
				} else if ((handler == null && (forceCreate || NavmeshClipper.allEnabled.Count > 0)) || (handler != null && !handler.isValid)) {
					// Note: Only create a handler if there are any navmesh cuts in the scene.
					// We don't want to waste a lot of memory if navmesh cutting isn't actually used for anything
					// and even more important: we don't want to do any sporadic updates to the graph which
					// may clear the graph's tags or change it's structure (e.g from the delaunay optimization in the TileHandler).

					// The tile handler is invalid (or doesn't exist), so re-create it
					handler = new TileHandler(graph);
					for (int i = 0; i < NavmeshClipper.allEnabled.Count; i++) AddClipper(NavmeshClipper.allEnabled[i]);
					handler.CreateTileTypesFromGraph();

					// Reload in huge bounds. This will cause all tiles to be updated.
					forcedReloadRects.Add(new IntRect(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue));
				}
			}

			/** Called when some tiles in a recast graph have been completely recalculated (e.g from scanning the graph) */
			public void OnRecalculatedTiles (NavmeshTile[] tiles) {
				Refresh();
				if (handler != null) handler.OnRecalculatedTiles(tiles);
			}

			/** Called when a NavmeshCut or NavmeshAdd is enabled */
			public void AddClipper (NavmeshClipper obj) {
				// Without the forceCreate parameter set to true then no handler will be created
				// because there are no clippers in the scene yet. However one is being added right now.
				Refresh(true);
				if (handler == null) return;
				var graphSpaceBounds = obj.GetBounds(handler.graph.transform);
				var touchingTiles = handler.graph.GetTouchingTilesInGraphSpace(graphSpaceBounds);
				handler.cuts.Add(obj, touchingTiles);
			}

			/** Called when a NavmeshCut or NavmeshAdd is disabled */
			public void RemoveClipper (NavmeshClipper obj) {
				Refresh();
				if (handler == null) return;
				var root = handler.cuts.GetRoot(obj);

				if (root != null) {
					forcedReloadRects.Add(root.previousBounds);
					handler.cuts.Remove(obj);
				}
			}
		}

		internal void OnEnable () {
			NavmeshClipper.AddEnableCallback(HandleOnEnableCallback, HandleOnDisableCallback);
		}

		internal void OnDisable () {
			NavmeshClipper.RemoveEnableCallback(HandleOnEnableCallback, HandleOnDisableCallback);
		}

		/** Discards all pending updates caused by moved or modified navmesh cuts */
		public void DiscardPending () {
			for (int i = 0; i < NavmeshClipper.allEnabled.Count; i++) {
				NavmeshClipper.allEnabled[i].NotifyUpdated();
			}

			var graphs = AstarPath.active.graphs;
			for (int i = 0; i < graphs.Length; i++) {
				var navmeshBase = graphs[i] as NavmeshBase;
				if (navmeshBase != null) navmeshBase.navmeshUpdateData.forcedReloadRects.Clear();
			}
		}

		/** Called when a NavmeshCut or NavmeshAdd is enabled */
		void HandleOnEnableCallback (NavmeshClipper obj) {
			var graphs = AstarPath.active.graphs;

			for (int i = 0; i < graphs.Length; i++) {
				var navmeshBase = graphs[i] as NavmeshBase;
				if (navmeshBase != null) navmeshBase.navmeshUpdateData.AddClipper(obj);
			}
			obj.ForceUpdate();
		}

		/** Called when a NavmeshCut or NavmeshAdd is disabled */
		void HandleOnDisableCallback (NavmeshClipper obj) {
			var graphs = AstarPath.active.graphs;

			for (int i = 0; i < graphs.Length; i++) {
				var navmeshBase = graphs[i] as NavmeshBase;
				if (navmeshBase != null) navmeshBase.navmeshUpdateData.RemoveClipper(obj);
			}
			lastUpdateTime = float.NegativeInfinity;
		}

		/** Update is called once per frame */
		internal void Update () {
			if (AstarPath.active.isScanning) return;
			Profiler.BeginSample("Navmesh cutting");
			bool anyInvalidHandlers = false;
			var graphs = AstarPath.active.graphs;
			for (int i = 0; i < graphs.Length; i++) {
				var navmeshBase = graphs[i] as NavmeshBase;
				if (navmeshBase != null) {
					navmeshBase.navmeshUpdateData.Refresh();
					anyInvalidHandlers = navmeshBase.navmeshUpdateData.forcedReloadRects.Count > 0;
				}
			}

			if ((updateInterval >= 0 && Time.realtimeSinceStartup - lastUpdateTime > updateInterval) || anyInvalidHandlers) {
				ForceUpdate();
			}
			Profiler.EndSample();
		}

		/** Checks all NavmeshCut instances and updates graphs if needed.
		 * \note This schedules updates for all necessary tiles to happen as soon as possible.
		 * The pathfinding threads will continue to calculate the paths that they were calculating when this function
		 * was called and then they will be paused and the graph updates will be carried out (this may be several frames into the
		 * future and the graph updates themselves may take several frames to complete).
		 * If you want to force all navmesh cutting to be completed in a single frame call this method
		 * and immediately after call AstarPath.FlushWorkItems.
		 *
		 * \snippet MiscSnippets.cs NavmeshUpdates.ForceUpdate
		 */
		public void ForceUpdate () {
			lastUpdateTime = Time.realtimeSinceStartup;

			List<NavmeshClipper> hasBeenUpdated = null;

			var graphs = AstarPath.active.graphs;
			for (int graphIndex = 0; graphIndex < graphs.Length; graphIndex++) {
				var navmeshBase = graphs[graphIndex] as NavmeshBase;
				if (navmeshBase == null) continue;

				// Done in Update as well, but users may call ForceUpdate directly
				navmeshBase.navmeshUpdateData.Refresh();

				var handler = navmeshBase.navmeshUpdateData.handler;

				if (handler == null) continue;

				var forcedReloadRects = navmeshBase.navmeshUpdateData.forcedReloadRects;

				// Get all navmesh cuts in the scene
				var allCuts = handler.cuts.AllItems;

				if (forcedReloadRects.Count == 0) {
					bool any = false;

					// Check if any navmesh cuts need updating
					for (var cut = allCuts; cut != null; cut = cut.next) {
						if (cut.obj.RequiresUpdate()) {
							any = true;
							break;
						}
					}

					// Nothing needs to be done for now
					if (!any) continue;
				}

				// Start batching tile updates which is good for performance
				// if we are updating a lot of them
				handler.StartBatchLoad();

				for (int i = 0; i < forcedReloadRects.Count; i++) {
					handler.ReloadInBounds(forcedReloadRects[i]);
				}
				forcedReloadRects.ClearFast();

				if (hasBeenUpdated == null) hasBeenUpdated = ListPool<NavmeshClipper>.Claim ();

				// Reload all bounds touching the previous bounds and current bounds
				// of navmesh cuts that have moved or changed in some other way
				for (var cut = allCuts; cut != null; cut = cut.next) {
					if (cut.obj.RequiresUpdate()) {
						// Make sure the tile where it was is updated
						handler.ReloadInBounds(cut.previousBounds);

						var newGraphSpaceBounds = cut.obj.GetBounds(handler.graph.transform);
						var newTouchingTiles = handler.graph.GetTouchingTilesInGraphSpace(newGraphSpaceBounds);
						handler.cuts.Move(cut.obj, newTouchingTiles);
						handler.ReloadInBounds(newTouchingTiles);

						hasBeenUpdated.Add(cut.obj);
					}
				}

				handler.EndBatchLoad();
			}

			if (hasBeenUpdated != null) {
				// Notify navmesh cuts that they have been updated
				// This will cause RequiresUpdate to return false
				// until it is changed again.
				// Note: This is not as efficient as it could be when multiple graphs are used
				// because every navmesh cut will be added to the list once for every graph.
				for (int i = 0; i < hasBeenUpdated.Count; i++) {
					hasBeenUpdated[i].NotifyUpdated();
				}

				ListPool<NavmeshClipper>.Release (ref hasBeenUpdated);
			}
		}
#else
		internal class NavmeshUpdateSettings {
			public NavmeshUpdateSettings(NavmeshBase graph) {}
			public void OnRecalculatedTiles (NavmeshTile[] tiles) {}
		}
		internal void Update () {}
		internal void OnEnable () {}
		internal void OnDisable () {}
#endif
	}
}
