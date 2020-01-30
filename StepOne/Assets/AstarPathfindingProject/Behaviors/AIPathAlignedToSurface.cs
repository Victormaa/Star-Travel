using UnityEngine;
using System.Collections.Generic;

namespace Pathfinding {
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_a_i_path_aligned_to_surface.php")]
	public class AIPathAlignedToSurface : AIPath {
		protected override void Start () {
			base.Start();
			movementPlane.Set(rotation);
		}

		protected override void Update () {
			base.Update();
			UpdateMovementPlane();
		}

		protected override void ApplyGravity (float deltaTime) {
			// Apply gravity
			if (usingGravity) {
				// Gravity is relative to the current surface.
				// Only the normal direction is well defined however so x and z are ignored.
				verticalVelocity += float.IsNaN(gravity.x) ? Physics.gravity.y : gravity.y;
			} else {
				verticalVelocity = 0;
			}
		}

		Mesh cachedMesh;
		List<Vector3> cachedNormals = new List<Vector3>();
		List<int> cachedTriangles = new List<int>();

		Vector3 InterpolateNormal (RaycastHit hit) {
			MeshCollider meshCollider = hit.collider as MeshCollider;

			if (meshCollider == null || meshCollider.sharedMesh == null)
				return hit.normal;

			Mesh mesh = meshCollider.sharedMesh;

			// For performance, cache the triangles and normals from the last frame
			if (mesh != cachedMesh) {
				if (!mesh.isReadable) return hit.normal;
				cachedMesh = mesh;
				mesh.GetNormals(cachedNormals);
				mesh.GetTriangles(cachedTriangles, 0);
			}
			var normals = cachedNormals;
			var triangles = cachedTriangles;
			Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
			Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
			Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];
			Vector3 baryCenter = hit.barycentricCoordinate;
			Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
			interpolatedNormal = interpolatedNormal.normalized;
			Transform hitTransform = hit.collider.transform;
			interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);
			return interpolatedNormal;
		}


		/** Find the world position of the ground below the character */
		protected override void UpdateMovementPlane () {
			// Construct a new movement plane which has new normal
			// but is otherwise as similar to the previous plane as possible
			var normal = InterpolateNormal(lastRaycastHit);
			var fwd = Vector3.Cross(movementPlane.rotation * Vector3.right, normal);

			movementPlane.Set(Quaternion.LookRotation(fwd, normal));
			rvoController.SetMovementPlane(movementPlane);
		}
	}
}
