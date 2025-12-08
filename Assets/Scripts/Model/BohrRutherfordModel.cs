using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UIElements;

namespace Sumfulla.Atomica
{
    public class BohrRutherfordModel : MonoBehaviour
    {
        private const int NUCLEUS_LIMIT_1 = 2;
        private const int NUCLEUS_LIMIT_2 = 10;
        private const int NUCLEUS_LIMIT_3 = 30;
        private const int NUCLEUS_LIMIT_4 = 58;

        [SerializeField] private NucleusLayer _nucleusLevel1 = default;
        [SerializeField] private NucleusLayer _nucleusLevel2 = default;
        [SerializeField] private NucleusLayer _nucleusLevel3 = default;
        [SerializeField] private NucleusLayer _nucleusLevel4 = default;
        [SerializeField] private NucleusLayer _nucleusLevel5 = default;
        [SerializeField] private GameObject _energyLevel1 = default;
        [SerializeField] private GameObject _energyLevel2 = default;
        [SerializeField] private GameObject _energyLevel3 = default;
        [SerializeField] private GameObject _energyLevel4 = default;
        [SerializeField] private GameObject _energyLevel5 = default;
        [SerializeField] private GameObject _energyLevel6 = default;
        [SerializeField] private GameObject _energyLevel7 = default;
        public Atom CurrentAtom { get; set; }
        public JSONData.JSONElement CurrentElement { get; set; }
        public JSONData Data { get; set; }

        public static Action<JSONData.JSONElement> UpdateElement;
        public static Action<string> UpdateIsotopeName;
        public static Action<string> UpdateElec;
        public static Action<int, int, int> UpdateParticleValues;
        public static Action<string> UpdateElecConfig;
        public static Action<string, string> UpdateSymbol;
        public static Action<string> UpdateMessenger;

        private CameraZoomer _camZoomer;
        void OnEnable()
        {
            UIController.SelectElement += SetStableElement;
        }
        void OnDisable()
        {
            UIController.SelectElement -= SetStableElement;
        }

        /// <summary>
        /// Sets up visible BR model by running data request and configuring initial selection
        /// </summary>
        public void Init()
        {
            _camZoomer = FindAnyObjectByType<CameraZoomer>();
            CurrentAtom = new Atom();
            InitialiseAtomData();
            AssignAdjustmentButtonBehaviour();
        }

        /// <summary>
        /// Collects all data from local JSON file(s)
        /// </summary>
        private void InitialiseAtomData()
        {
            Data = new JSONData();
            Data.GetData();
            Data.CreateIsotopesList();
            Data.AddIsotopesToElements();
        }

        /// <summary>
        /// Defines element by its protons with equally balanced electrons and most abundant neutron set
        /// </summary>
        public void SetStableElement(int protons)
        {
            int electrons = protons;
            JSONData.JSONElement element = JSONData.JsonElements.FirstOrDefault(el => el.AtomicNo == protons);
            int neutrons = (int)(element?.GetDefaultNeutronCount());

            RestructureAtom(protons, neutrons, electrons);
        }

        /// <summary>
        /// General method for responding to +/- UI buttons to update protons, neutrons, and electron
        /// </summary>
        public void Increment(ParticleType pt, bool addMore)
        {
            int val = addMore ? 1 : -1;

            switch (pt)
            {
                case ParticleType.Proton: IncrementBR_Protons(CurrentAtom.Particles.Protons + val); break;
                case ParticleType.Neutron: IncrementBR_Neutrons(CurrentAtom.Particles.Neutrons + val); break;
                case ParticleType.Electron: IncrementBR_Electrons(CurrentAtom.Particles.Electrons + val); break;
            }
        }

