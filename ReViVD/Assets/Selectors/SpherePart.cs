﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Revivd {

    public class SpherePart : SelectorPart {

        public float radius = 0.5f;
        public Vector3 handOffset = new Vector3(0f, 0f, 2.5f);

        protected override void CreatePrimitive() {
            primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        }

        protected override void UpdatePrimitive() {
            primitive.transform.localPosition = handOffset;
            primitive.transform.localRotation = Quaternion.identity;
            primitive.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        }

        protected override void UpdateManualModifications() {
            if (SteamVR_ControllerManager.RightController.triggerPressed) {
                radius += radius * SelectorManager.Instance.creationGrowthCoefficient * Time.deltaTime;
            }
            if (SteamVR_ControllerManager.LeftController.triggerPressed) {
                radius -= radius * SelectorManager.Instance.creationGrowthCoefficient * Time.deltaTime;
            }

            handOffset.z += SelectorManager.Instance.creationMovementCoefficient * SteamVR_ControllerManager.RightController.Joystick.y * Time.deltaTime;
            
            if (SteamVR_ControllerManager.RightController.padPressed) {
                if (SteamVR_ControllerManager.RightController.Pad.x >= 0) {
                    if (Mathf.Abs(SteamVR_ControllerManager.RightController.Pad.y) < 0.7071) {
                        handOffset.x += SelectorManager.Instance.creationMovementCoefficient * SteamVR_ControllerManager.RightController.Pad.x * Time.deltaTime;
                    }
                    else {
                        handOffset.y += SelectorManager.Instance.creationMovementCoefficient * SteamVR_ControllerManager.RightController.Pad.y * Time.deltaTime;
                    }
                }
                else {
                    if (Mathf.Abs(SteamVR_ControllerManager.RightController.Pad.y) < 0.7071) {
                        handOffset.x += SelectorManager.Instance.creationMovementCoefficient * SteamVR_ControllerManager.RightController.Pad.x * Time.deltaTime;
                    }
                    else {
                        handOffset.y += SelectorManager.Instance.creationMovementCoefficient * SteamVR_ControllerManager.RightController.Pad.y * Time.deltaTime;
                    }
                }
            }



        }

        protected override void ParseRibbonsToCheck() {
            Vector3 sphereCenter = primitive.transform.position;

            foreach (Atom a in checkedRibbons) {
                if (!a.path.specialRadii.TryGetValue(a.indexInPath, out float ribbonRadius))
                    ribbonRadius = a.path.baseRadius;
                if (DistancePointSegment(sphereCenter, a.path.transform.TransformPoint(a.point), a.path.transform.TransformPoint(a.path.AtomsAsBase[a.indexInPath + 1].point)) < radius + ribbonRadius) {
                    touchedRibbons.Add(a);
                }
            }
        }

        private float DistancePointSegment(Vector3 point, Vector3 a, Vector3 b) {
            if (Vector3.Dot(point - a, b - a) <= 0) {
                return (point - a).magnitude;
            }
            if (Vector3.Dot(point - b, a - b) <= 0) {
                return (point - b).magnitude;
            }
            return Vector3.Cross(b - a, point - a).magnitude / (b - a).magnitude;
        }
    }

}