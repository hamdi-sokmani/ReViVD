using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Revivd {
    public class BallAnimation : MonoBehaviour {

        /* *
         * iterator : iterator used to move the balls
         * finished : bool to check if the mouvement is finished or not
         * loaded : bool to check if the Animation is loaded or not
         * projectionPoint : point of the projection of the ball on the table
         * displayedPath : Path that is displayed
         * timeBall : GameObject of the ball 
         * ball : prefab of a ball
         */

        static BallAnimation _instance;
        public static BallAnimation Instance { get { return _instance; } }

        public int iterator;
        public bool finished;

        private bool loaded;
        private Vector3 projectionPoint;
        private LineRenderer projectionLine;
        private Path displayedPath;
        private GameObject timeBall;
        [SerializeField] private GameObject ball;

        private void Awake() {
            if (_instance != null) {
                Debug.LogWarning("Multiple instances of BallAnimation singleton");
            }
            _instance = this;

            loaded = false;
            finished = false;
            iterator = 1;
            displayedPath = new Path();
        }

        public void Load(List<Path> dispPaths) {
            if (loaded) {
                Unload();
            }
            
            // Setup the projection line to be drawn in the scene

            if (dispPaths.Count == 1) {
                displayedPath = dispPaths[0];
                iterator = 0;
                timeBall = Instantiate(ball, displayedPath.atoms[iterator].point, Quaternion.identity);
                projectionLine = timeBall.AddComponent<LineRenderer>();
                projectionLine.material = new Material(Shader.Find("Sprites/Default"));
                projectionLine.widthMultiplier = 0.02f;
                projectionLine.startColor = Color.gray;
                projectionLine.endColor = Color.gray;
                projectionLine.positionCount = 2;
                projectionLine.SetPositions(new Vector3[projectionLine.positionCount]);
                loaded = true;
                finished = false;
            }
        }

        private void Unload() {
            projectionLine.positionCount = 0;
            Destroy(projectionLine);
            Destroy(timeBall);
            displayedPath = new Path();
            iterator = 0;
            loaded = false;
            finished = false;
        }

        private void Update() {
            if (!loaded)
                return;
        }

        // MoveBallPosition function to move the ball after delai

        public IEnumerator MoveBallPosition(float secondsBetweenUpdates) {
            float t;
            Vector3 origpos;
            Vector3 targpos;
            while (!finished && loaded) {
                t = 0;
                origpos = timeBall.transform.position;
                targpos = displayedPath.atoms[iterator].point;
                while (t < secondsBetweenUpdates) {
                    Vector3 ballpos = Vector3.Lerp(origpos, targpos, t / secondsBetweenUpdates); //interpolation of points between two position used to smooth the mvt of the balls
                    timeBall.transform.localPosition = ballpos;
                    t += Time.deltaTime;
                    RaycastHit hit;
                    if (Physics.Raycast(timeBall.transform.position, transform.TransformDirection(Vector3.down), out hit)) {
                        if (hit.transform.name == "Table") {
                            projectionPoint = hit.point;
                        }
                        else {
                            projectionPoint = timeBall.transform.position;
                        }
                    }
                    else {
                        projectionPoint = timeBall.transform.position;
                    }
                    projectionLine.SetPosition(0, timeBall.transform.position);
                    projectionLine.SetPosition(1, projectionPoint);
                    yield return null;
                }
                timeBall.transform.position = targpos;
                iterator++;
                finished = (iterator >= displayedPath.atoms.Count) ? true : false;
            }
        }
    }
}
