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

    private bool _isSelected;
    public event Action<PartBehavior> SelectChanged;
    [SerializeField] protected Part _partObject;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public Part PartObject
    {
        get => this._partObject;
        set
        {
            if (PartObject != null) PartObject.StateChanged -= OnStateChanged;
            this._partObject = value;
            if (PartObject != null) PartObject.StateChanged += OnStateChanged;
        }
    }

    public SpriteRenderer SpriteRenderer
    {
        get => _spriteRenderer;
        set => _spriteRenderer = value;
    }

    public bool Selected
    {
        get => this._isSelected;
        set
        {
            if (this._isSelected != value) SelectChanged?.Invoke(this);
            this._isSelected = value;
        }
    }

    private void Awake()
    {
        SelectChanged += OnSelectChanged;
    }

    public virtual void OnStateChanged(Part part)
    {
        UpdateColor();
    }
    public virtual void OnSelectChanged(PartBehavior partBehavior)
    {
        UpdateColor();
    }

    void OnMouseDown()
    {

    }

    public virtual void UpdateColor()
    {
        SpriteRenderer.color = PartObject.State ? this.ActiveColor : this.InactiveColor;
        if (this._isSelected) SpriteRenderer.color = this.SelectedColor;
    }

    public SimulationManager GetSim()
    {
        return SimulationManager.Sim();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Clicked " + this);
        if (Input.GetKey(KeyCode.LeftControl) && PartObject.Active)
        {
            PartObject.State = !PartObject.State;
            // GetSim().GetCircuit().CalculateStateId(PartObject.Id);
        }
        else
        {
            this.Selected = !this.Selected; // toggle selected
        }
    }
}
