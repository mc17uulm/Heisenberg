using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

public enum State
{
    START,
    RUN,
    INSIDE,
    ACTIVATED,
    CLICKED,
    FINISHED
}

public enum Mode
{
    LASTPOS,
    AVG
}

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class Processing : MonoBehaviour {

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

    private List<DataSet> list;
    private State state;
    private List<Vector3> stack;
    private List<Vector3> targetPositions;
    private List<GameObject> missedPositions;
    private static System.Random rand = new System.Random();
    private int Index;
    private Mode indication;
    private IDictionary<string, object> config;
    private int tries;
    private Session session;

    void OnEnable()
    {

        config = Config.Load();

        session = new Session("1");
        
        indication = (bool) config["last_position"] ? Mode.LASTPOS : Mode.AVG;

        laserPointer = GetComponent<SteamVR_LaserPointer>();
        trackedController = GetComponent<SteamVR_TrackedController>();
        if(trackedController == null)
        {
            trackedController = GetComponentInParent<SteamVR_TrackedController>();
        }
        if(trackedController == null)
        {
            Debug.Log("TrackedContoroller still null");
            Application.Quit();
        }

        trackedController.TriggerClicked -= ControllerOnClick;
        trackedController.TriggerClicked += ControllerOnClick;

        Reset();

        LoadPositions();

        Index = 0;
        tries = 0;
        list = new List<DataSet>();


    }
	
	// Update is called once per frame
	void Update () {

        ProcessHits();

        ProcessButtons();

	}

    private void ControllerOnClick(object sender, ClickedEventArgs e)
    {

        if(state.Equals(State.ACTIVATED))
        {
            state = State.CLICKED;
            list.Add(new DataSet(Index, stack, targetSphere));
            timer = null;
            progressIndicator.enabled = false;
            Index++;
           

            if(Index >= targetPositions.Count)
            {
                foreach(DataSet set in list)
                {
                    GameObject clicked = Instantiate(indicatorButton, set.GetPosition(indication), Quaternion.identity) as GameObject;
                    clicked.transform.parent = canvas.transform;
                    clicked.transform.localScale = new Vector3(1, 1, 1);
                    clicked.SetActive(true);

                    missedPositions.Add(clicked);
                }
                session.AddTry(list);
            }
            else
            {
                // Checkt ob Durchgänge für diese Position bereits durchlaufen wurden
                if ((targetPositions.Count * (int)config["repeat"]) % (Index + 1) == 0)
                {
                    SetTargets();
                }
                //state = State.START;
            }
        }

    }

    private void ProcessHits()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(laserPointer.transform.position, transform.forward, 100.0f);

        GameObject obj;
        RaycastHit hit;

        if(Contains(hits, "Sphere", out obj, out hit))
        {

            if(hits.Length > 0)
            {
                stack.Add(hits[0].point);
            }

            switch(state)
            {

                case State.RUN:
                    timer = new Timer();
                    progressIndicator.enabled = true;
                    progressIndicator.fillAmount = 0;
                    progressIndicator.color = new Color(255, 0, 0);
                    state = State.INSIDE;
                    break;

                case State.INSIDE:
                    if(timer.Finished())
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
        else
        {
            if(Contains(hits, "Panel_", out obj, out hit))
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
        }
    }

    private void SetTargets()
    {
        int t = ((targetPositions.Count * (int)config["repeat"]) / (Index + 1)) - 1; 
        targetSphere.transform.localPosition = targetPositions[t];
        progressIndicator.transform.localPosition = targetPositions[t];
    }

    private Boolean Contains(RaycastHit[] hits, string name, out GameObject x, out RaycastHit hit)
    {
        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i].collider.gameObject.name.Equals(name))
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
        if ((bool) config["random"])
        {
            Vector3 first = p[0];
            p.RemoveAt(0);
            p = p.OrderBy(x => rand.Next()).ToList();
            p.Insert(0, first);
        }
        targetPositions = p;
        targetSphere.transform.localPosition = targetPositions[0];
        progressIndicator.transform.localPosition = targetPositions[0];
    }

    private void ProcessButtons()
    {

        if(Input.GetKeyDown(KeyCode.Space) && state.Equals(State.FINISHED))
        {
            foreach(GameObject o in missedPositions)
            {
                Destroy(o);
            }
            Reset();
            list = new List<DataSet>();
            Index = 0;
            tries++;
            if(tries == (int) config["tries"])
            {
                Debug.Log("Session finished");
                // Save to file
            }
        }

    }
}
