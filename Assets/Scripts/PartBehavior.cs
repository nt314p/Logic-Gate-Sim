using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PartBehavior : MonoBehaviour, IPointerDownHandler
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
    public event Action<PartBehavior> SelectChanged;
    private bool sr;
    private Part partObject;

    public Part PartObject {
        get => this.partObject;
        set 
        {
            this.partObject = value;
            PartObject.StateChanged += OnStateChanged;
        }
    }

    public SpriteRenderer SRenderer { get; set; }

    public bool Selected
    {
        get => this.isSelected;
        set
        {
            if (this.isSelected != value) SelectChanged?.Invoke(this);
            this.isSelected = value;
            GetSim().ToggleSelected(this); // not sure about this line...
        }
    }

    private void Update()
    {
        if (PartObject.HasStateUpdate()) OnStateChanged();
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

    public virtual void OnStateChanged(Part part)
    {
        UpdateColor();
    }
    public virtual void OnSelectUpdate()
    {
        UpdateColor();
    }

    void OnMouseDown()
    {
        
    }

    public virtual void UpdateColor()
    {
        SRenderer.color = PartObject.State ? this.ActiveColor : this.InactiveColor;
        if (this.Selected) SRenderer.color = this.SelectedColor;
    }

    public SimulationManager GetSim()
    {
        return SimulationManager.sim();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Clicked " + this.ToString());
        if (Input.GetKey(KeyCode.LeftControl) && PartObject.Active)
        {
            PartObject.State = !PartObject.State;
            GetSim().GetCircuit().CalculateStateId(PartObject.Id);
        }
        else
        {
            this.Selected = !this.Selected; // toggle selected
        }
    }
}
