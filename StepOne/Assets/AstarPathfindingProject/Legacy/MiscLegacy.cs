using UnityEngine;
using Pathfinding.Util;

namespace Pathfinding {
	// Obsolete methods in AIPath
	public partial class AIPath {
		/** True if the end of the path has been reached.
		 * \deprecated When unifying the interfaces for different movement scripts, this property has been renamed to #reachedEndOfPath
		  */
		[System.Obsolete("When unifying the interfaces for different movement scripts, this property has been renamed to reachedEndOfPath.  [AstarUpgradable: 'TargetReached' -> 'reachedEndOfPath']")]
		public bool TargetReached { get { return reachedEndOfPath; } }

		/** Rotation speed.
		 * \deprecated This field has been renamed to #rotationSpeed and is now in degrees per second instead of a damping factor.
		 */
		[System.Obsolete("This field has been renamed to #rotationSpeed and is now in degrees per second instead of a damping factor")]
		public float turningSpeed { get { return rotationSpeed/90; } set { rotationSpeed = value*90; } }

		/** Maximum speed in world units per second.
		 * \deprecated Use #maxSpeed instead
		 */
		[System.Obsolete("This member has been deprecated. Use 'maxSpeed' instead. [AstarUpgradable: 'speed' -> 'maxSpeed']")]
		public float speed { get { return maxSpeed; } set { maxSpeed = value; } }

		/** Direction that the agent wants to move in (excluding physics and local avoidance).
		 * \deprecated Only exists for compatibility reasons. Use #desiredVelocity or #steeringTarget instead instead.
		 */
		[System.Obsolete("Only exists for compatibility reasons. Use desiredVelocity or steeringTarget instead.")]
		public Vector3 targetDirection {
			get {
				return (steeringTarget - tr.position).normalized;
			}
		}

		/** Current desired velocity of the agent (excluding physics and local avoidance but it includes gravity).
		 * \deprecated This method no longer calculates the velocity. Use the #desiredVelocity property instead.
		 */
		[System.Obsolete("This method no longer calculates the velocity. Use the desiredVelocity property instead")]
		public Vector3 CalculateVelocity (Vector3 position) {
			return desiredVelocity;
		}
	}

	// Obsolete methods in RichAI
	public partial class RichAI {
		/** Force recalculation of the current path.
		 * If there is an ongoing path calculation, it will be canceled (so make sure you leave time for the paths to get calculated before calling this function again).
		 *
		 * \deprecated Use #SearchPath instead
		 */
		[System.Obsolete("Use SearchPath instead. [AstarUpgradable: 'UpdatePath' -> 'SearchPath']")]
		public void UpdatePath () {
			SearchPath();
		}

		/** Current velocity of the agent.
		 * Includes velocity due to gravity.
		 * \deprecated Use #velocity instead (lowercase 'v').
		 */
		[System.Obsolete("Use velocity instead (lowercase 'v'). [AstarUpgradable: 'Velocity' -> 'velocity']")]
		public Vector3 Velocity { get { return velocity; } }

		/** Waypoint that the agent is moving towards.
		 * This is either a corner in the path or the end of the path.
		 * \deprecated Use steeringTarget instead.
		 */
		[System.Obsolete("Use steeringTarget instead. [AstarUpgradable: 'NextWaypoint' -> 'steeringTarget']")]
		public Vector3 NextWaypoint { get { return steeringTarget; } }

		/** \details
		 * \deprecated Use Vector3.Distance(transform.position, ai.steeringTarget) instead.
		 */
		[System.Obsolete("Use Vector3.Distance(transform.position, ai.steeringTarget) instead.")]
		public float DistanceToNextWaypoint { get { return distanceToSteeringTarget; } }

		/** Search for new paths repeatedly */
		[System.Obsolete("Use canSearch instead. [AstarUpgradable: 'repeatedlySearchPaths' -> 'canSearch']")]
		public bool repeatedlySearchPaths { get { return canSearch; } set { canSearch = value; } }

		/** True if the end of the path has been reached.
		 * \deprecated When unifying the interfaces for different movement scripts, this property has been renamed to #reachedEndOfPath
		  */
		[System.Obsolete("When unifying the interfaces for different movement scripts, this property has been renamed to reachedEndOfPath (lowercase t).  [AstarUpgradable: 'TargetReached' -> 'reachedEndOfPath']")]
		public bool TargetReached { get { return reachedEndOfPath; } }

		/** True if a path to the target is currently being calculated.
		 * \deprecated Use #pathPending instead (lowercase 'p').
		  */
		[System.Obsolete("Use pathPending instead (lowercase 'p'). [AstarUpgradable: 'PathPending' -> 'pathPending']")]
		public bool PathPending { get { return pathPending; } }

		/** \details
		 * \deprecated Use approachingPartEndpoint (lowercase 'a') instead
		 */
		[System.Obsolete("Use approachingPartEndpoint (lowercase 'a') instead")]
		public bool ApproachingPartEndpoint { get { return approachingPartEndpoint; } }

		/** \details
		 * \deprecated Use approachingPathEndpoint (lowercase 'a') instead
		 */
		[System.Obsolete("Use approachingPathEndpoint (lowercase 'a') instead")]
		public bool ApproachingPathEndpoint { get { return approachingPathEndpoint; } }

		/** \details
		 * \deprecated This property has been renamed to 'traversingOffMeshLink'
		 */
		[System.Obsolete("This property has been renamed to 'traversingOffMeshLink'. [AstarUpgradable: 'TraversingSpecial' -> 'traversingOffMeshLink']")]
		public bool TraversingSpecial { get { return traversingOffMeshLink; } }

		/** Current waypoint that the agent is moving towards.
		 * \deprecated This property has been renamed to #steeringTarget
		 */
		[System.Obsolete("This property has been renamed to steeringTarget")]
		public Vector3 TargetPoint { get { return steeringTarget; } }

		[UnityEngine.Serialization.FormerlySerializedAs("anim")][SerializeField][HideInInspector]
		Animation animCompatibility;

		/** Anim for off-mesh links.
		 * \deprecated Use the onTraverseOffMeshLink event or the ... component instead. Setting this value will add a ... component
		 */
		[System.Obsolete("Use the onTraverseOffMeshLink event or the ... component instead. Setting this value will add a ... component")]
		public Animation anim {
			get {
				var setter = GetComponent<Pathfinding.Examples.AnimationLinkTraverser>();
				return setter != null ? setter.anim : null;
			}
			set {
				animCompatibility = null;
				var setter = GetComponent<Pathfinding.Examples.AnimationLinkTraverser>();
				if (setter == null) setter = gameObject.AddComponent<Pathfinding.Examples.AnimationLinkTraverser>();
				setter.anim = value;
			}
		}
	}
}