        /// <summary>
        /// Updates BR display based on particle parameters - proton updates will switch elements
        /// </summary>
        public void RestructureAtom(int protons, int neutrons, int electrons)
        {
            // If proton value has changes, we update the element entirely
            int initProton = CurrentAtom.Particles.Protons;
            if (initProton != protons)
            {
                CurrentElement = JSONData.JsonElements.FirstOrDefault(el => el.AtomicNo == protons);
                PlayerPrefs.SetInt(PrefRef.LAST_ATOMIC_NO, protons);
                UpdateElement?.Invoke(CurrentElement);

                CurrentAtom.Particles = new ParticlesCount(protons, CurrentElement.GetDefaultNeutronCount(), protons);
            }
            else
            {
                CurrentAtom.Particles = new ParticlesCount(protons, neutrons, electrons);
            }

            // Don't want to have to keep writing this out...
            ParticlesCount p = CurrentAtom.Particles;

            // Update particle systems and period rings
            ArrangeNucleus(p.Protons, p.Neutrons);
            StartCoroutine(ArrangeElectrons(p.Electrons));

            // Update UI labels
            UpdateElecConfig?.Invoke(CurrentAtom.Electroconfig);
            UpdateParticleValues?.Invoke(p.Protons, p.Neutrons, p.Electrons);
            //string ec = p.Protons == p.Electrons ? "" : (p.Protons - p.Electrons).ToString();
            string ec = GetIonicChargeString(p.Protons - p.Electrons);
            UpdateSymbol?.Invoke(CurrentElement.Symbol, ec);
        }

        /// <summary>
        /// Takes difference of protons minus electrons and returns number followed by +/- operator
        /// </summary>
        public string GetIonicChargeString(int diff)
        {
            if(diff == 0)
            {
                return "";
            }
            else if(diff > 1)
            {
                return $"{diff}+";
            }
            else
            {
                return $"{Mathf.Abs(diff)}-";
            }
        }

        /// <summary>
        /// Updates particle systems of proton/neutorn nucleus
        /// </summary>
        public void ArrangeNucleus(int protons, int neutrons)
        {
            UpdateNucleusParticles(ParticleType.Proton, protons);
            UpdateNucleusParticles(ParticleType.Neutron, neutrons);
        }

        /// <summary>
        /// Updates particle systems of surrounding electron particles
        /// </summary>
        public IEnumerator ArrangeElectrons(int electrons)
        {
            CurrentAtom.GenerateElectronDistribution(electrons);

            yield return new WaitForEndOfFrame();

            List<bool> values = new List<bool>
            {
                // Update atom model with finished data
                ActivateShell(_energyLevel1, CurrentAtom.Shell1),
                ActivateShell(_energyLevel2, CurrentAtom.Shell2),
                ActivateShell(_energyLevel3, CurrentAtom.Shell3),
                ActivateShell(_energyLevel4, CurrentAtom.Shell4),
                ActivateShell(_energyLevel5, CurrentAtom.Shell5),
                ActivateShell(_energyLevel6, CurrentAtom.Shell6),
                ActivateShell(_energyLevel7, CurrentAtom.Shell7)
            };

            int shell = ToBitmask(values);
            if (_camZoomer != null)
            {
                _camZoomer.DoReposition(shell);
            }
        }

        /// <summary>
        /// Turns list of booleans describing shell activations into workable bitmask value 
        /// </summary>
        private int ToBitmask(List<bool> values)
        {
            int mask = 0;
            int bit = 1;

            foreach (bool v in values)
            {
                if (v)
                {
                    mask |= bit;
                }

                bit <<= 1;
            }

            return mask;
        }

        /// <summary>
        /// Adds electron particles to a specific period shell/ring
        /// </summary>
        private bool ActivateShell(GameObject energyLevel, Atom.Shell elc)
        {
            bool active = false;
            if (elc.HasElectrons())
            {
                active = true;
                energyLevel.SetActive(active);
                energyLevel.GetComponent<EnergyLevel>().UpdateElectrons(elc.AngularCount_S, elc.AngularCount_P, elc.AngularCount_D, elc.AngularCount_F);
            }
            else
            {
                energyLevel.SetActive(active);
            }

            return active;
        }

