using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Globalization;
using System.Threading.Tasks;

// The class that handles the Players' animations

namespace Revivd {
    public class PlayerAnimation : MonoBehaviour {

        /**
         *CSVfilesPath : path to the CSV files
         * p1Colors, p2Colors : table of type Color that contains the startcolor and endcolor
         * linesWidth : list of type float that contains the width of lines for each player
         * p1ChildrenLines, p2ChildrenLines : list of type Gameobject that contains the gameobjects which holds the LineRenderers
         * p1NbOfPointsInLineHolders, p2NbOfPointsInLineHolders: table of type int that contains the number of points in each LineRenderer
         * posOffset : table of Vector3 that contains the offset for each player
         * showPaddle : table of bool that contains the bool values to show or hide the raquettes for each player
         * paddlePos : Matrix 2x2 that contains the two points which define the position and direction of the raquettes for each Player
         * loaded : bool that used to check if the Animation is loaded or not
         * totNbOfPoints : total number of points in a player animation
         * p1Counter, p2Counter : counters used in the animation of each players
         * finished : bool that used to check if the animation is finished or not
         * p1PaddlePosition, p2PaddlePosition : table of type Vector3 that holds the positions of the 2 points used to position the raquettes got from the list of positions
         * canceled : bool used to cancel the Animation if there's a error in the loading of the CSV data
         * task : object of type task used to create the thread that loads the CSV data
         * p1paddle, p2paddle : gameobjects used to move the raquettes
         * p1Paddlego, p2Paddlego : raquettes prefabs
         */
 


        static PlayerAnimation _instance;
        public static PlayerAnimation Instance { get { return _instance; } }

        public string CSVfilesPath;
        public Color[] p1Colors;
        public Color[] p2Colors;
        public float[] linesWidth;
        public List<LineRenderer> p1ChildrenLines;
        public int[] p1NbOfPointsInLineHolders;
        public List<List<Vector3>> p1Positions;
        public List<LineRenderer> p2ChildrenLines;
        public int[] p2NbOfPointsInLineHolders;
        public List<List<Vector3>> p2Positions;
        public Vector3[] posOffset;
        public bool[] showPaddle;
        public int[,] paddlePos;
        public bool loaded;
        public int totNbOfPoints;
        public int p1Counter;
        public int p2Counter;
        public bool finished;
        public Vector3[] p1PaddlePositions;
        public Vector3[] p2PaddlePositions;

        private bool canceled;
        private Task task;
        private GameObject p1Paddle;
        private GameObject p2Paddle;
        [SerializeField] private GameObject p1Paddlego;
        [SerializeField] private GameObject p2Paddlego;

        // initialization of the variables
        void Awake() {
            if (_instance != null) {
                Debug.LogWarning("Multiple instances of PlayerVisualization singleton");
            }
            _instance = this;

            loaded = false;
            finished = false;
            canceled = false;

            posOffset = new Vector3[2];
            p1ChildrenLines = new List<LineRenderer>();
            p1Positions = new List<List<Vector3>>();
            p1NbOfPointsInLineHolders = new int[0];
            p2ChildrenLines = new List<LineRenderer>();
            p2Positions = new List<List<Vector3>>();
            p2NbOfPointsInLineHolders = new int[0];

            showPaddle = new bool[2] { false, false };
            paddlePos = new int[2, 2];
            p1PaddlePositions = new Vector3[2];
            p2PaddlePositions = new Vector3[2];
        }

        void Update() {
            if (!loaded)
                return;
            // shows/hides the raquettes in the scene

            if (p1Paddle != null) {
                if (showPaddle[0]) {
                    p1Paddle.SetActive(true);
                }
                else {
                    p1Paddle.SetActive(false);
                }
            }
            if (p2Paddle != null) {
                if (showPaddle[1]) {
                    p2Paddle.SetActive(true);
                }
                else {
                    p2Paddle.SetActive(false);
                }
            }

        }

        // Task that loads the CSV data
        public async Task Load(List<Revivd.Path> displayedPaths, int pathIndex) {

            if (loaded) {
                Unload();
            }

            // load the CSV data only if there's only one path that is displayed
            if (displayedPaths.Count == 1) {
                bool readCSV = true;
                try {
                    canceled = false;
                    task = Task.Run(() => ReadCSVFiles(CSVfilesPath, pathIndex));
                    await task;
                }
                catch (System.Exception e) {
                    Revivd.ControlPanel.Instance.MakeErrorWindow("Error reading players' Mouvements data\n\n" + e.Message);
                    readCSV = false;
                }

                if (!readCSV) {
                    Debug.LogError("Failed loading data from CSV file");
                    loaded = false;
                }
                else {
                    Debug.Log("Successfully loaded CSV file");
                    loaded = true;
                    finished = false;

                    // Default values if they are incorrect

                    linesWidth[0] = (linesWidth[0] <= 0) ? 0.05f : linesWidth[0];
                    linesWidth[1] = (linesWidth[1] <= 0) ? 0.05f : linesWidth[1];

                    // ApplyOffsets to the players 

                    ApplyOffsets(1, posOffset[0]);
                    ApplyOffsets(2, posOffset[1]);

                    // Create Playres Lines

                    CreatePlayerLineHolders(1);
                    CreatePlayerLineHolders(2);

                    p1Counter = 0;
                    p2Counter = 0;

                }

            }
        }

