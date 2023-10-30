using System;
using System.Collections.Generic;
using ScriptableObjects.Variables;
using TMPro;
using UnityEngine;

namespace UI
{
    public class InGameUIHandler : MonoBehaviour, IVariableObserver<int>
    {
        [SerializeField] private TextMeshProUGUI _asteroidText;
        [SerializeField] private TextMeshProUGUI _asteroidPieceText;
        [SerializeField] private TextMeshProUGUI _fpsText;

        [SerializeField] private IntVariableSO _asteroidCount;
        
        [SerializeField] private float _updateInterval = 0.1f;

        private readonly Dictionary<int, string> _cachedNumberStrings = new();
        private int[] _frameRateSamples;
        private readonly int _cacheNumbersAmount = 1000;
        private readonly int _averageFromAmount = 30;
        private int _averageCounter = 0;
        private int _currentAveraged;
        
        private float _timeSinceLastUpdate = 0f;

        void Awake()
        {
            CacheNumberStrings();
            InitializeFrameRateSamples();

            RegisterObservers();
        }

        private void OnDestroy()
        {
            UnregisterObservers();
        }

        void Update()
        {
            SampleFrameRate();
            CalculateAverageFrameRate();
            if(_timeSinceLastUpdate < _updateInterval)
                _timeSinceLastUpdate += Time.deltaTime;
            else
            {
                UpdateFPSUI();
                _timeSinceLastUpdate = 0f;
            }

        }

        private void CacheNumberStrings()
        {
            for (int i = 0; i < _cacheNumbersAmount; i++)
            {
                _cachedNumberStrings[i] = i.ToString();
            }
        }

        private void InitializeFrameRateSamples()
        {
            _frameRateSamples = new int[_averageFromAmount];
        }

        private void RegisterObservers()
        {
            _asteroidCount.RegisterObserver(this);
        }

        private void UnregisterObservers()
        {
            _asteroidCount.UnregisterObserver(this);
        }

        private void SampleFrameRate()
        {
            var currentFrame = (int)Math.Round(1f / Time.smoothDeltaTime);
            _frameRateSamples[_averageCounter] = currentFrame;
        }

        private void CalculateAverageFrameRate()
        {
            var average = 0f;

            foreach (var frameRate in _frameRateSamples)
            {
                average += frameRate;
            }

            _currentAveraged = (int)Math.Round(average / _averageFromAmount);
            _averageCounter = (_averageCounter + 1) % _averageFromAmount;
        }

        private void UpdateFPSUI()
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

        private void UpdateAsteroidCountText()
        {
            _asteroidText.text = "Asteroids: " + _asteroidCount.Value;
        }

        public void OnValueChanged(int newValue)
        {
            UpdateAsteroidCountText();
        }
    }
}
