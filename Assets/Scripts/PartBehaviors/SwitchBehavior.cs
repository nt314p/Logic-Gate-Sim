﻿using System;
using LogicGateSimulator.Parts;
using UnityEngine;

namespace LogicGateSimulator.PartBehaviors
{
    public class SwitchBehavior : PartBehavior
    {
        [SerializeField] private Transform _toggle;
        private const float Offset = 0.015f;
        private const float TargetTolerance = 0.002f;
        private const float ToggleSpeed = 0.14f;
        private float _targetY = -Offset;
        private float _currentY = -Offset;

        public override Type PartType => typeof(Switch);
        
        private void LateUpdate()
        {
            _toggle.localPosition = Vector3.up * _currentY; // new Vector3(0, currentY, 0); 

            if (Mathf.Abs(_currentY - _targetY) < TargetTolerance)
            {
                _currentY = Mathf.Sign(_currentY) * Offset;
            }
            else
            {
                _currentY += ToggleSpeed * Time.deltaTime * -Mathf.Sign(_currentY - _targetY);
            }
        }

        public override void OnStateChanged(Part part)
        {
            _targetY = Offset * (part.State ? 1 : -1);
        }
    }
}