        public void Unload() {

            foreach (Transform child in this.transform)
                Destroy(child.gameObject);
            if (p1Paddle != null) {
                Destroy(p1Paddle);
            }
            if (p2Paddle != null) {
                Destroy(p2Paddle);
            }
            p1ChildrenLines.Clear();
            p1Positions.Clear();
            p1NbOfPointsInLineHolders = new int[0];
            p2ChildrenLines.Clear();
            p2Positions.Clear();
            p2NbOfPointsInLineHolders = new int[0];
            p1PaddlePositions = new Vector3[2];
            p2PaddlePositions = new Vector3[2];
            p1Counter = 0;
            p2Counter = 0;
            loaded = false;
            canceled = false;
            finished = false;
            Debug.Log("Successfully unloaded the Animation");
        }

        void ReadCSVFiles(string filepath, int index) {

            bool readP1Data = false;
            bool readP2Data = false;

            if (File.Exists(filepath + "/data" + index + "P" + 1 + ".csv")) {
                readP1Data = ReadPlayerCSVFile(1, out p1Positions, out p1NbOfPointsInLineHolders);
            }
            else {
                Debug.LogWarning("Didn't found data for player 1");
            }

            if (File.Exists(filepath + "/data" + index + "P" + 2 + ".csv")) {
                readP2Data = ReadPlayerCSVFile(2, out p2Positions, out p2NbOfPointsInLineHolders);
            }
            else {
                Debug.LogWarning("Didn't found data for player 2");
            }

            if (!readP1Data && !readP2Data) {
                throw new System.Exception("Neither Player 1 or Player 2 Data could be loaded");
            }

            Debug.Log("Finished ReadingCSV");

            // loop through each line in the CSV files and store it in positions variable

            bool ReadPlayerCSVFile(int player, out List<List<Vector3>> positions, out int[] nbOfPointsInLineHolders) {
                List<List<Vector3>> pos = new List<List<Vector3>>();
                StreamReader streamReader = new StreamReader(filepath + "/data" + index + "P" + player + ".csv");
                string data_String = streamReader.ReadLine();
                var data_values = data_String.Split(',');
                nbOfPointsInLineHolders = new int[data_values.Length];
                totNbOfPoints = 0; // get the number of points from the first line in the CSV
                for (int i = 0; i < data_values.Length; i++) {
                    nbOfPointsInLineHolders[i] = int.Parse(data_values[i]);
                    totNbOfPoints += nbOfPointsInLineHolders[i];
                }
                bool endOfFile = false;
                while (!endOfFile && !canceled) { // if didn't cancel the loading by user
                    data_String = streamReader.ReadLine();
                    if (data_String == null) {
                        endOfFile = true;
                        break;
                    }
                    data_values = data_String.Split(',');
                    if (data_values.Length != 3 * totNbOfPoints) {
                        break;
                    }
                    List<Vector3> data_Vector3 = new List<Vector3>();
                    for (int i = 0; i < totNbOfPoints; i++) {
                        Vector3 position = new Vector3(float.Parse(data_values[3 * i], CultureInfo.InvariantCulture), float.Parse(data_values[3 * i + 1], CultureInfo.InvariantCulture), float.Parse(data_values[3 * i + 2], CultureInfo.InvariantCulture));
                        data_Vector3.Add(position);
                    }
                    pos.Add(data_Vector3);
                }
                positions = pos;
                return true;
            }
        }

        // create child gameobjects with linerenderer components used to draw a line
        void CreatePlayerLineHolders(int player) {
            if (player == 1) {
                GameObject player1 = new GameObject();
                player1.transform.parent = this.transform;
                for (int i = 0; i < p1NbOfPointsInLineHolders.Length; i++) {
                    GameObject lineHolder = new GameObject();
                    lineHolder.transform.position = Vector3.zero;
                    lineHolder.transform.rotation = Quaternion.identity;
                    lineHolder.name = "lineHolder " + i;
                    lineHolder.transform.parent = player1.transform;

                    LineRenderer line = CreateALine(lineHolder, p1NbOfPointsInLineHolders[i], p1Colors);
                    p1ChildrenLines.Add(line);
                }
            }
            else if (player == 2) {
                GameObject player2 = new GameObject();
                player2.transform.parent = this.transform;
                for (int i = 0; i < p2NbOfPointsInLineHolders.Length; i++) {
                    GameObject lineHolder = new GameObject();
                    lineHolder.transform.position = Vector3.zero;
                    lineHolder.transform.rotation = Quaternion.identity;
                    lineHolder.name = "lineHolder " + i;
                    lineHolder.transform.parent = player2.transform;

                    LineRenderer line = CreateALine(lineHolder, p2NbOfPointsInLineHolders[i], p2Colors);
                    p2ChildrenLines.Add(line);
                }
            }

            LineRenderer CreateALine(GameObject gameObject, int nbOfPointsInLineHolder, Color[] colors) {
                LineRenderer line = gameObject.AddComponent<LineRenderer>();
                line.material = new Material(Shader.Find("Sprites/Default"));
                line.widthMultiplier = linesWidth[0];
                line.startColor = colors[0];
                line.endColor = colors[1];
                line.positionCount = nbOfPointsInLineHolder;
                line.SetPositions(new Vector3[line.positionCount]);
                line.useWorldSpace = false;
                return line;
            }
        }

