﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;


[RequireComponent(typeof(SteamVR_LaserPointer))]
public class Processing_OLD : MonoBehaviour {

    // roter Ring um Ziel um Countdown anzuzeigen
    public Image progressIndicator;

    // Gibt Anzahl der Durchgänge an
    public Text scoreText;

    // Sphere welche als Ziel und als Referenzpunkt genutzt werden
    public GameObject targetSphere;
    // Zeigt Treffer an
    public GameObject indicatorButton;
    public GameObject canvas;

    private SteamVR_LaserPointer laserPointer;
    private SteamVR_TrackedController trackedController;

    private Timer timer;
    // Angabe der ms für welche der Nutzer im Ziel sein muss um klicken zu können
    private int timespan = 500;

    private State state;
    private List<Vector3> stack;
    private List<Vector3> targetPositions;
    private List<GameObject> missedPositions;
    public static System.Random rand = new System.Random();
    private int Index;
    private IDictionary<string, object> config;
    private int Tries;
    private Session session;
    private Try t;

    void OnEnable()
    {

        config = Config.Load();

        session = new Session();

        laserPointer = GetComponent<SteamVR_LaserPointer>();
        trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = GetComponentInParent<SteamVR_TrackedController>();
        }
        if (trackedController == null)
        {
            Debug.Log("TrackedContoroller still null");
            Application.Quit();
        }

        trackedController.TriggerClicked -= ControllerOnClick;
        trackedController.TriggerClicked += ControllerOnClick;

        Reset();

        LoadPositions();

        Index = 0;
        Tries = 1;

