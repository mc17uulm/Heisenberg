/**
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

[RequireComponent(typeof(SteamVR_LaserPointer))]
public class VRUIInput : MonoBehaviour
{
	public Text DebugOutput;

    // target point on canvas
	//public GameObject missedBtn;

    public Image targetPoint;

    // missed button object
    public GameObject missedButton;

    // circular progress bar
    public Image progress;

	public Logger log;

    public Text Score;

    // Can be enabled. Only for older project to enable the "shoot"
    // public SteamVR_TrackedController SecondController;

    private const long TIMEOUT = 10000000;

	private SteamVR_LaserPointer laserPointer;
	private SteamVR_TrackedController trackedController;

	private bool PointerInButton = false;
	private bool SecondTriggerClicked = false;
	private long SecondTriggerClickTime = 0;

    //public Button mainBtn;
    //public Button middle;
    public GameObject sphere;
    public GameObject middle;

    public GameObject canvas;

	private int CurrentTrial = 0;

    // Milliseconds the user needs to be inside of the radius before he can shoot
    private int timespan = 500;

    // times the test is done
    private int Times = 5;
    private int Index = 0;
    private int lastPositions = 50;

    // Timer
    private Timer timer;

    // tries
    private List<DataSet> list;

    // Actual DataSet
    private DataSet tmp;

    // User can shoot
    private bool activated = false;

    public Image Background;

    // State
    private State state;
    private Stack<Vector3> store;

    private Vector3 ControllerPositionBefore, ControllerForwardVectorBefore, ControllerUpVectorBefore;

    private LinkedList<GameObject> missedBtns;

    private List<Vector3> positions;
    private List<int> indices;
    private static System.Random rnd = new System.Random();

    private void OnEnable()
	{
        laserPointer = GetComponent<SteamVR_LaserPointer>();

        // Add controller
		trackedController = GetComponent<SteamVR_TrackedController>();
		if (trackedController == null)
		{
			trackedController = GetComponentInParent<SteamVR_TrackedController>();
		}
        if(trackedController == null)
        {
            Debug.Log("Still null");
        } else {
            Debug.Log("not null");
        }


        // Add Events to Controller
		trackedController.TriggerClicked -= HandleTriggerClicked;
		trackedController.TriggerClicked += HandleTriggerClicked;

		trackedController.PadTouched -= HandleTouchPadTouched;
		trackedController.PadTouched += HandleTouchPadTouched;

		trackedController.PadClicked -= HandleTouchPadClicked;
		trackedController.PadClicked += HandleTouchPadClicked;

        //SecondController.TriggerClicked -= HandleSecondTriggerClicked;
        //SecondController.TriggerClicked += HandleSecondTriggerClicked;

        timer = null;
        state = State.START;
        progress.enabled = false;
        store = new Stack<Vector3>();

        Score.text = "0/" + Times;

        missedBtns = new LinkedList<GameObject>();

        positions = new List<Vector3>();
        string[] p = (Resources.Load("positions") as TextAsset).text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
        //Debug.Log("P: " + p);
        foreach(string pos in p)
        {
            Debug.Log("P: " + pos);
            string[] v = pos.Split(' ');
            Vector3 vec = new Vector3(
                (float.Parse(v[0], CultureInfo.InvariantCulture.NumberFormat)),
                (float.Parse(v[1], CultureInfo.InvariantCulture.NumberFormat)),
                (float.Parse(v[2], CultureInfo.InvariantCulture.NumberFormat))
            );
            Debug.Log("Vec: " + vec);
            positions.Add(vec);
        }

        indices =  Enumerable.Range(1, positions.Count).OrderBy(x => rnd.Next()).Take(6).ToList();

    }

    void Start() {
        log.Initialize("TouchPad");
        log.writeHeader("Timestamp;UserID;Trial;Event;OldPosX;OldPosY;OldPosZ;NewPosX;NewPosY;NewPosZ;XDiffPos;YDiffPos;ZDiffPos;Distance;OldFwdX;OldFwdY;OldFwdZ;NewFwdX;NewFwdY;NewFwdZ;XDiffAngle;YDiffAngle;ZDiffAngle;TotalAngle;");

        list = new List<DataSet>();

    }


    private Boolean Contains(RaycastHit[] hits, string name, out GameObject x, out RaycastHit hit) {

        for(int i = 0; i < hits.Length; i++) {
            if(hits[i].collider.gameObject.name.Equals(name)) {
                x = hits[i].collider.gameObject;
                hit = hits[i];
                return true;
            }
        }

        x = null;
        hit = new RaycastHit();
        return false;

    }

    private void Update()
    {
        if(state.Equals(State.START))
        {
            if(Index == 0)
            {
                sphere.transform.localPosition = positions.First<Vector3>();
                progress.transform.localPosition = positions.First<Vector3>();
            }
            else
            {
                Vector3 act = middle.transform.TransformPoint(positions[rnd.Next(positions.Count)]);
                sphere.transform.localPosition = positions[indices[Index - 1]];
                progress.transform.position = positions[indices[Index - 1]];
                
            }
            state = State.RUN;
        }
          //  if (!state.Equals(State.FINISHED)) {
                RaycastHit[] hits;
                hits = Physics.RaycastAll(laserPointer.transform.position, transform.forward, 100.0f);

                GameObject obj;
                RaycastHit hit;

                if (Contains(hits, "Sphere", out obj, out hit)) {

                    if (hits.Length > 0)
                    {
                        store.Push(hits[0].point);
                    }

                    Debug.Log("Pos Btn: " + obj.transform.position);

                    switch (state) {

                        case State.RUN:
                            timer = new Timer();
                            progress.enabled = true;
                            progress.fillAmount = 0;
                            progress.color = new Color(255, 0, 0);
                            //obj.GetComponent <Renderer>().material.color = new Color(255,255,255);
                            state = State.INSIDE;
                            break;

                        case State.INSIDE:
                            if (timer.Finished()) {
                                state = State.ACTIVATED;
                                //tmp = new DataSet(Index, CanvasPos);
                                progress.color = new Color(0, 255, 0);
                                //obj.GetComponent<Renderer>().material.color = new Color(0, 255, 0);
                       
                            Debug.Log("ACTIVATED");
                            } else {
                                float prog = timer.GetProgress(timespan);
                                //Debug.Log("Update progress: " + prog);
                                progress.fillAmount = prog;
                            }
                            break;

                        default:
                            break;

                    }
                } else {
                    if (Contains(hits, "Panel_", out obj, out hit)) {
                        Vector3 tar = hit.point;

                        progress.enabled = false;
                        timer = null;

                        //if (state.Equals(State.INSIDE)) {
                        state = State.RUN;
                        //}
                        EventSystem.current.SetSelectedGameObject(null);
                    }
                }
           // }
            
            // After finished a round, the user can start a new round by clicking the SPACE bar
            if (Input.GetKeyDown(KeyCode.Space) && state.Equals(State.FINISHED)) {
                Debug.Log("Here");
                timer = null;
                state = State.START;
                progress.enabled = false;
                Index = 0;
                Score.text = Index + "/" + Times;
                foreach (GameObject o in missedBtns) {
                    Destroy(o);
                }
                missedBtns = new LinkedList<GameObject>();
                list = new List<DataSet>();
            }
            

    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
	{

        // Accept click only if resting position is set and not too "old"
        if (state.Equals(State.ACTIVATED)) {

            //Debug.Log("Successfull click");
            state = State.CLICKED;

            Debug.Log("CLICKED");
            Debug.Log(store.Peek());
            Stack<Vector3> t = new Stack<Vector3>(store.Reverse());
            Debug.Log(t.Peek());
            Debug.Log(store.Peek());
            store.Clear();

            list.Add(new DataSet(Index, new List<Vector3>()));

            timer = null;
            tmp = null;
            progress.enabled = false;
            Index++;
            Score.text = Index + "/" + Times;



            if (Index == 0)
            {

                state = State.FINISHED;

                foreach(DataSet set in list)
                {
                    Debug.Log("LastPosition: " + set.GetLastPosition());
                    GameObject missed = Instantiate(missedButton, set.GetLastPosition(), Quaternion.identity) as GameObject;
                    missed.transform.parent = canvas.transform;
                    missed.transform.localScale = new Vector3(1, 1, 1);
                    missed.SetActive(true);

                    missedBtns.AddLast(missed);
                    Debug.Log("COUNT: " + missedBtns.Count);

                    Debug.Log(set.ToString());

                    // TODO: save to file

                }


            }
            else
            {
                state = State.RUN;
            }
		}

	}

    

    private void HandleTouchPadTouched(object sender, ClickedEventArgs e)
    {

        // Accept click only if resting position is set and not too "old"
        if (SecondTriggerClicked && System.DateTime.Now.Ticks - SecondTriggerClickTime < TIMEOUT)
        {

            CalculateOffsetTouchTap();


        }
    }

    private void HandleTouchPadClicked(object sender, ClickedEventArgs e)
    {

        // Accept click only if resting position is set and not too "old"
        if (SecondTriggerClicked && System.DateTime.Now.Ticks - SecondTriggerClickTime < TIMEOUT)
        {

            CalculateOffsetTouchClick();
            // reset
            SecondTriggerClicked = false;
            CurrentTrial++;
            //ActualPointerPosition.SetActive(true);
            //ActualPointerPosition.transform.position = laserPointer.pointer.transform.position + (-trackedController.transform.position + laserPointer.pointer.transform.position);
            StartCoroutine("HidePointerFeedback");
        }

    }

    private void HandleSecondTriggerClicked(object sender, ClickedEventArgs e){
		if (PointerInButton) {
			// Set initial position and rotation now
			ControllerPositionBefore = trackedController.transform.position;
			ControllerForwardVectorBefore = trackedController.transform.forward;
			ControllerUpVectorBefore = trackedController.transform.up;
			SecondTriggerClickTime = System.DateTime.Now.Ticks;
			SecondTriggerClicked = true;
		}

	}

	private string CalculateOffsetTriggerClicked(){
		// ##### Position #####
		Vector3 DistanceVector = trackedController.transform.position - ControllerPositionBefore;
		float TotalDistance = DistanceVector.magnitude;

		// ##### Rotation #####
		// X difference
		Vector3 OldForwardVectorProjectedX = Vector3.ProjectOnPlane(ControllerForwardVectorBefore,trackedController.transform.right);
		Vector3 CurrentForwardVectorProjectedX = Vector3.ProjectOnPlane (trackedController.transform.forward, trackedController.transform.right);
		float XDiff = Vector3.SignedAngle (OldForwardVectorProjectedX, CurrentForwardVectorProjectedX, trackedController.transform.right);

		// Y difference
		Vector3 OldForwardVectorProjectedY = Vector3.ProjectOnPlane(ControllerForwardVectorBefore,Vector3.up);
		Vector3 CurrentForwardVectorProjectedY = Vector3.ProjectOnPlane (trackedController.transform.forward, Vector3.up);
		float YDiff = Vector3.SignedAngle (OldForwardVectorProjectedY, CurrentForwardVectorProjectedY, Vector3.up);

		// Z difference
		Vector3 OldUpVectorProjectedZ = Vector3.ProjectOnPlane(ControllerUpVectorBefore,trackedController.transform.forward);
		Vector3 CurrentUpVectorProjectedZ = Vector3.ProjectOnPlane (trackedController.transform.up, trackedController.transform.forward);
		float ZDiff = Vector3.SignedAngle (OldUpVectorProjectedZ, CurrentUpVectorProjectedZ, trackedController.transform.forward);

		// Ovarall difference
		float TotalDiff = Vector3.Angle(ControllerForwardVectorBefore, trackedController.transform.forward);

        return string.Format("X:{0} | Y:{1} | Z:{2}\nDistance:{3}\nTotal:{4}", XDiff, YDiff, ZDiff, TotalDistance, TotalDiff);

		//log.writeToLog(string.Format("{0};TriggerClicked;{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};",CurrentTrial,ControllerPositionBefore.x,ControllerPositionBefore.y,ControllerPositionBefore.z,trackedController.transform.position.x,trackedController.transform.position.y,trackedController.transform.position.z,DistanceVector.x,DistanceVector.y,DistanceVector.z,TotalDistance,ControllerForwardVectorBefore.x,ControllerForwardVectorBefore.y,ControllerForwardVectorBefore.z,trackedController.transform.forward.x,trackedController.transform.forward.y,trackedController.transform.forward.z,XDiff,YDiff,ZDiff,TotalDiff));
			
		//DebugOutput.text = string.Format ("Current trial:{5}\nDistance: {0}\nOffset rotation: X:{1}, Y:{2}, Z:{3}\nTotal offset angle: {4}\n", TotalDistance, XDiff, YDiff, ZDiff, TotalDiff,CurrentTrial);
	}

	private void CalculateOffsetTriggerUnclicked(){
		// ##### Position #####
		Vector3 DistanceVector = trackedController.transform.position - ControllerPositionBefore;
		float TotalDistance = DistanceVector.magnitude;

		// ##### Rotation #####
		// X difference
		Vector3 OldForwardVectorProjectedX = Vector3.ProjectOnPlane(ControllerForwardVectorBefore,trackedController.transform.right);
		Vector3 CurrentForwardVectorProjectedX = Vector3.ProjectOnPlane (trackedController.transform.forward, trackedController.transform.right);
		float XDiff = Vector3.SignedAngle (OldForwardVectorProjectedX, CurrentForwardVectorProjectedX, trackedController.transform.right);

		// Y difference
		Vector3 OldForwardVectorProjectedY = Vector3.ProjectOnPlane(ControllerForwardVectorBefore,Vector3.up);
		Vector3 CurrentForwardVectorProjectedY = Vector3.ProjectOnPlane (trackedController.transform.forward, Vector3.up);
		float YDiff = Vector3.SignedAngle (OldForwardVectorProjectedY, CurrentForwardVectorProjectedY, Vector3.up);

		// Z difference
		Vector3 OldUpVectorProjectedZ = Vector3.ProjectOnPlane(ControllerUpVectorBefore,trackedController.transform.forward);
		Vector3 CurrentUpVectorProjectedZ = Vector3.ProjectOnPlane (trackedController.transform.up, trackedController.transform.forward);
		float ZDiff = Vector3.SignedAngle (OldUpVectorProjectedZ, CurrentUpVectorProjectedZ, trackedController.transform.forward);

		// Ovarall difference
		float TotalDiff = Vector3.Angle(ControllerForwardVectorBefore, trackedController.transform.forward);

		log.writeToLog(string.Format("{0};TriggerUnclick;{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};",CurrentTrial,ControllerPositionBefore.x,ControllerPositionBefore.y,ControllerPositionBefore.z,trackedController.transform.position.x,trackedController.transform.position.y,trackedController.transform.position.z,DistanceVector.x,DistanceVector.y,DistanceVector.z,TotalDistance,ControllerForwardVectorBefore.x,ControllerForwardVectorBefore.y,ControllerForwardVectorBefore.z,trackedController.transform.forward.x,trackedController.transform.forward.y,trackedController.transform.forward.z,XDiff,YDiff,ZDiff,TotalDiff));

		DebugOutput.text += string.Format ("Distance: {0}\nOffset rotation: X:{1}, Y:{2}, Z:{3}\nTotal offset angle: {4}\n", TotalDistance, XDiff, YDiff, ZDiff, TotalDiff);
	}

	private void CalculateOffsetTouchTap(){
		// ##### Position #####
		Vector3 DistanceVector = trackedController.transform.position - ControllerPositionBefore;
		float TotalDistance = DistanceVector.magnitude;

		// ##### Rotation #####
		// X difference
		Vector3 OldForwardVectorProjectedX = Vector3.ProjectOnPlane(ControllerForwardVectorBefore,trackedController.transform.right);
		Vector3 CurrentForwardVectorProjectedX = Vector3.ProjectOnPlane (trackedController.transform.forward, trackedController.transform.right);
		float XDiff = Vector3.SignedAngle (OldForwardVectorProjectedX, CurrentForwardVectorProjectedX, trackedController.transform.right);

		// Y difference
		Vector3 OldForwardVectorProjectedY = Vector3.ProjectOnPlane(ControllerForwardVectorBefore,Vector3.up);
		Vector3 CurrentForwardVectorProjectedY = Vector3.ProjectOnPlane (trackedController.transform.forward, Vector3.up);
		float YDiff = Vector3.SignedAngle (OldForwardVectorProjectedY, CurrentForwardVectorProjectedY, Vector3.up);

		// Z difference
		Vector3 OldUpVectorProjectedZ = Vector3.ProjectOnPlane(ControllerUpVectorBefore,trackedController.transform.forward);
		Vector3 CurrentUpVectorProjectedZ = Vector3.ProjectOnPlane (trackedController.transform.up, trackedController.transform.forward);
		float ZDiff = Vector3.SignedAngle (OldUpVectorProjectedZ, CurrentUpVectorProjectedZ, trackedController.transform.forward);

		// Ovarall difference
		float TotalDiff = Vector3.Angle(ControllerForwardVectorBefore, trackedController.transform.forward);

		log.writeToLog(string.Format("{0};TouchPadTouch;{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};",CurrentTrial,ControllerPositionBefore.x,ControllerPositionBefore.y,ControllerPositionBefore.z,trackedController.transform.position.x,trackedController.transform.position.y,trackedController.transform.position.z,DistanceVector.x,DistanceVector.y,DistanceVector.z,TotalDistance,ControllerForwardVectorBefore.x,ControllerForwardVectorBefore.y,ControllerForwardVectorBefore.z,trackedController.transform.forward.x,trackedController.transform.forward.y,trackedController.transform.forward.z,XDiff,YDiff,ZDiff,TotalDiff));

		DebugOutput.text = string.Format ("Current trial:{5}\nDistance: {0}\nOffset rotation: X:{1}, Y:{2}, Z:{3}\nTotal offset angle: {4}\n", TotalDistance, XDiff, YDiff, ZDiff, TotalDiff,CurrentTrial);
	}



	private void CalculateOffsetTouchClick(){
		// ##### Position #####
		Vector3 DistanceVector = trackedController.transform.position - ControllerPositionBefore;
		float TotalDistance = DistanceVector.magnitude;

		// ##### Rotation #####
		// X difference
		Vector3 OldForwardVectorProjectedX = Vector3.ProjectOnPlane(ControllerForwardVectorBefore,trackedController.transform.right);
		Vector3 CurrentForwardVectorProjectedX = Vector3.ProjectOnPlane (trackedController.transform.forward, trackedController.transform.right);
		float XDiff = Vector3.SignedAngle (OldForwardVectorProjectedX, CurrentForwardVectorProjectedX, trackedController.transform.right);

		// Y difference
		Vector3 OldForwardVectorProjectedY = Vector3.ProjectOnPlane(ControllerForwardVectorBefore,Vector3.up);
		Vector3 CurrentForwardVectorProjectedY = Vector3.ProjectOnPlane (trackedController.transform.forward, Vector3.up);
		float YDiff = Vector3.SignedAngle (OldForwardVectorProjectedY, CurrentForwardVectorProjectedY, Vector3.up);

		// Z difference
		Vector3 OldUpVectorProjectedZ = Vector3.ProjectOnPlane(ControllerUpVectorBefore,trackedController.transform.forward);
		Vector3 CurrentUpVectorProjectedZ = Vector3.ProjectOnPlane (trackedController.transform.up, trackedController.transform.forward);
		float ZDiff = Vector3.SignedAngle (OldUpVectorProjectedZ, CurrentUpVectorProjectedZ, trackedController.transform.forward);

		// Ovarall difference
		float TotalDiff = Vector3.Angle(ControllerForwardVectorBefore, trackedController.transform.forward);

		log.writeToLog(string.Format("{0};TouchPadClick;{1};{2};{3};{4};{5};{6};{7};{8};{9};{10};{11};{12};{13};{14};{15};{16};{17};{18};{19};{20};",CurrentTrial,ControllerPositionBefore.x,ControllerPositionBefore.y,ControllerPositionBefore.z,trackedController.transform.position.x,trackedController.transform.position.y,trackedController.transform.position.z,DistanceVector.x,DistanceVector.y,DistanceVector.z,TotalDistance,ControllerForwardVectorBefore.x,ControllerForwardVectorBefore.y,ControllerForwardVectorBefore.z,trackedController.transform.forward.x,trackedController.transform.forward.y,trackedController.transform.forward.z,XDiff,YDiff,ZDiff,TotalDiff));

		DebugOutput.text += string.Format ("Distance: {0}\nOffset rotation: X:{1}, Y:{2}, Z:{3}\nTotal offset angle: {4}\n", TotalDistance, XDiff, YDiff, ZDiff, TotalDiff);
	}


	public IEnumerator HidePointerFeedback(){
		yield return new WaitForSeconds (2f);
		//ActualPointerPosition.SetActive (false);
	}

}
    */