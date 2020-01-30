using UnityEngine;
using Pathfinding.RVO.Sampled;

namespace Pathfinding.RVO {
	/** Quadtree for quick nearest neighbour search of rvo agents.
	 * \see Pathfinding.RVO.Simulator
	 */
	public class RVOQuadtree {
		const int LeafSize = 15;

		float maxRadius = 0;
		public bool threeD;

		/** Node in a quadtree for storing RVO agents.
		 * \see Pathfinding.GraphNode for the node class that is used for pathfinding data.
		 */
		struct Node {
			public int child00;
			public Agent linkedList;
			public byte count;

			/** Maximum speed of all agents inside this node */
			public float maxSpeed;

			public void Add (Agent agent) {
				agent.next = linkedList;
				linkedList = agent;
			}

			/** Distribute the agents in this node among the children.
			 * Used after subdividing the node.
			 */
			public void Distribute (Node[] nodes, float xcenter, float ycenter, float zcenter) {
				while (linkedList != null) {
					Agent nx = linkedList.next;
					var index = child00 + (linkedList.position.x > xcenter ? 2 : 0) + (linkedList.position.y > ycenter ? 1 : 0) + (linkedList.position.z > zcenter ? 4 : 0);
					nodes[index].Add(linkedList);
					linkedList = nx;
				}
				count = 0;
			}

			public float CalculateMaxSpeed (Node[] nodes, int index) {
				if (child00 == index) {
					// Leaf node
					for (var agent = linkedList; agent != null; agent = agent.next) {
						maxSpeed = System.Math.Max(maxSpeed, agent.CalculatedSpeed);
					}
				} else {
					maxSpeed = System.Math.Max(nodes[child00].CalculateMaxSpeed(nodes, child00), nodes[child00+1].CalculateMaxSpeed(nodes, child00+1));
					maxSpeed = System.Math.Max(maxSpeed, nodes[child00+2].CalculateMaxSpeed(nodes, child00+2));
					maxSpeed = System.Math.Max(maxSpeed, nodes[child00+3].CalculateMaxSpeed(nodes, child00+3));
				}
				return maxSpeed;
			}
		}

		Node[] nodes = new Node[16];
		int filledNodes = 1;

		// Bounds
		float bxmin, bxmax, bymin, bymax, bzmin, bzmax;

		/** Removes all agents from the tree */
		public void Clear () {
			nodes[0] = new Node();
			filledNodes = 1;
			maxRadius = 0;
		}

		/** Set the bounding box of this quadtree.
		 * This must be done before adding any agents to the tree and all agents must be inside the bounding box.
		 */
		public void SetBounds (Vector3 min, Vector3 max) {
			bxmin = min.x;
			bymin = min.y;
			bzmin = min.z;
			bxmax = max.x;
			bymax = max.y;
			bzmax = max.z;
		}

		int GetNodeIndex () {
			int n = threeD ? 8 : 4;

			if (filledNodes + n >= nodes.Length) {
				var nds = new Node[nodes.Length*2];
				for (int i = 0; i < nodes.Length; i++) nds[i] = nodes[i];
				nodes = nds;
			}

			for (int i = 0; i < n; i++) {
				nodes[filledNodes] = new Node();
				nodes[filledNodes].child00 = filledNodes;
				filledNodes++;
			}

			return filledNodes-n;
		}

		/** Add a new agent to the tree.
		 * \warning Agents must not be added multiple times to the same tree
		 */
		public void Insert (Agent agent) {
			int i = 0;
			float xmin = bxmin, xmax = bxmax, ymin = bymin, ymax = bymax, zmin = bzmin, zmax = bzmax;

			if (!threeD) {
				zmin = float.PositiveInfinity;
				zmax = float.PositiveInfinity;
			}

			Vector3 p = agent.position;

			agent.next = null;

			maxRadius = System.Math.Max(agent.radius, maxRadius);

			int depth = 0;

			while (true) {
				depth++;

				if (nodes[i].child00 == i) {
					// Leaf node. Break at depth 10 in case lots of agents ( > LeafSize ) are in the same spot
					if (nodes[i].count < LeafSize || depth > 10) {
						nodes[i].Add(agent);
						nodes[i].count++;
						break;
					} else {
						// Split
						nodes[i].child00 = GetNodeIndex();
						nodes[i].Distribute(nodes, (xmin+xmax)*0.5f, (ymin+ymax)*0.5f, (zmin+zmax)*0.5f);
					}
				}
				// Note, no else. The node might have been changed from a leaf node to an inner node inside the IF statement above.
				if (nodes[i].child00 != i) {
					// Not a leaf node
					i = nodes[i].child00;

					if (threeD) {
						float cz = (zmin+zmax)*0.5f;
						if (p.z > cz) {
							i += 4;
							zmin = cz;
						} else {
							zmax = cz;
						}
					}

					float cx = (xmin+xmax)*0.5f;
					if (p.x > cx) {
						i += 2;
						xmin = cx;
					} else {
						xmax = cx;
					}

					float cy = (ymin+ymax)*0.5f;
					if (p.y > cy) {
						i += 1;
						ymin = cy;
					} else {
						ymax = cy;
					}
				}
			}
		}

