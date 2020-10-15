using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Revivd {
    public class VideoAnimation : MonoBehaviour {

        /**
         * VideoFilesPath : path to the videos folder
         * videoPlayer : videoPlayer Component
         * videoPlayerScreen : videoPlayer Gameobject
         */

        static VideoAnimation _instance;
        public static VideoAnimation Instance { get { return _instance; } }
        public string videoFilesPath;

        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private GameObject videoPlayerScreen;

        private void Awake() {
            if (_instance != null) {
                Debug.LogWarning("Multiple instances of VideoAnimation singleton");
            }
            _instance = this;
            videoPlayerScreen.SetActive(false);
        }

        // Loads the video from the the folders
        public void GetVideo(int pathIndex) {

            string videoPath = videoFilesPath + "/vid" + pathIndex + ".mp4";
            if (System.IO.File.Exists(videoPath)) {
                videoPlayer.url = videoPath;
                videoPlayerScreen.SetActive(true);
                videoPlayer.Play();
            }
            else {
                videoPath = videoFilesPath + "/vid" + pathIndex + ".wav";
                if (System.IO.File.Exists(videoPath)) {
                    videoPlayer.url = videoPath;
                    videoPlayerScreen.SetActive(true);
                    videoPlayer.Play();
                }
                else {
                    Debug.LogError("Can't find the correspondant video for this path");
                    StopVideo();
                }
            }
        }

        public void StopVideo() {
            videoPlayer.Stop();
            videoPlayerScreen.SetActive(false);
        }
    }
}

