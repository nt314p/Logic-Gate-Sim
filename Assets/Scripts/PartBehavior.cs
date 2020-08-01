using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PartBehavior : MonoBehaviour
{
    /*[SerializeField]
    protected Color ActiveColor; // (0f, 0.7882353f, 0.0902f) bright green

    [SerializeField]
    protected Color InactiveColor; // (0.04705883f, 0.454902f, 0.1137255f) dark green

    [SerializeField]
    protected Color SelectedColor; // (0.4478532f, 0.8867924f, 0f) another bright green*/

    protected readonly Color ActiveColor = new Color(0f, 0.7882353f, 0.0902f); // bright green
    protected readonly Color InactiveColor = new Color(0.04705883f, 0.454902f, 0.1137255f); // dark green
    protected readonly Color SelectedColor = new Color(0.4478532f, 0.8867924f, 0f); // another bright green

    private bool isSelected;
    private bool hasSelectedUpdate;
    private bool sr;

    public Part PartObj
    {
        get; set;
    }

    public SpriteRenderer SRenderer
    {
        get; set;
    }

    public bool Selected
    {
        get => this.isSelected;
        set
        {
            if (this.isSelected != value) hasSelectedUpdate = true;
            this.isSelected = value;
            GetSim().ToggleSelected(this); // not sure about this line...
        }
    }

    private void Update()
    {
        if (PartObj.HasStateUpdate()) OnStateUpdate();
        if (hasSelectedUpdate)
        {
            hasSelectedUpdate = false;
            OnSelectUpdate();
        }
        ChildUpdate();
    }

    public virtual void ChildUpdate()
    {

    }

    public virtual void OnStateUpdate()
    {
        UpdateColor();
    }
    public virtual void OnSelectUpdate()
    {
        UpdateColor();
    }

    void OnMouseDown()
    {
        Debug.Log("Clicked " + this.ToString());
        if (Input.GetKey(KeyCode.LeftControl) && PartObj.Active)
        {
            PartObj.State = !PartObj.State;
            GetSim().GetCircuit().CalculateStateId(PartObj.Id);
        } else
        {
            this.Selected = !this.Selected; // toggle selected
        }
    }

    public virtual void UpdateColor()
    {
        SRenderer.color = PartObj.State ? this.ActiveColor : this.InactiveColor;
        if (this.Selected) SRenderer.color = this.SelectedColor;
    }

    public SimulationManager GetSim()
    {
        return SimulationManager.sim();
    }
}
