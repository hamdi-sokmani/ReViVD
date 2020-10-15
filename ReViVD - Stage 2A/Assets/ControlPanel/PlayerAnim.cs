using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Revivd {
    public class PlayerAnim : MonoBehaviour {

        /* Class the handles the inputs in UI for a player animation */

        public InputField lineWidth;
        public Dropdown startColor;
        public Dropdown endColor;
        public InputField posOffset_x;
        public InputField posOffset_y;
        public InputField posOffset_z;
        public Toggle showPaddle;
        public GameObject paddlePos;
        public InputField paddlePos1;
        public InputField paddlePos2;

        PlayerAnimation playAnim;

        private void Awake() {
            playAnim = PlayerAnimation.Instance;
        }

        // set Listeners to input fields that change the value in the Classes when they are changed in the UI

        private void OnEnable() {
            lineWidth.onEndEdit.AddListener(delegate {
                if (playAnim.loaded) {
                    float value = Tools.ParseField_f(lineWidth, 0.05f);
                    if (transform.name == "Inputs Player1") {
                        playAnim.linesWidth[0] = value;
                    }
                    if (transform.name == "Inputs Player2") {
                        playAnim.linesWidth[1] = value;
                    }
                }
            });

            startColor.onValueChanged.AddListener(delegate {
                if (playAnim.loaded) {
                    int color = startColor.value;
                    Color c = Color.blue;
                    switch (color) {
                        case 0: {
                                c = Color.red;
                                break;
                            }
                        case 1: {
                                c = Color.green;
                                break;
                            }
                        case 2: {
                                c = Color.blue;
                                break;
                            }
                    }
                    if (transform.name == "Inputs Player1") {
                        playAnim.p1Colors[0] = c;
                    }
                    if (transform.name == "Inputs Player2") {
                        playAnim.p2Colors[0] = c;
                    }
                }
            });

            endColor.onValueChanged.AddListener(delegate {
                if (playAnim.loaded) {
                    int color = endColor.value;
                    Color c = Color.blue;
                    switch (color) {
                        case 0: {
                                c = Color.red;
                                break;
                            }
                        case 1: {
                                c = Color.green;
                                break;
                            }
                        case 2: {
                                c = Color.blue;
                                break;
                            }
                    }
                    if (transform.name == "Inputs Player1") {
                        playAnim.p1Colors[1] = c;
                    }
                    if (transform.name == "Inputs Player2") {
                        playAnim.p2Colors[1] = c;
                    }
                }
            });

            posOffset_x.onEndEdit.AddListener(delegate {
                if (playAnim.loaded) {
                    if (transform.name == "Inputs Player1") {
                        Vector3 oldOffset = playAnim.posOffset[0];
                        playAnim.ApplyOffsets(1, new Vector3(-oldOffset.x, 0, 0));
                        playAnim.posOffset[0].x = Tools.ParseField_f(posOffset_x, 0);
                        Vector3 newOffset = playAnim.posOffset[0];
                        playAnim.ApplyOffsets(1, new Vector3(newOffset.x, 0, 0));
                    }
                    if (transform.name == "Inputs Player2") {
                        Vector3 oldOffset = playAnim.posOffset[1];
                        playAnim.ApplyOffsets(2, new Vector3(-oldOffset.x, 0, 0));
                        playAnim.posOffset[1].x = Tools.ParseField_f(posOffset_x, 0);
                        Vector3 newOffset = playAnim.posOffset[1];
                        playAnim.ApplyOffsets(2, new Vector3(newOffset.x, 0, 0));
                    }
                }
            });

            posOffset_y.onEndEdit.AddListener(delegate {
                if (playAnim.loaded) {
                    if (transform.name == "Inputs Player1") {
                        Vector3 oldOffset = playAnim.posOffset[0];
                        playAnim.ApplyOffsets(1, new Vector3(0, -oldOffset.y, 0));
                        playAnim.posOffset[0].y = Tools.ParseField_f(posOffset_y, 0);
                        Vector3 newOffset = playAnim.posOffset[0];
                        playAnim.ApplyOffsets(1, new Vector3(0, newOffset.y, 0));
                    }
                    if (transform.name == "Inputs Player2") {
                        Vector3 oldOffset = playAnim.posOffset[1];
                        playAnim.ApplyOffsets(2, new Vector3(0, -oldOffset.y, 0));
                        playAnim.posOffset[1].y = Tools.ParseField_f(posOffset_y, 0);
                        Vector3 newOffset = playAnim.posOffset[1];
                        playAnim.ApplyOffsets(2, new Vector3(0, newOffset.y, 0));
                    }
                }
            });

            posOffset_z.onEndEdit.AddListener(delegate {
                if (playAnim.loaded) {
                    if (transform.name == "Inputs Player1") {
                        Vector3 oldOffset = playAnim.posOffset[0];
                        playAnim.ApplyOffsets(1, new Vector3(0, 0, -oldOffset.z));
                        playAnim.posOffset[0].z = Tools.ParseField_f(posOffset_z, 0);
                        Vector3 newOffset = playAnim.posOffset[0];
                        playAnim.ApplyOffsets(1, new Vector3(0, 0, newOffset.z));
                    }
                    if (transform.name == "Inputs Player2") {
                        Vector3 oldOffset = playAnim.posOffset[1];
                        playAnim.ApplyOffsets(2, new Vector3(0, 0, -oldOffset.z));
                        playAnim.posOffset[1].z = Tools.ParseField_f(posOffset_z, 0);
                        Vector3 newOffset = playAnim.posOffset[1];
                        playAnim.ApplyOffsets(2, new Vector3(0, 0, newOffset.z));
                    }
                }
            });

            showPaddle.onValueChanged.AddListener((bool isOn) => {
                if (playAnim.loaded) {
                    if (showPaddle.isOn) {
                        paddlePos.SetActive(true);
                        if (transform.name == "Inputs Player1") {
                            playAnim.showPaddle[0] = true;
                        }
                        if (transform.name == "Inputs Player2") {
                            playAnim.showPaddle[1] = true;
                        }
                    }
                    else {
                        paddlePos.SetActive(false);
                        if (transform.name == "Inputs Player1") {
                            playAnim.showPaddle[0] = false;
                        }
                        if (transform.name == "Inputs Player2") {
                            playAnim.showPaddle[1] = false;
                        }
                    }
                }
            });
            paddlePos1.onEndEdit.AddListener(delegate {
                if (playAnim.loaded) {
                    int value = Tools.ParseField_i(paddlePos1, 13);
                    if (value >= 0 && value < playAnim.totNbOfPoints) {
                        if (transform.name == "Inputs Player1") {
                            playAnim.paddlePos[0, 0] = Tools.ParseField_i(paddlePos1, 13); ;
                        }
                        if (transform.name == "Inputs Player2") {
                            playAnim.paddlePos[1, 0] = Tools.ParseField_i(paddlePos1, 13);
                        }
                    }
                }
            });
            paddlePos2.onEndEdit.AddListener(delegate {
                if (playAnim.loaded) {
                    int value = Tools.ParseField_i(paddlePos2, 14);
                    if (value >= 0 && value < playAnim.totNbOfPoints) {
                        if (transform.name == "Inputs Player1") {
                            playAnim.paddlePos[0, 1] = Tools.ParseField_i(paddlePos2, 14);
                        }
                        if (transform.name == "Inputs Player2") {
                            playAnim.paddlePos[1, 1] = Tools.ParseField_i(paddlePos2, 14);
                        }
                    }
                }
            });
        }

        private void OnDisable() {
            lineWidth.onEndEdit.RemoveAllListeners();
            startColor.onValueChanged.RemoveAllListeners();
            endColor.onValueChanged.RemoveAllListeners();
            posOffset_x.onEndEdit.RemoveAllListeners();
            posOffset_y.onEndEdit.RemoveAllListeners();
            posOffset_z.onEndEdit.RemoveAllListeners();
            paddlePos1.onEndEdit.RemoveAllListeners();
            paddlePos2.onEndEdit.RemoveAllListeners();
        }
    }
}
