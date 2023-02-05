using UnityEngine;

namespace PathCreation.Examples
{
    // Example of creating a path at runtime from a set of points.

    [RequireComponent(typeof(PathCreator))]
    public class GeneratePathExample : MonoBehaviour
    {
        public bool closedLoop = true;
        public Transform[] waypoints;
        private BezierPath bezierPath;

        public void Generate()
        {
            if (waypoints.Length > 0)
            {
                if (transform.parent != null)
                    transform.position = transform.InverseTransformPoint(Vector3.zero);
                // Create a new bezier path from the waypoints.
                bezierPath = new BezierPath(waypoints, closedLoop, PathSpace.xyz);

                GetComponent<PathCreator>().bezierPath = bezierPath;
            }
        }

        //private void OnDrawGizmos()
        //{
        //    int points = bezierPath.NumPoints;
        //    int j = 0;
        //    for (int i = 0; i < points; i++)
        //    {
        //        if (i % 3 != 0) continue;
        //        Gizmos.DrawWireSphere(bezierPath.GetPoint(i), .1f);
        //        Debug.Log($"{bezierPath.GetPoint(i)}");


        //        j++;
        //    }
        //}
    }
}