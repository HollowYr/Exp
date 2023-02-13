﻿using PathCreation.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace SplineMesh
{
    /// <summary>
    /// Example of component to places assets along a spline. This component can be used as-is but will most likely be a base for your own component.
    /// 
    /// In this example, the user gives the prefab to place, a spacing value between two placements, the prefab scale and an horizontal offset to the spline.
    /// These three last values have an additional range, allowing to add some randomness. for each placement, the computed value will be between value and value+range.
    /// 
    /// Prefabs are placed from the start of the spline at computed spacing, unitl there is no lentgh remaining. Prefabs are stored, destroyed
    /// and built again each time the spline or one of its curves change.
    /// 
    /// A random seed is used to obtain the same random numbers at each update. The user can specify the seed to test some other random number set.
    /// 
    /// Place prefab along a spline and deform it easily have a lot of usages if you have some imagination : 
    ///  - place trees along a road
    ///  - create a rocky bridge
    ///  - create a footstep track with decals
    ///  - create a path of firefly in the dark
    ///  - create a natural wall with overlapping rocks
    ///  - etc.
    /// </summary>
    [ExecuteInEditMode]
    [SelectionBase]
    [DisallowMultipleComponent]
    public class ExampleSower : MonoBehaviour
    {
        internal GameObject generated;
        private Spline spline = null;
        private bool toUpdate = true;
        [SerializeField] private SplineMeshTiling meshTiling;
        [SerializeField] private bool hideChildren = true;
        public GameObject prefab = null;
        public float scale = 1, scaleRange = 0;
        public float spacing = 1, spacingRange = 0;
        public float offset = 0, offsetRange = 0;
        public bool isRandomYaw = false;
        public int randomSeed = 0;

        private void OnEnable()
        {
            string generatedName = "generated by " + GetType().Name;
            var generatedTranform = transform.Find(generatedName);
            generated = generatedTranform != null ? generatedTranform.gameObject : UOUtility.Create(generatedName, gameObject);
            meshTiling = GetComponent<SplineMeshTiling>();
            spline = GetComponentInParent<Spline>();
            spline.NodeListChanged += (s, e) =>
            {
                toUpdate = true;
                foreach (CubicBezierCurve curve in spline.GetCurves())
                {
                    curve.Changed.AddListener(() => toUpdate = true);
                }
            };
            foreach (CubicBezierCurve curve in spline.GetCurves())
            {
                curve.Changed.AddListener(() => toUpdate = true);
            }
        }

        private void Awake()
        {
            this.enabled = false;
        }

        private void OnValidate()
        {
            toUpdate = true;
        }

        private void Update()
        {
            if (toUpdate)
            {
                Sow();
                toUpdate = false;
                ConvertSplineSowerToBezierPath();
            }
        }

        private void ConvertSplineSowerToBezierPath()
        {
            Transform[] children = generated.transform.GetAllChildrenArray();
            Transform splineMesh = meshTiling.generated.GetComponentInChildren<MeshCollider>().transform;
            UOUtility.DestroyChildren(splineMesh.gameObject);
            GameObject path = new GameObject();
            path.name = "Bezier path";
            splineMesh.gameObject.layer = LayerMask.NameToLayer("GrindRails");

            path.transform.SetParent(splineMesh);
            GeneratePathExample bezierPath = path.AddComponent<GeneratePathExample>();

            bezierPath.waypoints = children;
            bezierPath.closedLoop = spline.IsLoop;
            bezierPath.Generate();
            if (hideChildren == false) return;
            children.ToList().ForEach(child => child.gameObject.SetActive(false));
        }

        public void Sow()
        {
            UOUtility.DestroyChildren(generated);

            UnityEngine.Random.InitState(randomSeed);
            if (spacing + spacingRange <= 0 ||
                prefab == null)
                return;

            float distance = 0;
            while (distance <= spline.Length)
            {
                CurveSample sample = spline.GetSampleAtDistance(distance);

                GameObject go;
                go = Instantiate(prefab, generated.transform);
                go.transform.localRotation = Quaternion.identity;
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;

                // move along spline, according to spacing + random
                go.transform.localPosition = sample.location;
                // apply scale + random
                float rangedScale = scale + UnityEngine.Random.Range(0, scaleRange);
                go.transform.localScale = new Vector3(rangedScale, rangedScale, rangedScale);
                // rotate with random yaw
                if (isRandomYaw)
                {
                    go.transform.Rotate(0, 0, UnityEngine.Random.Range(-180, 180));
                }
                else
                {
                    go.transform.rotation = sample.Rotation;
                }
                // move orthogonaly to the spline, according to offset + random
                var binormal = (Quaternion.LookRotation(sample.tangent, sample.up) * Vector3.right).normalized;
                var localOffset = offset + UnityEngine.Random.Range(0, offsetRange * Math.Sign(offset));
                localOffset *= sample.scale.x;
                binormal *= localOffset;
                go.transform.position += binormal;

                distance += spacing + UnityEngine.Random.Range(0, spacingRange);
            }
        }
    }
}