using UnityEngine;
using System.Collections;

namespace Pathfinding {
	/** Simple patrol behavior.
	 * This will set the destination on the agent so that it moves through the sequence of objects in the #targets array.
	 * Upon reaching a target it will wait for #delay seconds.
	 *
	 * \see #Pathfinding.AIDestinationSetter
	 * \see #Pathfinding.AIPath
	 * \see #Pathfinding.RichAI
	 * \see #Pathfinding.AILerp
	 */
	[UniqueComponent(tag = "ai.destination")]
	[HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_patrol.php")]
	public class Patrol : VersionedMonoBehaviour {

        public bool sheep;

        public bool Spider;

		/** Target points to move to in order */
		public Transform[] targets;

		/** Time in seconds to wait at each target */
		public float delay = 0;

		/** Current target index */
		int index;

        /** For The Random One*/
        public Transform PatrolPoints;

        private void OnEnable()
        {
            if (sheep || Spider)
            {
                var min = 0;
                var max = 13;
                for (int i = 0; i < targets.Length; i++)
                {
                    var target = targets[i];
                    // Change the Patrol Points for Random;
                    targets[i] = PatrolPoints.GetChild(Random.Range(min, max));
                }
            }
            
        }

        IAstarAI agent;
		float switchTime = float.PositiveInfinity;

		protected override void Awake () {
			base.Awake();
			agent = GetComponent<IAstarAI>();
		}

		/** Update is called once per frame */
		void Update () {
			if (targets.Length == 0) return;

			bool search = false;

			if (agent.reachedEndOfPath && !agent.pathPending && float.IsPositiveInfinity(switchTime)) {
				switchTime = Time.time + delay;
			}

			if (Time.time >= switchTime) {
				index = index + 1;
				search = true;
				switchTime = float.PositiveInfinity;
			}

			index = index % targets.Length;
			agent.destination = targets[index].position;

			if (search) agent.SearchPath();
		}
	}
}
