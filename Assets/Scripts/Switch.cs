using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : Part {

    private Transform toggle;
    private float currY;
    private float tarY;

    // Start is called before the first frame update
    void Start () {
        tarY = -0.015f;
        currY = tarY;
        Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < transforms.Length; i++) {
            if (transforms[i].gameObject.name.Equals("Toggle")) {
                toggle = transforms[i];
                break;
            }
        }
        SetState (false);
        SetIsActive(true);
    }

    // Update is called once per frame
    void Update () {
        toggle.localPosition = new Vector3 (0, currY, 0);
        if (currY != tarY) {
            currY += 0.14f * Time.deltaTime * -Mathf.Sign (currY - tarY);
        }
        if (Mathf.Abs(currY - tarY) < 0.002f) {
            currY = Mathf.Sign(currY) * 0.015f;
        }
    }

    void OnMouseDown () {
        if (Input.GetKey (KeyCode.LeftControl)) {
            SetState (!GetState ());
            SimulationManager sim = FindObjectOfType<SimulationManager> ();
            Circuit currCircuit = sim.GetCircuit ();
            currCircuit.SetAllOfId (GetId (), GetState ());
        } else {
            Debug.Log ("clicked " + this.ToString ());
        }
    }

    public override void OnStateUpdate () {
        if (GetState ()) {
            // toggle.transform.localPosition = new Vector3 (0, 0.015f, -0.01f);
            tarY = 0.015f;
        } else {
            // toggle.transform.localPosition = new Vector3 (0, -0.015f, -0.01f);
            tarY = -0.015f;
        }
    }

    public void UpdateState () {

    }
}