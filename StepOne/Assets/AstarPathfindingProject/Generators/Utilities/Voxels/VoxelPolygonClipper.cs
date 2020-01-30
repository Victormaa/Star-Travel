namespace Pathfinding.Voxels {
	/** Utility for clipping polygons */
	internal struct VoxelPolygonClipper {
		public float[] x;
		public float[] y;
		public float[] z;
		public int n;

		public VoxelPolygonClipper (int capacity) {
			x = new float[capacity];
			y = new float[capacity];
			z = new float[capacity];
			n = 0;
		}

		public UnityEngine.Vector3 this[int i] {
			set {
				x[i] = value.x;
				y[i] = value.y;
				z[i] = value.z;
			}
		}

		/** Clips a polygon against an axis aligned half plane.
		 * The polygons stored in this object are clipped against the half plane at x = -offset.
		 *
		 * \param result Ouput vertices
		 * \param multi Scale factor for the input vertices. Should be +1 or -1. If -1 the negative half plane is kept.
		 * \param offset Offset to move the input vertices with before cutting
		 *
		 */
		public void ClipPolygonAlongX (ref VoxelPolygonClipper result, float multi, float offset) {
			// Number of resulting vertices
			int m = 0;
			bool prev, curr;

			float dj = multi*x[(n-1)]+offset;

			for (int i = 0, j = n-1; i < n; j = i, i++) {
				float di = multi*x[i]+offset;
				prev = dj >= 0;
				curr = di >= 0;

				if (prev != curr) {
					float s = dj / (dj - di);
					result.x[m] = x[j] + (x[i]-x[j])*s;
					result.y[m] = y[j] + (y[i]-y[j])*s;
					result.z[m] = z[j] + (z[i]-z[j])*s;
					m++;
				}

				if (curr) {
					result.x[m] = x[i];
					result.y[m] = y[i];
					result.z[m] = z[i];
					m++;
				}

				dj = di;
			}

			result.n = m;
		}

		/** Clips a polygon against an axis aligned half plane.
		 * The polygons stored in this object are clipped against the half plane at z = -offset.
		 *
		 * \param result Ouput vertices. Only the Y and Z coordinates are calculated. The X coordinates are undefined.
		 * \param multi Scale factor for the input vertices. Should be +1 or -1. If -1 the negative half plane is kept.
		 * \param offset Offset to move the input vertices with before cutting
		 *
		 */
		public void ClipPolygonAlongZWithYZ (ref VoxelPolygonClipper result, float multi, float offset) {
			// Number of resulting vertices
			int m = 0;
			bool prev, curr;

			float dj = multi*z[(n-1)]+offset;

			for (int i = 0, j = n-1; i < n; j = i, i++) {
				float di = multi*z[i]+offset;
				prev = dj >= 0;
				curr = di >= 0;

				if (prev != curr) {
					float s = dj / (dj - di);
					result.y[m] = y[j] + (y[i]-y[j])*s;
					result.z[m] = z[j] + (z[i]-z[j])*s;
					m++;
				}

				if (curr) {
					result.y[m] = y[i];
					result.z[m] = z[i];
					m++;
				}

				dj = di;
			}

			result.n = m;
		}

		/** Clips a polygon against an axis aligned half plane.
		 * The polygons stored in this object are clipped against the half plane at z = -offset.
		 *
		 * \param result Ouput vertices. Only the Y coordinates are calculated. The X and Z coordinates are undefined.
		 * \param multi Scale factor for the input vertices. Should be +1 or -1. If -1 the negative half plane is kept.
		 * \param offset Offset to move the input vertices with before cutting
		 *
		 */
		public void ClipPolygonAlongZWithY (ref VoxelPolygonClipper result, float multi, float offset) {
			// Number of resulting vertices
			int m = 0;
			bool prev, curr;

			float dj = multi*z[(n-1)]+offset;

			for (int i = 0, j = n-1; i < n; j = i, i++) {
				float di = multi*z[i]+offset;
				prev = dj >= 0;
				curr = di >= 0;

				if (prev != curr) {
					float s = dj / (dj - di);
					result.y[m] = y[j] + (y[i]-y[j])*s;
					m++;
				}

				if (curr) {
					result.y[m] = y[i];
					m++;
				}

				dj = di;
			}

			result.n = m;
		}
	}

	/** Utility for clipping polygons */
	internal struct Int3PolygonClipper {
		/** Cache this buffer to avoid unnecessary allocations */
		float[] clipPolygonCache;

		/** Cache this buffer to avoid unnecessary allocations */
		int[] clipPolygonIntCache;

		/** Initialize buffers if they are null */
		public void Init () {
			if (clipPolygonCache == null) {
				clipPolygonCache = new float[7*3];
				clipPolygonIntCache = new int[7*3];
			}
		}

		/** Clips a polygon against an axis aligned half plane.
		 * \param vIn Input vertices
		 * \param n Number of input vertices (may be less than the length of the vIn array)
		 * \param vOut Output vertices, needs to be large enough
		 * \param multi Scale factor for the input vertices
		 * \param offset Offset to move the input vertices with before cutting
		 * \param axis Axis to cut along, either x=0, y=1, z=2
		 *
		 * \returns Number of output vertices
		 *
		 * The vertices will be scaled and then offset, after that they will be cut using either the
		 * x axis, y axis or the z axis as the cutting line. The resulting vertices will be added to the
		 * vOut array in their original space (i.e before scaling and offsetting).
		 */
		public int ClipPolygon (Int3[] vIn, int n, Int3[] vOut, int multi, int offset, int axis) {
			Init();
			int[] d = clipPolygonIntCache;

			for (int i = 0; i < n; i++) {
				d[i] = multi*vIn[i][axis]+offset;
			}

			// Number of resulting vertices
			int m = 0;

			for (int i = 0, j = n-1; i < n; j = i, i++) {
				bool prev = d[j] >= 0;
				bool curr = d[i] >= 0;

				if (prev != curr) {
					double s = (double)d[j] / (d[j] - d[i]);

					vOut[m] = vIn[j] + (vIn[i]-vIn[j])*s;
					m++;
				}

				if (curr) {
					vOut[m] = vIn[i];
					m++;
				}
			}

			return m;
		}
	}
}
