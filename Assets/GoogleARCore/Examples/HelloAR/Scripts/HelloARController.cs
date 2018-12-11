//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.HelloAR
{
    using System.Collections.Generic;
    using GoogleARCore;
    using GoogleARCore.Examples.Common;
    using UnityEngine;

#if UNITY_EDITOR
    // Set up touch input propagation while using Instant Preview in the editor.
    using Input = InstantPreviewInput;
#endif

    /// <summary>
    /// Controls the HelloAR example.
    /// </summary>
    public class HelloARController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject DetectedPlanePrefab;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject RedRobot;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a feature point.
        /// </summary>
        public GameObject BlueRobot;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a feature point.
        /// </summary>
        public GameObject Arena;
        
        /// <summary>
        /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
        /// </summary>
        public GameObject SearchingForPlaneUI;

        /// <summary>
        /// True if the robots and arena have been created.
        /// </summary>
        public bool CreatedMatch = false;

        /// <summary>
        /// The plane generator script so we can turn off plane detecting
        /// </summary>
        public GameObject planeGeneratorObj;

        /// <summary>
        /// The point generator script so we can turn off point detecting
        /// </summary>
        public GameObject pointGeneratorObj;

        /// <summary>
        /// The rotation in degrees need to apply to model when the Andy model is placed.
        /// </summary>
        private const float k_ModelRotation = 180.0f;

        /// <summary>
        /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<DetectedPlane> m_AllPlanes = new List<DetectedPlane>();

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;

        private void Start()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            //planeGeneratorObj = GameObject.FindGameObjectWithTag("PlaneGenerator");
            //pointGeneratorObj = GameObject.FindGameObjectWithTag("PointGenerator");
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            _UpdateApplicationLifecycle();

            if (!CreatedMatch)
            {
                // Hide snackbar when currently tracking at least one plane.
                Session.GetTrackables<DetectedPlane>(m_AllPlanes);
                bool showSearchingUI = true;
                for (int i = 0; i < m_AllPlanes.Count; i++)
                {
                    if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
                    {
                        showSearchingUI = false;
                        break;
                    }
                }

                SearchingForPlaneUI.SetActive(showSearchingUI);
            }
            // If the player has not touched the screen, we are done with this update.
            Touch touch;
            if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
            {
                return;
            }

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
                TrackableHitFlags.FeaturePointWithSurfaceNormal;

            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
            {
                // Use hit pose and camera pose to check if hittest is from the
                // back of the plane, if it is, no need to create the anchor.
                if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                        hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else if (!CreatedMatch)
                {
                    // Instantiate robot models and arena at the hit pose.
                    var Stage = Instantiate(Arena, hit.Pose.position, hit.Pose.rotation);

                    Vector3 pos = Stage.transform.position;
                    pos.x = Stage.transform.position.x + 1.218f * RedRobot.transform.localScale.x;
                    pos.y = pos.y + (RedRobot.transform.localScale.y * 0.5f);

                    var Red = Instantiate(RedRobot, pos, Stage.transform.rotation);

                    pos.x = -pos.x;

                    var Blue = Instantiate(BlueRobot, pos, Stage.transform.rotation);

                    // Compensate for the hitPose rotation facing away from the raycast (i.e. camera).
                    //andyObject.transform.Rotate(0, k_ModelRotation, 0, Space.Self);

                    // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                    // world evolves.
                    var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                    
                    // Make New Prefabs model a child of the anchor.
                    Stage.transform.parent = anchor.transform;
                    Red.transform.parent = anchor.transform;
                    Blue.transform.parent = anchor.transform;


                    // fix rotation of actors
                    Quaternion rot = Stage.transform.rotation;

                    rot.x = 0.0f;
                    rot.y = 180;
                    rot.z = 0.0f;

                    Blue.transform.localRotation = rot;

                    // fix the robots postions
                    Blue.transform.localPosition = new Vector3(Stage.transform.localPosition.x - (1.218f * BlueRobot.transform.localScale.x), BlueRobot.transform.localScale.y / 2, 0.0f);

                    Red.transform.localPosition = new Vector3(Stage.transform.localPosition.x + (1.218f * RedRobot.transform.localScale.x), RedRobot.transform.localScale.y / 2, 0.0f);

                    // prevents user from creating more then 1 arena
                    CreatedMatch = true;

                    // stop detecting new surfaces
                    planeGeneratorObj.GetComponent<DetectedPlaneGenerator>().StopDetecting = true;
                    pointGeneratorObj.GetComponent<PointcloudVisualizer>().StopDetecting = true;
                }
            }
        }

        /// <summary>
        /// Check and update the application lifecycle.
        /// </summary>
        private void _UpdateApplicationLifecycle()
        {
            // Exit the app when the 'back' button is pressed.
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            // Only allow the screen to sleep when not tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
            }
            else
            {
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}