        t = new Try(Tries);


    }

    // Update is called once per frame
    void Update() {

        ProcessHits();

        ProcessButtons();

    }

    private void ControllerOnClick(object sender, ClickedEventArgs e)
    {
        /**
        if (state.Equals(State.ACTIVATED))
        {
            state = State.CLICKED;

            timer = null;
            progressIndicator.enabled = false;
            Index++;

            int hi = (int)config["repeat"] == 1 ? Index : (Index % (int)config["repeat"] == 0 ? (int)config["repeat"] : Index % (int)config["repeat"]);
            Debug.Log("hi: " + hi);
            //t.AddHit(new Hit(hi, targetSphere.transform.position, stack, false));

            scoreText.text = "Versuch: " + (Tries - 1) + "/" + config["tries"] + "\r\nPosition: " + ((t.GetHits().Count / (int)config["repeat"])) + "/" + targetPositions.Count;

            if (Index >= (int)config["repeat"] * targetPositions.Count)
            {
                session.AddTry(t);
                Tries++;
                state = State.FINISHED;
                targetSphere.SetActive(false);

                if (Tries > (int)config["tries"])
                {
                    state = State.FINISHED;
                    targetSphere.SetActive(false);
                    scoreText.text = "Finished";
                    Debug.Log("finished");
                    int ind = 0;
                    foreach (Try tr in session.GetTries()) {
                        foreach (Hit h in tr.GetHits())
                        {
                            // Erster hit auf target um position von target zu ermitteln
                            if (tr.GetIndex() == 1 && ind == 0)
                            {
                                GameObject target = Instantiate(targetSphere, h.GetTarget(), Quaternion.identity) as GameObject;
                                target.transform.parent = canvas.transform;
                                target.transform.localScale = new Vector3((int)config["dimension"], (int)config["dimension"], 1);
                                target.SetActive(true);
                                missedPositions.Add(target);
                            }
                            Vector3 m = (bool)config["last_position"] ? h.GetLastPosition() : h.GetAverage();
                            GameObject clicked = Instantiate(indicatorButton, m, Quaternion.identity) as GameObject;
                            clicked.transform.parent = canvas.transform;
                            clicked.transform.localScale = new Vector3(1, 1, 1);
                            clicked.SetActive(true);

                            missedPositions.Add(clicked);
                        }
                    }
                    ind++;

                    session.SaveToFile((string)config["savefile"]);
                }
            }
            else if (state.Equals(State.FIRST) || state.Equals(State.RUN))
            {
                t.AddHit(new Hit(1, targetSphere.transform.position, stack, true));
            }
            else
            {
                // Checkt ob Durchgänge für diese Position bereits durchlaufen wurden
                if (Index % (int)config["repeat"] == 0)
                {
                    SetTargets();
                }
                state = State.RUN;
            }

            stack = new List<Vector3>();
        }*/

    }

    private void ProcessHits()
    {
       /**( if (!state.Equals(State.FINISHED))
        {
            RaycastHit[] hits;
            hits = Physics.RaycastAll(laserPointer.transform.position, transform.forward, 100.0f);

            GameObject obj;
            RaycastHit hit;

            if (Contains(hits, "Sphere", out obj, out hit))
            {

                if (hits.Length > 0)
                {
                    stack.Add(hits[0].point);
                }

                switch (state)
                {

                    case State.RUN:
                        state = State.FIRST;
                        break;

                    case State.SECOND:
                        timer = new Timer();
                        progressIndicator.enabled = true;
                        progressIndicator.fillAmount = 0;
                        progressIndicator.color = new Color(255, 0, 0);
                        state = State.INSIDE;
                        break;

                    case State.INSIDE:
                        if (timer.Finished())
                        {
                            state = State.ACTIVATED;
                            progressIndicator.color = new Color(0, 255, 0);
                        }
                        else
                        {
                            float progress = timer.GetProgress(timespan);
                            progressIndicator.fillAmount = progress;
                        }
                        break;

                    default:
                        break;
                }

            }
            else if (Contains(hits, "TargetPanel", out obj, out hit))
            {
                if (state.Equals(State.ACTIVATED))
                {
                    stack.Add(hits[0].point);
                }
                else
                {
                    progressIndicator.enabled = false;
                    timer = null;
                    state = State.RUN;
                }
            }
        }*/
    }

    private void SetTargets()
    {
        int o = t.GetHits().Count / (int)config["repeat"];
        targetSphere.transform.localPosition = targetPositions[o];
        progressIndicator.transform.localPosition = targetPositions[o];
    }

    private Boolean Contains(RaycastHit[] hits, string name, out GameObject x, out RaycastHit hit)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.gameObject.name.Equals(name))
            {
                x = hits[i].collider.gameObject;
                hit = hits[i];
                return true;
            }
        }

        x = null;
        hit = new RaycastHit();
        return false;
    }

    private void Reset()
    {
        timer = null;
        state = State.START;
        progressIndicator.enabled = false;
        stack = new List<Vector3>();
        missedPositions = new List<GameObject>();
    }

    private void LoadPositions()
    {

        targetPositions = new List<Vector3>();
        //List<string> p = (Resources.Load("positions") as TextAsset).text.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
        List<Vector3> p = ((Vector3[])config["positions"]).ToList<Vector3>();
        if ((bool)config["random"])
        {
            Vector3 first = p[0];
            p.RemoveAt(0);
            p = p.OrderBy(x => rand.Next()).ToList();
            p.Insert(0, first);
        }
        targetPositions = p;
        scoreText.text = "Versuch: 0/" + config["tries"] + "\r\nPosition: 0/" + targetPositions.Count;

        Vector3 panel = canvas.transform.localPosition;
        panel.z = (int)config["distance"];
        canvas.transform.localPosition = panel;
        targetSphere.transform.localPosition = targetPositions[0];
        //(targetSphere.transform as RectTransform).sizeDelta = new Vector2((int)config["dimension"], (int)config["dimension"]);
        targetSphere.transform.localScale = new Vector3((int)config["dimension"], (int)config["dimension"], 1);
        progressIndicator.transform.localPosition = targetPositions[0];
        float dimension = (int)config["dimension"] * 4;
        progressIndicator.transform.localScale = new Vector3(dimension, dimension, 1);
    }

    private void ProcessButtons()
    {

        if (Input.GetKeyDown(KeyCode.Space) && state.Equals(State.FINISHED))
        {
           /** Reset();
            Index = 0;
            t = new Try(Tries);
            stack = new List<Vector3>();
            state = State.RUN;
            targetSphere.SetActive(true);
            targetSphere.transform.localPosition = targetPositions[0];
            progressIndicator.transform.localPosition = targetPositions[0];
            scoreText.text = "Versuch: " + (Tries - 1) + "/" + config["tries"] + "\r\nPosition: 0/" + targetPositions.Count;*/
        }

    }
}