		public void CalculateSpeeds () {
			nodes[0].CalculateMaxSpeed(nodes, 0);
		}

		/** Find all agents that could collide with an agent at position p within timeHorizon seconds.
		 * This method will call agent.InsertAgentNeighbour for each agent found.
		 */
		public void Query (Vector3 p, float speed, float timeHorizon, float agentRadius, Agent agent) {
			var query = new QuadtreeQuery {
				p = p, speed = speed, timeHorizon = timeHorizon, maxRadius = float.PositiveInfinity,
				agentRadius = agentRadius, agent = agent, nodes = nodes
			};

			if (threeD) query.QueryRec3D(0, bxmin, bxmax, bymin, bymax, bzmin, bzmax);
			else query.QueryRec(0, bxmin, bxmax, bymin, bymax);
		}

		struct QuadtreeQuery {
			public Vector3 p;
			public float speed, timeHorizon, agentRadius, maxRadius;
			public Agent agent;
			public Node[] nodes;

			public void QueryRec (int i, float xmin, float xmax, float ymin, float ymax) {
				// Determine the radius that we need to search to take all agents into account
				// Note: the second agentRadius usage should actually be the radius of the other agents, not this agent
				// but for performance reasons and for simplicity we assume that agents have approximately the same radius.
				// Thus an agent with a very small radius may in some cases detect an agent with a very large radius too late
				// however this effect should be minor.
				var radius = System.Math.Min(System.Math.Max((nodes[i].maxSpeed + speed)*timeHorizon, agentRadius) + agentRadius, maxRadius);
				var childOffset = nodes[i].child00;

				if (childOffset == i) {
					// Leaf node
					for (Agent a = nodes[i].linkedList; a != null; a = a.next) {
						float v = agent.InsertAgentNeighbour(a, radius*radius);
						// Limit the search if the agent has hit the max number of nearby agents threshold
						if (v < maxRadius*maxRadius) {
							maxRadius = Mathf.Sqrt(v);
						}
					}
				} else {
					// Not a leaf node
					float cx = (xmin + xmax)*0.5f;
					float cy = (ymin + ymax)*0.5f;

					if (p.x-radius < cx) {
						if (p.y-radius < cy) {
							QueryRec(childOffset, xmin, cx, ymin, cy);
							radius = System.Math.Min(radius, maxRadius);
						}
						if (p.y+radius > cy) {
							QueryRec(childOffset+1, xmin, cx, cy, ymax);
							radius = System.Math.Min(radius, maxRadius);
						}
					}

					if (p.x+radius > cx) {
						if (p.y-radius < cy) {
							QueryRec(childOffset+2, cx, xmax, ymin, cy);
							radius = System.Math.Min(radius, maxRadius);
						}
						if (p.y+radius > cy) {
							QueryRec(childOffset+3, cx, xmax, cy, ymax);
						}
					}
				}
			}