        // function that sets the positions of the points in each Linerenderer points 
        public IEnumerator DrawLines(float secondsBetweenUpdates) {
            //Draws new lines each frame
            Debug.Log("StartedCoroutine");
            while (!finished && !canceled) {
                if (p1ChildrenLines.Count != 0 && p1Counter < p1Positions.Count) { //Player 1
                    int p1DrawCount = 0;
                    for (int i = 0; i < p1ChildrenLines.Count; i++) {
                        LineRenderer line = p1ChildrenLines[i];
                        var pos = new Vector3[line.positionCount];
                        for (int j = 0; j < line.positionCount; j++) {
                            pos[j] = p1Positions[p1Counter][p1DrawCount + j];
                        }
                        line.SetPositions(pos);
                        p1DrawCount += line.positionCount;
                    }

                    MouveP1Paddle(p1Counter);

                    p1Counter++;
                }
                if (p2ChildrenLines.Count != 0 && p2Counter < p2Positions.Count) { // Player 2
                    int p2DrawCount = 0;
                    for (int i = 0; i < p2ChildrenLines.Count; i++) {
                        LineRenderer line = p2ChildrenLines[i];
                        var pos = new Vector3[line.positionCount];
                        for (int j = 0; j < pos.Length; j++) {
                            pos[j] = p2Positions[p2Counter][p2DrawCount + j];
                        }
                        line.SetPositions(pos);
                        p2DrawCount += line.positionCount;
                    }

                    MouveP2Paddle(p2Counter);

                    p2Counter++;
                }
                yield return new WaitForSecondsRealtime(secondsBetweenUpdates);
                finished = (p1Counter >= p1Positions.Count && p2Counter >= p2Positions.Count) ? true : false;
            }
        }

        // function to move the raquette of Player 1
        void MouveP1Paddle(int p1Counter) {
            p1PaddlePositions[0] = p1Positions[p1Counter][paddlePos[0, 0]];
            p1PaddlePositions[1] = p1Positions[p1Counter][paddlePos[0, 1]];

            if (p1Paddle == null) {
                p1Paddle = Instantiate(p1Paddlego, p1PaddlePositions[1], Quaternion.FromToRotation(Vector3.forward, p1PaddlePositions[0] - p1PaddlePositions[1]));
            }

            p1Paddle.transform.position = p1PaddlePositions[1];
            Quaternion p1PaddleRot = Quaternion.FromToRotation(Vector3.forward, p1PaddlePositions[0] - p1PaddlePositions[1]);
            p1Paddle.transform.rotation = p1PaddleRot;
            p1Paddle.transform.Rotate(0, 0, 90 - p1PaddleRot.eulerAngles.z);
        }

        // function to move the raquette of Player 2
        void MouveP2Paddle(int p2Counter) {
            p2PaddlePositions[0] = p2Positions[p2Counter][paddlePos[1, 0]];
            p2PaddlePositions[1] = p2Positions[p2Counter][paddlePos[1, 1]];

            if (p2Paddle == null) {
                p2Paddle = Instantiate(p2Paddlego, p2PaddlePositions[1], Quaternion.FromToRotation(Vector3.forward, p2PaddlePositions[0] - p2PaddlePositions[1]));
            }

            p2Paddle.transform.position = p2PaddlePositions[1];
            Quaternion p2PaddleRot = Quaternion.FromToRotation(Vector3.forward, p2PaddlePositions[0] - p2PaddlePositions[1]);
            p2Paddle.transform.rotation = p2PaddleRot;
            p2Paddle.transform.Rotate(0, 0, 90 - p2PaddleRot.eulerAngles.z);
        }

        public void ApplyOffsets(int player, Vector3 posOffset) {
            if (player == 1) {
                foreach (List<Vector3> pos in p1Positions) {
                    for (int i = 0; i < pos.Count; i++) {
                        pos[i] += posOffset;
                    }
                }
            }
            
            if (player == 2) {
                foreach (List<Vector3> pos in p2Positions) {
                    for (int i = 0; i < pos.Count; i++) {
                        pos[i] += posOffset;
                    }
                }
            }
        }
    }
}
