using UnityEngine;


namespace Sumfulla.Atomica
{
    public class MainController : MonoBehaviour
    {
        [Range(0, 118)]
        [Tooltip("0 uses previous selection (defaults to H)")]
        [SerializeField] private int _startingProtons = 0;
        [SerializeField] private BohrRutherfordModel _brModel;
        [SerializeField] private UIController _ui;


        private void Start()
        {
            InitialiseAtomDisplay();
            StartCoroutine(_ui.Init());
        }

        /// <summary>
        /// Create visual elements of atom model
        /// </summary>
        private void InitialiseAtomDisplay()
        {
            _brModel.Init();
            
            // Display previous selections - unless one is specified in 
            int saved = PlayerPrefs.GetInt(PrefRef.LAST_ATOMIC_NO, 1);
            _brModel.SetStableElement(_startingProtons == 0 ? saved : _startingProtons);
        }


    }
}