        /// <summary>
        /// Updates RB model neutron/proton quantity in nucleus
        /// </summary>
        private void UpdateNucleusParticles(ParticleType particleType, int particleCount)
        {
            int[] disableLimits = new int[] { 0, NUCLEUS_LIMIT_1, NUCLEUS_LIMIT_2, NUCLEUS_LIMIT_3, NUCLEUS_LIMIT_4 };
            NucleusLayer[] nucleusLayers = { _nucleusLevel1, _nucleusLevel2, _nucleusLevel3, _nucleusLevel4, _nucleusLevel5 };

            if (particleCount > NUCLEUS_LIMIT_4)
            {
                _nucleusLevel5.ActivateSystem(particleType, true);
                _nucleusLevel5.UpdateNucleusLevel(particleType, particleCount - NUCLEUS_LIMIT_4);
                _nucleusLevel4.ActivateSystem(particleType, true);
                _nucleusLevel4.UpdateNucleusLevel(particleType, NUCLEUS_LIMIT_4 - NUCLEUS_LIMIT_3);
                _nucleusLevel4.ActivateSystem(particleType, true);
                _nucleusLevel3.UpdateNucleusLevel(particleType, NUCLEUS_LIMIT_3 - NUCLEUS_LIMIT_2);
                _nucleusLevel2.ActivateSystem(particleType, true);
                _nucleusLevel2.UpdateNucleusLevel(particleType, NUCLEUS_LIMIT_2 - NUCLEUS_LIMIT_1);
                _nucleusLevel1.ActivateSystem(particleType, true);
                _nucleusLevel1.UpdateNucleusLevel(particleType, NUCLEUS_LIMIT_1);
            }
            else if (particleCount > NUCLEUS_LIMIT_3)
            {
                _nucleusLevel5.ActivateSystem(particleType, false);
                _nucleusLevel4.ActivateSystem(particleType, true);
                _nucleusLevel4.UpdateNucleusLevel(particleType, particleCount - NUCLEUS_LIMIT_3);
                _nucleusLevel3.ActivateSystem(particleType, true);
                _nucleusLevel3.UpdateNucleusLevel(particleType, NUCLEUS_LIMIT_3 - NUCLEUS_LIMIT_2);

                _nucleusLevel2.ActivateSystem(particleType, true);
                _nucleusLevel2.UpdateNucleusLevel(particleType, NUCLEUS_LIMIT_2 - NUCLEUS_LIMIT_1);
                _nucleusLevel1.ActivateSystem(particleType, true);
                _nucleusLevel1.UpdateNucleusLevel(particleType, NUCLEUS_LIMIT_1);
            }
            else if (particleCount > NUCLEUS_LIMIT_2)
            {
                _nucleusLevel5.ActivateSystem(particleType, false);
                _nucleusLevel4.ActivateSystem(particleType, false);
                _nucleusLevel3.ActivateSystem(particleType, true);
                _nucleusLevel3.UpdateNucleusLevel(particleType, particleCount - NUCLEUS_LIMIT_2);

                _nucleusLevel2.ActivateSystem(particleType, true);
                _nucleusLevel2.UpdateNucleusLevel(particleType, NUCLEUS_LIMIT_2 - NUCLEUS_LIMIT_1);
                _nucleusLevel1.ActivateSystem(particleType, true);
                _nucleusLevel1.UpdateNucleusLevel(particleType, NUCLEUS_LIMIT_1);
            }
            else if (particleCount > NUCLEUS_LIMIT_1)
            {
                _nucleusLevel5.ActivateSystem(particleType, false);
                _nucleusLevel4.ActivateSystem(particleType, false);
                _nucleusLevel3.ActivateSystem(particleType, false);

                _nucleusLevel2.ActivateSystem(particleType, true);
                _nucleusLevel2.UpdateNucleusLevel(particleType, particleCount - NUCLEUS_LIMIT_1);
                _nucleusLevel1.ActivateSystem(particleType, true);
                _nucleusLevel1.UpdateNucleusLevel(particleType, NUCLEUS_LIMIT_1);
            }
            else
            {
                _nucleusLevel5.ActivateSystem(particleType, false);
                _nucleusLevel4.ActivateSystem(particleType, false);
                _nucleusLevel3.ActivateSystem(particleType, false);

                _nucleusLevel2.ActivateSystem(particleType, false);
                _nucleusLevel1.ActivateSystem(particleType, true);
                _nucleusLevel1.UpdateNucleusLevel(particleType, particleCount);
            }
        }

