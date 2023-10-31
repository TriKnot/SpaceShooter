using ScriptableObjects.Variables;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

namespace UI
{
    public class UIButtonHandler : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private Toggle _useJobsToggle;
        [SerializeField] private Toggle _usePoolingToggle;
        [SerializeField] private Toggle _usePhysicsToggle;
        [SerializeField] private TMP_Dropdown _vSyncToggle;
        [SerializeField] private Slider _spawnRateSlider;
        [SerializeField] private TextMeshProUGUI _spawnRateText;
        [SerializeField] private Slider _initialAsteroidCountSlider;
        [SerializeField] private TextMeshProUGUI _initialAsteroidCountText;
        
        [Header("Scene References")]
        [SerializeField] private BoolVariableSO _useJobsSO;
        [SerializeField] private BoolVariableSO _usePoolingSO;
        [SerializeField] private BoolVariableSO _usePhysicsSO;
        [SerializeField] private IntVariableSO _vSyncSO;
        [SerializeField] private FloatVariableSO _spawnRateSO;
        [SerializeField] private IntVariableSO _initialAsteroidCountSO;
        
        private void Awake()
        {
            // Set the UI to the current settings
            // Bool
            SetUseJobs(_useJobsSO.Value);
            _useJobsToggle.isOn = _useJobsSO.Value;
            SetUsePooling(_usePoolingSO.Value);
            _usePoolingToggle.isOn = _usePoolingSO.Value;
            SetUsePhysics(_usePhysicsSO.Value);
            _usePhysicsToggle.isOn = _usePhysicsSO.Value;
            // Int
            SetVSync(_vSyncSO.Value);
            _vSyncToggle.value = _vSyncSO.Value;
            SetInitialAsteroidCount(_initialAsteroidCountSO.Value);
            _initialAsteroidCountSlider.value = _initialAsteroidCountSO.Value;
            // Float
            SetSpawnRate(_spawnRateSO.Value);
             _spawnRateSlider.value = _spawnRateSO.Value;
        }
        
        public void StartGame()
        {
            SceneManager.LoadScene(1);
        }
        
        public void SetVSync(int value)
        {
            // 0 = off, 1 = on, 2 = half
            value = Mathf.Clamp(value, 0, 2);
            QualitySettings.vSyncCount = value;
            _vSyncSO.Value = value;
        }
        
        public void SetUseJobs(bool value)
        {
            _useJobsSO.Value = value;
        }
        
        public void SetUsePooling(bool value)
        {
            _usePoolingSO.Value = value;
        }
        
        public void SetUsePhysics(bool value)
        {
            _usePhysicsSO.Value = value;
        }

        public void SetSpawnRate(float value)
        {
            _spawnRateSO.Value = value;
            _spawnRateText.text = value.ToString("F3");
            if(value == 0)
                _spawnRateText.text = "Off";
        }
        
        public void SetInitialAsteroidCount(float value)
        {
            value = Mathf.FloorToInt(value);
            _initialAsteroidCountSO.Value = (int)value;
            _initialAsteroidCountText.text = value.ToString("F0");
        }

        public void QuitGame()
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
