using UnityEngine;
using System.Collections.Generic;
using Pathfinding.Serialization;

namespace Pathfinding {
	/** Basic point graph.
	 * \ingroup graphs
	 * The point graph is the most basic graph structure, it consists of a number of interconnected points in space called nodes or waypoints.\n
	 * The point graph takes a Transform object as "root", this Transform will be searched for child objects, every child object will be treated as a node.
	 * If #recursive is enabled, it will also search the child objects of the children recursively.
	 * It will then check if any connections between the nodes can be made, first it will check if the distance between the nodes isn't too large (#maxDistance)
	 * and then it will check if the axis aligned distance isn't too high. The axis aligned distance, named #limits,
	 * is useful because usually an AI cannot climb very high, but linking nodes far away from each other,
	 * but on the same Y level should still be possible. #limits and #maxDistance are treated as being set to infinity if they are set to 0 (zero). \n
	 * Lastly it will check if there are any obstructions between the nodes using
	 * <a href="http://unity3d.com/support/documentation/ScriptReference/Physics.Raycast.html">raycasting</a> which can optionally be thick.\n
	 * One thing to think about when using raycasting is to either place the nodes a small
	 * distance above the ground in your scene or to make sure that the ground is not in the raycast \a mask to avoid the raycast from hitting the ground.\n
	 *
	 * Alternatively, a tag can be used to search for nodes.
	 * \see http://docs.unity3d.com/Manual/Tags.html
	 *
	 * For larger graphs, it can take quite some time to scan the graph with the default settings.
	 * If you have the pro version you can enable 'optimizeForSparseGraph' which will in most cases reduce the calculation times
	 * drastically.
	 *
	 * \note Does not support linecast because of obvious reasons.
	 *
	 * \shadowimage{pointgraph_graph.png}
	 * \shadowimage{pointgraph_inspector.png}
	 *
	 */
	[JsonOptIn]
	public class PointGraph : NavGraph
		, IUpdatableGraph {
		/** Childs of this transform are treated as nodes */
		[JsonMember]
		public Transform root;

		/** If no #root is set, all nodes with the tag is used as nodes */
		[JsonMember]
		public string searchTag;

		/** Max distance for a connection to be valid.
		 * The value 0 (zero) will be read as infinity and thus all nodes not restricted by
		 * other constraints will be added as connections.
		 *
		 * A negative value will disable any neighbours to be added.
		 * It will completely stop the connection processing to be done, so it can save you processing
		 * power if you don't these connections.
		 */
		[JsonMember]
		public float maxDistance;

		/** Max distance along the axis for a connection to be valid. 0 = infinity */
		[JsonMember]
		public Vector3 limits;

		/** Use raycasts to check connections */
		[JsonMember]
		public bool raycast = true;

		/** Use the 2D Physics API */
		[JsonMember]
		public bool use2DPhysics;

		/** Use thick raycast */
		[JsonMember]
		public bool thickRaycast;

		/** Thick raycast radius */
		[JsonMember]
		public float thickRaycastRadius = 1;

		/** Recursively search for child nodes to the #root */
		[JsonMember]
		public bool recursive = true;

		/** Layer mask to use for raycast */
		[JsonMember]
		public LayerMask mask;

		/** Optimizes the graph for sparse graphs.
		 *
		 * This can reduce calculation times for both scanning and for normal path requests by huge amounts.
		 * It reduces the number of node-node checks that need to be done during scan, and can also optimize getting the nearest node from the graph (such as when querying for a path).
		 *
		 * Try enabling and disabling this option, check the scan times logged when you scan the graph to see if your graph is suited for this optimization
		 * or if it makes it slower.
		 *
		 * The gain of using this optimization increases with larger graphs, the default scan algorithm is brute force and requires O(n^2) checks, this optimization
		 * along with a graph suited for it, requires only O(n) checks during scan (assuming the connection distance limits are reasonable).
		 *
		 * \warning
		 * When you have this enabled, you will not be able to move nodes around using scripting unless you recalculate the lookup structure at the same time.
		 * \see #RebuildNodeLookup
		 *
		 * If you enable this during runtime, you will need to call #RebuildNodeLookup to make sure any existing nodes are added to the lookup structure.
		 * If the graph doesn't have any nodes yet or if you are going to scan the graph afterwards then you do not need to do this.
		 *
		 * \astarpro
		 */
		[JsonMember]
		public bool optimizeForSparseGraph;

		PointKDTree lookupTree = new PointKDTree();

		/** All nodes in this graph.
		 * Note that only the first #nodeCount will be non-null.
		 *
		 * You can also use the GetNodes method to get all nodes.
		 */
		public PointNode[] nodes;

		/** Number of nodes in this graph */
		public int nodeCount { get; protected set; }

		public override int CountNodes () {
			return nodeCount;
		}

		public override void GetNodes (System.Action<GraphNode> action) {
			if (nodes == null) return;
			var count = nodeCount;
			for (int i = 0; i < count; i++) action(nodes[i]);
		}

		public override NNInfoInternal GetNearest (Vector3 position, NNConstraint constraint, GraphNode hint) {
			return GetNearestInternal(position, constraint, true);
		}

		public override NNInfoInternal GetNearestForce (Vector3 position, NNConstraint constraint) {
			return GetNearestInternal(position, constraint, false);
		}

		NNInfoInternal GetNearestInternal (Vector3 position, NNConstraint constraint, bool fastCheck) {
			if (nodes == null) return new NNInfoInternal();
			var iposition = (Int3)position;

			if (optimizeForSparseGraph) {
				return new NNInfoInternal(lookupTree.GetNearest(iposition, fastCheck ? null : constraint));
			}

			float maxDistSqr = constraint == null || constraint.constrainDistance ? AstarPath.active.maxNearestNodeDistanceSqr : float.PositiveInfinity;

			var nnInfo = new NNInfoInternal(null);
			long minDist = long.MaxValue;
			long minConstDist = long.MaxValue;

			for (int i = 0; i < nodeCount; i++) {
				PointNode node = nodes[i];
				long dist = (iposition - node.position).sqrMagnitudeLong;

				if (dist < minDist) {
					minDist = dist;
					nnInfo.node = node;
				}

				if (dist < minConstDist && dist < maxDistSqr && (constraint == null || constraint.Suitable(node))) {
					minConstDist = dist;
					nnInfo.constrainedNode = node;
				}
			}

			if (!fastCheck) nnInfo.node = nnInfo.constrainedNode;

			nnInfo.UpdateInfo();
			return nnInfo;
		}

		/** Add a node to the graph at the specified position.
		 * \note Vector3 can be casted to Int3 using (Int3)myVector.
		 *
		 * \note This needs to be called when it is safe to update nodes, which is
		 * - when scanning
		 * - during a graph update
		 * - inside a callback registered using AstarPath.AddWorkItem
		 *
		 * \snippet MiscSnippets.cs PointGraph.AddNode
		 */
		public PointNode AddNode (Int3 position) {
			return AddNode(new PointNode(active), position);
		}

		/** Add a node with the specified type to the graph at the specified position.
		 *
		 * \param node This must be a node created using T(AstarPath.active) right before the call to this method.
		 * The node parameter is only there because there is no new(AstarPath) constraint on
		 * generic type parameters.
		 * \param position The node will be set to this position.
		 * \note Vector3 can be casted to Int3 using (Int3)myVector.
		 *
		 * \note This needs to be called when it is safe to update nodes, which is
		 * - when scanning
		 * - during a graph update
		 * - inside a callback registered using AstarPath.AddWorkItem
		 *
		 * \see AstarPath.AddWorkItem
		 */
		public T AddNode<T>(T node, Int3 position) where T : PointNode {
			if (nodes == null || nodeCount == nodes.Length) {
				var newNodes = new PointNode[nodes != null ? System.Math.Max(nodes.Length+4, nodes.Length*2) : 4];
				if (nodes != null) nodes.CopyTo(newNodes, 0);
				nodes = newNodes;
			}

			node.SetPosition(position);
			node.GraphIndex = graphIndex;
			node.Walkable = true;

			nodes[nodeCount] = node;
			nodeCount++;

			if (optimizeForSparseGraph) AddToLookup(node);

			return node;
		}

		/** Recursively counds children of a transform */
		protected static int CountChildren (Transform tr) {
			int c = 0;

			foreach (Transform child in tr) {
				c++;
				c += CountChildren(child);
			}
			return c;
		}

		/** Recursively adds childrens of a transform as nodes */
		protected void AddChildren (ref int c, Transform tr) {
			foreach (Transform child in tr) {
				nodes[c].position = (Int3)child.position;
				nodes[c].Walkable = true;
				nodes[c].gameObject = child.gameObject;

				c++;
				AddChildren(ref c, child);
			}
		}

		/** Rebuilds the lookup structure for nodes.
		 *
		 * This is used when #optimizeForSparseGraph is enabled.
		 *
		 * You should call this method every time you move a node in the graph manually and
		 * you are using #optimizeForSparseGraph, otherwise pathfinding might not work correctly.
		 *
		 * You may also call this after you have added many nodes using the
		 * #AddNode method. When adding nodes using the #AddNode method they
		 * will be added to the lookup structure. The lookup structure will
		 * rebalance itself when it gets too unbalanced however if you are
		 * sure you won't be adding any more nodes in the short term, you can
		 * make sure it is perfectly balanced and thus squeeze out the last
		 * bit of performance by calling this method. This can improve the
		 * performance of the #GetNearest method slightly. The improvements
		 * are on the order of 10-20%.
		 *
		 * \astarpro
		 */
		public void RebuildNodeLookup () {
			if (!optimizeForSparseGraph || nodes == null) {
				lookupTree = new PointKDTree();
			} else {
				lookupTree.Rebuild(nodes, 0, nodeCount);
			}
		}

		void AddToLookup (PointNode node) {
			lookupTree.Add(node);
		}

		protected virtual PointNode[] CreateNodes (int count) {
			var nodes = new PointNode[count];

			for (int i = 0; i < nodeCount; i++) nodes[i] = new PointNode(active);
			return nodes;
		}

		protected override IEnumerable<Progress> ScanInternal () {
			yield return new Progress(0, "Searching for GameObjects");

			if (root == null) {
				// If there is no root object, try to find nodes with the specified tag instead
				GameObject[] gos = searchTag != null? GameObject.FindGameObjectsWithTag (searchTag) : null;

				if (gos == null) {
					nodes = new PointNode[0];
					nodeCount = 0;
				} else {
					yield return new Progress(0.1f, "Creating nodes");

					// Create all the nodes
					nodeCount = gos.Length;
					nodes = CreateNodes(nodeCount);

					for (int i = 0; i < gos.Length; i++) {
						nodes[i].position = (Int3)gos[i].transform.position;
						nodes[i].Walkable = true;
						nodes[i].gameObject = gos[i].gameObject;
					}
				}
			} else {
				// Search the root for children and create nodes for them
				if (!recursive) {
					nodeCount = root.childCount;
					nodes = CreateNodes(nodeCount);

					int c = 0;
					foreach (Transform child in root) {
						nodes[c].position = (Int3)child.position;
						nodes[c].Walkable = true;
						nodes[c].gameObject = child.gameObject;

						c++;
					}
				} else {
					nodeCount = CountChildren(root);
					nodes = CreateNodes(nodeCount);

					int startID = 0;
					AddChildren(ref startID, root);
				}
			}

			yield return new Progress(0.15f, "Building node lookup");
			// Note that this *must* run every scan
			RebuildNodeLookup();

			foreach (var progress in ConnectNodesAsync()) yield return progress.MapTo(0.16f, 1.0f);
		}

		/** Recalculates connections for all nodes in the graph.
		 * This is useful if you have created nodes manually using #AddNode and then want to connect them in the same way as the point graph normally connects nodes.
		 */
		public void ConnectNodes () {
			foreach (var progress in ConnectNodesAsync()) {}
		}

		/** Calculates connections for all nodes in the graph.
		 * This is an IEnumerable, you can iterate through it using e.g foreach to get progress information.
		 */
		IEnumerable<Progress> ConnectNodesAsync () {
			if (maxDistance >= 0) {
				// To avoid too many allocations, these lists are reused for each node
				var connections = new List<Connection>();
				var candidateConnections = new List<GraphNode>();

				long maxSquaredRange;
				// Max possible squared length of a connection between two nodes
				// This is used to speed up the calculations by skipping a lot of nodes that do not need to be checked
				if (maxDistance == 0 && (limits.x == 0 || limits.y == 0 || limits.z == 0)) {
					maxSquaredRange = long.MaxValue;
				} else {
					maxSquaredRange = (long)(Mathf.Max(limits.x, Mathf.Max(limits.y, Mathf.Max(limits.z, maxDistance))) * Int3.Precision) + 1;
					maxSquaredRange *= maxSquaredRange;
				}

				// Report progress every N nodes
				const int YieldEveryNNodes = 512;

				// Loop through all nodes and add connections to other nodes
				for (int i = 0; i < nodeCount; i++) {
					if (i % YieldEveryNNodes == 0) {
						yield return new Progress(i/(float)nodes.Length, "Connecting nodes");
					}

					connections.Clear();
					var node = nodes[i];
					if (optimizeForSparseGraph) {
						candidateConnections.Clear();
						lookupTree.GetInRange(node.position, maxSquaredRange, candidateConnections);
						for (int j = 0; j < candidateConnections.Count; j++) {
							var other = candidateConnections[j] as PointNode;
							float dist;
							if (other != node && IsValidConnection(node, other, out dist)) {
								connections.Add(new Connection(
									other,
									/** \todo Is this equal to .costMagnitude */
									(uint)Mathf.RoundToInt(dist*Int3.FloatPrecision)
									));
							}
						}
					} else {
						// Only brute force is available in the free version
						for (int j = 0; j < nodeCount; j++) {
							if (i == j) continue;

							PointNode other = nodes[j];
							float dist;
							if (IsValidConnection(node, other, out dist)) {
								connections.Add(new Connection(
									other,
									/** \todo Is this equal to .costMagnitude */
									(uint)Mathf.RoundToInt(dist*Int3.FloatPrecision)
									));
							}
						}
					}
					node.connections = connections.ToArray();
				}
			}
		}

		/** Returns if the connection between \a a and \a b is valid.
		 * Checks for obstructions using raycasts (if enabled) and checks for height differences.\n
		 * As a bonus, it outputs the distance between the nodes too if the connection is valid.
		 *
		 * \note This is not the same as checking if node a is connected to node b.
		 * That should be done using a.ContainsConnection(b)
		 */
		public virtual bool IsValidConnection (GraphNode a, GraphNode b, out float dist) {
			dist = 0;

			if (!a.Walkable || !b.Walkable) return false;

			var dir = (Vector3)(b.position-a.position);

			if (
				(!Mathf.Approximately(limits.x, 0) && Mathf.Abs(dir.x) > limits.x) ||
				(!Mathf.Approximately(limits.y, 0) && Mathf.Abs(dir.y) > limits.y) ||
				(!Mathf.Approximately(limits.z, 0) && Mathf.Abs(dir.z) > limits.z)) {
				return false;
			}

			dist = dir.magnitude;
			if (maxDistance == 0 || dist < maxDistance) {
				if (raycast) {
					var ray = new Ray((Vector3)a.position, dir);
					var invertRay = new Ray((Vector3)b.position, -dir);

					if (use2DPhysics) {
						if (thickRaycast) {
							return !Physics2D.CircleCast(ray.origin, thickRaycastRadius, ray.direction, dist, mask) && !Physics2D.CircleCast(invertRay.origin, thickRaycastRadius, invertRay.direction, dist, mask);
						} else {
							return !Physics2D.Linecast((Vector2)(Vector3)a.position, (Vector2)(Vector3)b.position, mask) && !Physics2D.Linecast((Vector2)(Vector3)b.position, (Vector2)(Vector3)a.position, mask);
						}
					} else {
						if (thickRaycast) {
							return !Physics.SphereCast(ray, thickRaycastRadius, dist, mask) && !Physics.SphereCast(invertRay, thickRaycastRadius, dist, mask);
						} else {
							return !Physics.Linecast((Vector3)a.position, (Vector3)b.position, mask) && !Physics.Linecast((Vector3)b.position, (Vector3)a.position, mask);
						}
					}
				} else {
					return true;
				}
			}
			return false;
		}

		GraphUpdateThreading IUpdatableGraph.CanUpdateAsync (GraphUpdateObject o) {
			return GraphUpdateThreading.UnityThread;
		}

		void IUpdatableGraph.UpdateAreaInit (GraphUpdateObject o) {}
		void IUpdatableGraph.UpdateAreaPost (GraphUpdateObject o) {}

		/** Updates an area in the list graph.
		 * Recalculates possibly affected connections, i.e all connectionlines passing trough the bounds of the \a guo will be recalculated
		 * \astarpro */
		void IUpdatableGraph.UpdateArea (GraphUpdateObject guo) {
			if (nodes == null) return;

			for (int i = 0; i < nodeCount; i++) {
				var node = nodes[i];
				if (guo.bounds.Contains((Vector3)node.position)) {
					guo.WillUpdateNode(node);
					guo.Apply(node);
				}
			}

			if (guo.updatePhysics) {
				// Use a copy of the bounding box, we should not change the GUO's bounding box since it might be used for other graph updates
				Bounds bounds = guo.bounds;

				if (thickRaycast) {
					// Expand the bounding box to account for the thick raycast
					bounds.Expand(thickRaycastRadius*2);
				}

				// Create a temporary list used for holding connection data
				List<Connection> tmpList = Pathfinding.Util.ListPool<Connection>.Claim ();

				for (int i = 0; i < nodeCount; i++) {
					PointNode node = nodes[i];
					var nodePos = (Vector3)node.position;

					List<Connection> conn = null;

					for (int j = 0; j < nodeCount; j++) {
						if (j == i) continue;

						var otherNodePos = (Vector3)nodes[j].position;
						// Check if this connection intersects the bounding box.
						// If it does we need to recalculate that connection.
						if (VectorMath.SegmentIntersectsBounds(bounds, nodePos, otherNodePos)) {
							float dist;
							PointNode other = nodes[j];
							bool contains = node.ContainsConnection(other);
							bool validConnection = IsValidConnection(node, other, out dist);

							// Fill the 'conn' list when we need to change a connection
							if (conn == null && (contains != validConnection)) {
								tmpList.Clear();
								conn = tmpList;
								conn.AddRange(node.connections);
							}

							if (!contains && validConnection) {
								// A new connection should be added
								uint cost = (uint)Mathf.RoundToInt(dist*Int3.FloatPrecision);
								conn.Add(new Connection(other, cost));
							} else if (contains && !validConnection) {
								// A connection should be removed
								for (int q = 0; q < conn.Count; q++) {
									if (conn[q].node == other) {
										conn.RemoveAt(q);
										break;
									}
								}
							}
						}
					}

					// Save the new connections if any were changed
					if (conn != null) {
						node.connections = conn.ToArray();
					}
				}

				// Release buffers back to the pool
				Pathfinding.Util.ListPool<Connection>.Release (ref tmpList);
			}
		}

#if UNITY_EDITOR
		public override void OnDrawGizmos (Pathfinding.Util.RetainedGizmos gizmos, bool drawNodes) {
			base.OnDrawGizmos(gizmos, drawNodes);

			if (!drawNodes) return;

			Gizmos.color = new Color(0.161f, 0.341f, 1f, 0.5f);

			if (root != null) {
				DrawChildren(this, root);
			} else if (!string.IsNullOrEmpty(searchTag)) {
				GameObject[] gos = GameObject.FindGameObjectsWithTag(searchTag);
				for (int i = 0; i < gos.Length; i++) {
					Gizmos.DrawCube(gos[i].transform.position, Vector3.one*UnityEditor.HandleUtility.GetHandleSize(gos[i].transform.position)*0.1F);
				}
			}
		}

		static void DrawChildren (PointGraph graph, Transform tr) {
			foreach (Transform child in tr) {
				Gizmos.DrawCube(child.position, Vector3.one*UnityEditor.HandleUtility.GetHandleSize(child.position)*0.1F);
				if (graph.recursive) DrawChildren(graph, child);
			}
		}
#endif

		protected override void PostDeserialization (GraphSerializationContext ctx) {
			RebuildNodeLookup();
		}

		public override void RelocateNodes (Matrix4x4 deltaMatrix) {
			base.RelocateNodes(deltaMatrix);
			RebuildNodeLookup();
		}

		protected override void DeserializeSettingsCompatibility (GraphSerializationContext ctx) {
			base.DeserializeSettingsCompatibility(ctx);

			root = ctx.DeserializeUnityObject() as Transform;
			searchTag = ctx.reader.ReadString();
			maxDistance = ctx.reader.ReadSingle();
			limits = ctx.DeserializeVector3();
			raycast = ctx.reader.ReadBoolean();
			use2DPhysics = ctx.reader.ReadBoolean();
			thickRaycast = ctx.reader.ReadBoolean();
			thickRaycastRadius = ctx.reader.ReadSingle();
			recursive = ctx.reader.ReadBoolean();
			ctx.reader.ReadBoolean(); // Deprecated field
			mask = (LayerMask)ctx.reader.ReadInt32();
			optimizeForSparseGraph = ctx.reader.ReadBoolean();
			ctx.reader.ReadBoolean(); // Deprecated field
		}

		protected override void SerializeExtraInfo (GraphSerializationContext ctx) {
			// Serialize node data

			if (nodes == null) ctx.writer.Write(-1);

			// Length prefixed array of nodes
			ctx.writer.Write(nodeCount);
			for (int i = 0; i < nodeCount; i++) {
				// -1 indicates a null field
				if (nodes[i] == null) ctx.writer.Write(-1);
				else {
					ctx.writer.Write(0);
					nodes[i].SerializeNode(ctx);
				}
			}
		}

		protected override void DeserializeExtraInfo (GraphSerializationContext ctx) {
			int count = ctx.reader.ReadInt32();

			if (count == -1) {
				nodes = null;
				return;
			}

			nodes = new PointNode[count];
			nodeCount = count;

			for (int i = 0; i < nodes.Length; i++) {
				if (ctx.reader.ReadInt32() == -1) continue;
				nodes[i] = new PointNode(active);
				nodes[i].DeserializeNode(ctx);
			}
		}
	}
}
