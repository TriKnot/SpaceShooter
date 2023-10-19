using System;
using System.Collections.Generic;
using ScriptableObjects.Variables;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIHandler : MonoBehaviour, IVariableObserver<int>
    {
        [SerializeField] private TextMeshProUGUI _asteroidText;
        [SerializeField] private TextMeshProUGUI _asteroidPieceText;
        [SerializeField] private TextMeshProUGUI _fpsText;
        
        [SerializeField] private IntVariableSO _asteroidCount;
        [SerializeField] private IntVariableSO _asteroidPieceCount;
        
        private Dictionary<int, string> _cachedNumberStrings = new();
        private int[] _frameRateSamples;
        private int _cacheNumbersAmount = 300;
        private int _averageFromAmount = 30;
        private int _averageCounter = 0;
        private int _currentAveraged;

        void Awake()
        {
            // Cache strings and create array
            for (int i = 0; i < _cacheNumbersAmount; i++)
            {
                _cachedNumberStrings[i] = i.ToString();
            }

            _frameRateSamples = new int[_averageFromAmount];
            
            _asteroidCount.RegisterObserver(this);
            _asteroidPieceCount.RegisterObserver(this);
        }

        private void OnDestroy()
        {
            _asteroidCount.UnregisterObserver(this);
        }

        void Update()
        {
            // Sample
            {
                var currentFrame =
                    (int)Math.Round(1f /
                                    Time.smoothDeltaTime); // If your game modifies Time.timeScale, use unscaledDeltaTime and smooth manually (or not).
                _frameRateSamples[_averageCounter] = currentFrame;
            }

            // Average
            {
                var average = 0f;

                foreach (var frameRate in _frameRateSamples)
                {
                    average += frameRate;
                }

                _currentAveraged = (int)Math.Round(average / _averageFromAmount);
                _averageCounter = (_averageCounter + 1) % _averageFromAmount;
            }

            // Assign to UI
            {
                 string fps = _currentAveraged switch
                {
                    var x when x >= 0 && x < _cacheNumbersAmount => _cachedNumberStrings[x],
                    var x when x >= _cacheNumbersAmount => $"> {_cacheNumbersAmount}",
                    var x when x < 0 => "< 0",
                    _ => "?"
                };
                 
                 _fpsText.text = "FPS: " + fps;
            }
        }

        private void UpdateAsteroidCountText()
        {
            _asteroidText.text = "Asteroids: " + _asteroidCount.Value;    
        }
        
        private void UpdateAsteroidPieceCountText()
        {
            _asteroidPieceText.text = "Asteroid pieces: " + _asteroidPieceCount.Value;    
        }

        public void OnValueChanged(int newValue)
        {
            UpdateAsteroidCountText();
            UpdateAsteroidPieceCountText();
        }
    }
}
