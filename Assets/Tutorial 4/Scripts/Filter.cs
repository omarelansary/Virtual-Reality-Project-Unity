using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tutorial_4
{
    public class Filter : MonoBehaviour
    {
        [Header("Moving average")]
        [Range(1, 200)] public int samples = 30;
        
        [Header("Single Exponential")]
        [Range(0.01f, 1.0f)] public float seAlpha = 0.03f;
        
        [Header("Double Exponential")]
        [Range(0.0f, 1.0f)] public float deAlpha = 0.04f;
        [Range(0.0f, 1.0f)] public float deBeta = 0.5f;

        [Header("One Euro")] public float frequency = 60f; 

        // TODO put your temp values for filters here
        // 1. Moving Average History
        private Queue<Vector3> _maHistory = new Queue<Vector3>();

        // 2. Single Exponential Previous State
        private Vector3 _sePrevious = Vector3.zero;
        private bool _seInitialized = false;

        // 3. Double Exponential Previous States
        private Vector3 _dePreviousS = Vector3.zero; // Previous Smoothed Position (s_t-1)
        private Vector3 _dePreviousD = Vector3.zero; // Previous Trend (d_t-1)
        private bool _deInitialized = false;

        private OneEuroFilter<Vector3> _oneEuro;

        private void Start()
        {
            _oneEuro = new OneEuroFilter<Vector3>(frequency);
        }

        // TODO implement these filters
        // you can introduce new properties above, below the comment
        
        /* * WHAT IT DOES: 
        * Smooths movement by taking the average of the last 'k' frames.
        * * PROS: Very stable.
        * CONS: Adds "lag" (latency). If you move your head quickly, the camera follows late.
        - In the Game: The HeadTracker.cs calculates your face position (e.g., (0.5, 0.2, -0.4)). It calls filter.DoubleExponential(currentHeadPos). So, value is your current noisy head position.
        - In Debug Mode: The FilterDebug.cs script gets your mouse position. It calls filter.MovingAverage(mousePos). So, value is your mouse position.
        */
        public Vector3 MovingAverage(Vector3 value)
        {
            // Add new value to history
            _maHistory.Enqueue(value);

            // Maintain the size of the queue based on the 'samples' variable
            if (_maHistory.Count > samples)
            {
                _maHistory.Dequeue();
            }

            // Calculate Average
            Vector3 sum = Vector3.zero;
            foreach (Vector3 v in _maHistory)
            {
                sum += v;
            }

            return sum / _maHistory.Count;
        }

        /* * WHAT IT DOES: 
        * Smooths movement by blending the new position with the previous smoothed position.
        * * MATH: NewPos = (RawInput * alpha) + (OldPos * (1 - alpha))
        * * ALPHA (seAlpha): 
        * - High Alpha (1.0): Trusts the raw input completely. No smoothing, no lag.
        * - Low Alpha (0.01): Trusts the old position. Very smooth, lots of lag.
        */
        public Vector3 SingleExponential(Vector3 value)
        {
            // Initialize if first frame
            if (!_seInitialized)
            {
                _sePrevious = value;
                _seInitialized = true;
                return value;
            }

            // Apply Formula
            Vector3 result = (value * seAlpha) + ((1.0f - seAlpha) * _sePrevious);
            
            // Update state
            _sePrevious = result;

            return result;
        }

        /* * WHAT IT DOES: 
        * A smarter filter that tracks Position AND Velocity (Trend).
        * It helps reduce the "lag" seen in the Single Exponential filter.
        * * VARIABLES:
        * - s_t: The smoothed position.
        * - d_t: The trend (how fast/direction it is moving).
        */
        public Vector3 DoubleExponential(Vector3 value)
        {
            // Initialize if first frame
            if (!_deInitialized)
            {
                _dePreviousS = value;
                _dePreviousD = Vector3.zero;
                _deInitialized = true;
                return value;
            }

            // 1. Calculate new Smoothed Position (s_t)
            Vector3 currentS = (value * deAlpha) + ((1.0f - deAlpha) * (_dePreviousS + _dePreviousD));

            // 2. Calculate new Trend (d_t)
            Vector3 currentD = (deBeta * (currentS - _dePreviousS)) + ((1.0f - deBeta) * _dePreviousD);

            // 3. Update state variables for next frame
            _dePreviousS = currentS;
            _dePreviousD = currentD;

            return currentS;
        }

        public Vector3 OneEuro(Vector3 value)
        {
            return _oneEuro.Filter(value);
        }
    }
}