			public void QueryRec3D (int i, float xmin, float xmax, float ymin, float ymax, float zmin, float zmax) {
				// Determine the radius that we need to search to take all agents into account
				// Note: the second agentRadius usage should actually be the radius of the other agents, not this agent
				// but for performance reasons and for simplicity we assume that agents have approximately the same radius.
				// Thus an agent with a very small radius may in some cases detect an agent with a very large radius too late
				// however this effect should be minor.
				var radius = System.Math.Min(System.Math.Max((nodes[i].maxSpeed + speed)*timeHorizon, agentRadius) + agentRadius, maxRadius);
				var childOffset = nodes[i].child00;

				if (childOffset == i) {
					// Leaf node
					for (Agent a = nodes[i].linkedList; a != null; a = a.next) {
						float v = agent.InsertAgentNeighbour(a, radius*radius);
						// Limit the search if the agent has hit the max number of nearby agents threshold
						if (v < maxRadius*maxRadius) {
							maxRadius = Mathf.Sqrt(v);
						}
					}
				} else {
					// Not a leaf node
					float cx = (xmin + xmax)*0.5f;
					float cy = (ymin + ymax)*0.5f;
					float cz = (zmin + zmax)*0.5f;

					if (p.z-radius < cz) {
						if (p.x-radius < cx) {
							if (p.y-radius < cy) {
								QueryRec3D(childOffset, xmin, cx, ymin, cy, zmin, cz);
								radius = System.Math.Min(radius, maxRadius);
							}
							if (p.y+radius > cy) {
								QueryRec3D(childOffset+1, xmin, cx, cy, ymax, zmin, cz);
								radius = System.Math.Min(radius, maxRadius);
							}
						}

						if (p.x+radius > cx) {
							if (p.y-radius < cy) {
								QueryRec3D(childOffset+2, cx, xmax, ymin, cy, zmin, cz);
								radius = System.Math.Min(radius, maxRadius);
							}
							if (p.y+radius > cy) {
								QueryRec3D(childOffset+3, cx, xmax, cy, ymax, zmin, cz);
							}
						}
					}

					if (p.z+radius > cz) {
						if (p.x-radius < cx) {
							if (p.y-radius < cy) {
								QueryRec3D(childOffset+4, xmin, cx, ymin, cy, cz, zmax);
								radius = System.Math.Min(radius, maxRadius);
							}
							if (p.y+radius > cy) {
								QueryRec3D(childOffset+5, xmin, cx, cy, ymax, cz, zmax);
								radius = System.Math.Min(radius, maxRadius);
							}
						}

						if (p.x+radius > cx) {
							if (p.y-radius < cy) {
								QueryRec3D(childOffset+6, cx, xmax, ymin, cy, cz, zmax);
								radius = System.Math.Min(radius, maxRadius);
							}
							if (p.y+radius > cy) {
								QueryRec3D(childOffset+7, cx, xmax, cy, ymax, cz, zmax);
							}
						}
					}
				}
			}
		}

		public void DebugDraw () {
			DebugDrawRec(0, bxmin, bxmax, bymin, bymax, bzmin, bzmax);
		}

		void DebugDrawRec (int i, float xmin, float xmax, float ymin, float ymax, float zmin, float zmax) {
			Debug.DrawLine(new Vector3(xmin, ymin, zmin), new Vector3(xmax, ymin, zmin), Color.white);
			Debug.DrawLine(new Vector3(xmax, ymin, zmin), new Vector3(xmax, ymax, zmin), Color.white);
			Debug.DrawLine(new Vector3(xmax, ymax, zmin), new Vector3(xmin, ymax, zmin), Color.white);
			Debug.DrawLine(new Vector3(xmin, ymax, zmin), new Vector3(xmin, ymin, zmin), Color.white);
			if (threeD) {
				Debug.DrawLine(new Vector3(xmin, ymin, zmax), new Vector3(xmax, ymin, zmax), Color.white);
				Debug.DrawLine(new Vector3(xmax, ymin, zmax), new Vector3(xmax, ymax, zmax), Color.white);
				Debug.DrawLine(new Vector3(xmax, ymax, zmax), new Vector3(xmin, ymax, zmax), Color.white);
				Debug.DrawLine(new Vector3(xmin, ymax, zmax), new Vector3(xmin, ymin, zmax), Color.white);
				Debug.DrawLine(new Vector3(xmin, ymin, zmin), new Vector3(xmin, ymin, zmax), Color.white);
				Debug.DrawLine(new Vector3(xmax, ymin, zmin), new Vector3(xmax, ymin, zmax), Color.white);
				Debug.DrawLine(new Vector3(xmax, ymax, zmin), new Vector3(xmax, ymax, zmax), Color.white);
				Debug.DrawLine(new Vector3(xmin, ymax, zmin), new Vector3(xmin, ymax, zmax), Color.white);
			}

			float cx = (xmin + xmax)*0.5f;
			float cy = (ymin + ymax)*0.5f;
			float cz = (zmin + zmax)*0.5f;
			if (nodes[i].child00 != i) {
				// Not a leaf node
				DebugDrawRec(nodes[i].child00+3, cx, xmax, cy, ymax, zmin, cz);
				DebugDrawRec(nodes[i].child00+2, cx, xmax, ymin, cy, zmin, cz);
				DebugDrawRec(nodes[i].child00+1, xmin, cx, cy, ymax, zmin, cz);
				DebugDrawRec(nodes[i].child00+0, xmin, cx, ymin, cy, zmin, cz);
				if (threeD) {
					DebugDrawRec(nodes[i].child00+7, cx, xmax, cy, ymax, cz, zmax);
					DebugDrawRec(nodes[i].child00+6, cx, xmax, ymin, cy, cz, zmax);
					DebugDrawRec(nodes[i].child00+5, xmin, cx, cy, ymax, cz, zmax);
					DebugDrawRec(nodes[i].child00+4, xmin, cx, ymin, cy, cz, zmax);
				}
			}

			for (Agent a = nodes[i].linkedList; a != null; a = a.next) {
				Debug.DrawLine(new Vector3(cx, cy, cz), a.position+Vector3.up, new Color(1, 1, 0, 0.5f));
			}
		}
	}
}
