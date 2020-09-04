﻿using System;
using LogicGateSimulator.Parts;
using UnityEngine;

namespace LogicGateSimulator.PartBehaviors
{
    public abstract class PartBehavior : MonoBehaviour //, IPointerDownHandler
    {
        // [SerializeField] protected Color ActiveColor; // (0f, 0.7882353f, 0.0902f) bright green
        // [SerializeField] protected Color InactiveColor; // (0.04705883f, 0.454902f, 0.1137255f) dark green
        // [SerializeField] protected Color SelectedColor; // (0.4478532f, 0.8867924f, 0f) another bright green

        protected readonly Color ActiveColor = new Color(0f, 0.7882353f, 0.0902f); // bright green
        protected readonly Color InactiveColor = new Color(0.04705883f, 0.454902f, 0.1137255f); // dark green
        protected readonly Color SelectedColor = new Color(0.4478532f, 0.8867924f, 0f); // another bright green

        
        private bool _isSelected;
        private bool _isPreview;
        
        public event Action<PartBehavior> SelectChanged;
        public event Action<bool, PartBehavior> MouseHover;
        
        private Part _partObject;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer selectionRenderer;
        [SerializeField] private Material previewMaterial;
        [SerializeField] private Material placedMaterial;

        public Part PartObject
        {
            get => this._partObject;
            set
            {
                if (PartObject != null) PartObject.StateChanged -= OnStateChanged;
                this._partObject = value;
                if (PartObject != null) PartObject.StateChanged += OnStateChanged;
                OnPartObjectChanged();
            }
        }

        public abstract Type PartType { get; }
        public SpriteRenderer SpriteRenderer => spriteRenderer;
        public SpriteRenderer SelectionSpriteRenderer => selectionRenderer;

        public bool Selected
        {
            get => this._isSelected;
            set
            {
                if (this._isSelected == value) return;
                this._isSelected = value;
                SelectChanged?.Invoke(this);
            }
        }

        private void Awake()
        {
            SelectChanged += OnSelectChanged;
            UpdateColor();
        }

        private void Update()
        {
            
        }

        private void OnMouseEnter() => MouseHover?.Invoke(true, this);
        
        private void OnMouseExit() => MouseHover?.Invoke(false, this);

        public virtual void OnStateChanged(Part part)
        {
            UpdateColor();
        }

        protected virtual void OnSelectChanged(PartBehavior partBehavior)
        {
            UpdateColor();
        }

        protected virtual void OnPartObjectChanged()
        {
            UpdateColor();
        }

        protected virtual void UpdateColor()
        {
            if (spriteRenderer != null)
            {
                if (PartObject != null)
                {
                    SpriteRenderer.color = PartObject.State ? this.ActiveColor : this.InactiveColor;
                }
            }

            SelectionSpriteRenderer.enabled = this.Selected;
        }

        public SimulationManager GetSim()
        {
            return SimulationManager.Sim();
        }

        //public void OnPointerDown(PointerEventData eventData)
        public void OnMouseDown()
        {
            if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && PartObject.Active)
            {
                PartObject.State = !PartObject.State;
            }
            else
            {
                this.Selected = !this.Selected; // toggle selected
            }
        }
    }
}