        /// <summary>
        /// Sets up UITK elements and behaviours
        /// </summary>
        private void AssignAdjustmentButtonBehaviour()
        {
            UIController uic = FindAnyObjectByType<UIController>();
            if (uic != null)
            {
                uic.AdjustProton.OnIncrease += () => Increment(ParticleType.Proton, true);
                uic.AdjustProton.OnDecrease += () => Increment(ParticleType.Proton, false);
                uic.AdjustNeutron.OnIncrease += () => Increment(ParticleType.Neutron, true);
                uic.AdjustNeutron.OnDecrease += () => Increment(ParticleType.Neutron, false);
                uic.AdjustElectron.OnIncrease += () => Increment(ParticleType.Electron, true);
                uic.AdjustElectron.OnDecrease += () => Increment(ParticleType.Electron, false);
            }
        }

        /// <summary>
        /// Checks and behaviour when proton value is incremented in UI
        /// </summary>
        public void IncrementBR_Protons(int p)
        {

            if (p <= 0)
            {
                string warningMsg = "Cannot implement model with 0 protons";
                UpdateMessenger?.Invoke(warningMsg);
                Debug.LogWarning(warningMsg);
            }
            else if (p >= JSONData.JsonElements.Count)
            {
                string warningMsg = "No elements with more than " + JSONData.JsonElements.Count + " protons available";
                UpdateMessenger?.Invoke(warningMsg);
                Debug.LogWarning(warningMsg);
            }
            else
            {
                RestructureAtom(p, CurrentAtom.Particles.Neutrons, CurrentAtom.Particles.Electrons);
            }
        }

        /// <summary>
        /// Checks and behaviour when neutron value is incremented in UI
        /// </summary>
        public void IncrementBR_Neutrons(int n)
        {
            JSONData.JSONElement element = JSONData.JsonElements[CurrentAtom.Particles.Protons - 1];
            int lowest = element.Isotopes[0].Neutrons;
            int highest = element.Isotopes[element.Isotopes.Count - 1].Neutrons;

            if (n < lowest)
            {
                string warningMsg = element.Name + " cannot have less than " + lowest + " neutrons";
                UpdateMessenger?.Invoke(warningMsg);
                Debug.LogWarning(warningMsg);
            }
            else if (n > highest)
            {
                string warningMsg = element.Name + " cannot have more than " + highest + " neutrons";
                UpdateMessenger?.Invoke(warningMsg);
                Debug.LogWarning(warningMsg);
            }
            else
            {
                UpdateIsotopeName?.Invoke(CurrentElement.GetIsotopeName(n));
                RestructureAtom(CurrentAtom.Particles.Protons, n, CurrentAtom.Particles.Electrons);
            }
        }

        /// <summary>
        /// Checks and behaviour when electron value is incremented in UI
        /// </summary>
        public void IncrementBR_Electrons(int e)
        {
            if (e < 1)
            {
                string warningMsg = "Element must have at least 1 electron";
                UpdateMessenger?.Invoke(warningMsg);
                Debug.LogWarning(warningMsg);
            }
            else if (e >= JSONData.JsonElements.Count + 16)
            {
                string warningMsg = "Cannot implement model with more than " + JSONData.JsonElements.Count + " electrons";
                UpdateMessenger?.Invoke(warningMsg);
                Debug.LogWarning(warningMsg);
            }
            else
            {
                RestructureAtom(CurrentAtom.Particles.Protons, CurrentAtom.Particles.Neutrons, e);
            }
        }
    }


    /// <summary>
    /// Struct that holds basic particle information of an atom instance
    /// </summary>
    public struct ParticlesCount
    {
        public int Protons;
        public int Neutrons;
        public int Electrons;

        public ParticlesCount(int p, int n, int e)
        {
            Protons = p;
            Neutrons = n;
            Electrons = e;
        }
    }
}