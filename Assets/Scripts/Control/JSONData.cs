using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Sumfulla.Atomica
{
    public class JSONData
    {
        private readonly bool _outputData = false;

        [Serializable]
        public class JSONImport
        {
            public List<JSONTablePosition> TOE_Positions = new List<JSONTablePosition>();
            public List<JSONElement> Elements = new List<JSONElement>();
            public List<JSONIsotope> Isotopes = new List<JSONIsotope>();
        }

        [Serializable]
        public class JSONTablePosition
        {
            public string Row;
            public string Column;
            public string Symbol;
            public string Number;
        }

        [Serializable]
        public class JSONElement
        {
            public int AtomicNo;
            public float AtomicMass;
            public string Name;
            public string Symbol;
            public string TypeName;
            public string TypeGroupRef;
            public float ElectroNeg;
            public int Period;
            public string Description;
            public string DiscoveredBy;

            public int MostAbundantIsotope;
            public float MostAbundancy;
            public List<Isotope> Isotopes;
            public Dictionary<int, string> IsotopeNames = new Dictionary<int, string>();

            /// <summary>
            /// Updates isotope data on element to be persistent throughout app
            /// </summary>
            public void CalculateIsotopeData()
            {
                if (Isotopes.Count <= 0)
                {
                    Debug.LogWarning($"No isotopes available for this element ({Name})");
                }
                else
                {
                    Isotope mostAbundant = null;
                    float comp = 0;

                    foreach (Isotope isotope in Isotopes)
                    {
                        if (isotope.Composition > comp)
                        {
                            comp = isotope.Composition;
                            mostAbundant = isotope;
                        }
                        string iName = string.IsNullOrWhiteSpace(isotope.IsotopeName) ? Name : isotope.IsotopeName;
                        //Debug.Log($"'{Name}' => {isotope.Neutrons} | {iName}");
                        IsotopeNames.Add(isotope.Neutrons, iName);
                    }

                    if (mostAbundant != null)
                    {
                        MostAbundantIsotope = mostAbundant.Neutrons;
                        MostAbundancy = mostAbundant.Composition * 100f;
                    }
                    else
                    {
                        if (Isotopes.Count > 0)
                        {
                            //Debug.LogWarning("ERR: Most abundant isotope unregistered: " + Isotopes[0].Symbol);
                        }
                        MostAbundancy = 0;
                    }

                    Isotopes = Isotopes.OrderBy(i => i.Neutrons).ToList();

                    // Create a default null option (native name for element)
                    if (!IsotopeNames.TryGetValue(0, out string zeroName))
                    {
                        IsotopeNames.Add(0, Name);
                    }
                }
            }

            /// <summary>
            /// Decides which neutron value start with when element type has protons updated
            /// </summary>
            public int GetDefaultNeutronCount()
            {
                int defNuetron = 0;
                if (MostAbundantIsotope != 0)
                {
                    return MostAbundantIsotope;
                }
                else if (Isotopes != null && Isotopes[0].Neutrons != 0)
                {
                    return Isotopes[0].Neutrons;
                }

                return defNuetron;
            }

            /// <summary>
            /// Returns a string of Isotype name if it has a different name that its native state value
            /// </summary>
            public string GetIsotopeName(int neutronCount)
            {
                if (IsotopeNames.TryGetValue(neutronCount, out string value))
                {
                    return value;
                }

                return Name;
            }
        }

        [Serializable]
        public class JSONIsotope
        {
            public string IsotopeName;
            public string Symbol;
            public bool IsStable;
            public int Protons;
            public int Neutrons;
            public float RelativeAtomicMass;
            public float Composition;
        }

        public static List<JSONElement> JsonElements { get; set; } = new List<JSONElement>();
        public static List<JSONIsotope> JsonIsotopes = new List<JSONIsotope>();
        public static List<JSONTablePosition> JsonTablePositions = new List<JSONTablePosition>();
        public static List<Isotope> Isotopes = new List<Isotope>();

        /// <summary>
        /// Runs when application starts
        /// </summary>
        public void GetData()
        {
            ReadData(JSONRefs.JSON_FN_ELEMENTS);
            ReadData(JSONRefs.JSON_FN_ISOTOPES);
            ReadData(JSONRefs.JSON_FN_TOE_POSITION);
        }

        /// <summary>
        /// Reads JSON data from input file
        /// </summary>
        /// <param name="filename">Name of JSON file</param>
        private void ReadData(string filename)
        {
            // Define filepath based on input parameter
            string filepath = Path.Combine(Application.streamingAssetsPath, filename + JSONRefs.JSON_EXT);

            try
            {
                // Check if file exists
                if (File.Exists(filepath))
                {
                    // Get content from file
                    string contents = File.ReadAllText(filepath);
                    JSONImport jsonImport = JsonUtility.FromJson<JSONImport>(contents);
                    switch (filename)
                    {
                        case JSONRefs.JSON_FN_ELEMENTS:
                            JsonElements = jsonImport.Elements;
                            break;
                        case JSONRefs.JSON_FN_ISOTOPES:
                            JsonIsotopes = jsonImport.Isotopes;
                            break;
                        case JSONRefs.JSON_FN_TOE_POSITION:
                            JsonTablePositions = jsonImport.TOE_Positions;
                            break;
                    }
                    if (_outputData)
                    {
                        PrintOutJson(filename);
                    }
                }
                else
                {
                    Debug.Log($"Unable to read JSON data for [{filename}], file does not exists");
                }
            }
            catch (Exception ex)
            {
                // Return error message
                Debug.Log(ex.Message);
            }
        }

        /// <summary>
        /// Printout confirmation of retrieved JSON data from input file
        /// </summary>
        private void PrintOutJson(string filename)
        {
            switch (filename)
            {
                case JSONRefs.JSON_FN_ELEMENTS:
                    if (JsonElements != null)
                    {
                        foreach (JSONElement element in JsonElements)
                        {
                            string output = string.Format("{0}\t{1}\t{2}", element.AtomicNo, element.Symbol, element.Name);
                            Debug.Log(output);
                        }
                    }
                    else
                    {
                        Debug.Log("JsonElements is NULL");
                    }
                    break;
                case JSONRefs.JSON_FN_ISOTOPES:
                    if (JsonIsotopes != null)
                    {
                        foreach (JSONIsotope isotopes in JsonIsotopes)
                        {
                            string output = string.Format("{0}\t{1}\t{2}\t{3}\t{4}", isotopes.Symbol, isotopes.Protons, isotopes.Neutrons, isotopes.IsStable, isotopes.IsotopeName);
                            Debug.Log(output);
                        }
                    }
                    else
                    {
                        Debug.Log("JsonIsotopes is NULL");
                    }
                    break;
                case JSONRefs.JSON_FN_TOE_POSITION:
                    if (JsonTablePositions != null)
                    {
                        foreach (JSONTablePosition position in JsonTablePositions)
                        {
                            string output = string.Format("{0}\t{1}\t{2}\t{3}", position.Row, position.Column, position.Symbol, position.Number);
                            Debug.Log(output);
                        }
                    }
                    else
                    {
                        Debug.Log("JsonTablePositions is NULL");
                    }
                    break;
            }
        }

        /// <summary>
        /// Populates list of potential isotopes
        /// </summary>
        public void CreateIsotopesList()
        {
            foreach (JSONIsotope jsonIsotope in JsonIsotopes)
            {
                Isotope isotope = new Isotope(jsonIsotope.IsotopeName, jsonIsotope.Symbol, jsonIsotope.IsStable, jsonIsotope.Protons, jsonIsotope.Neutrons, jsonIsotope.RelativeAtomicMass, jsonIsotope.Composition);
                Isotopes.Add(isotope);
            }
        }

        /// <summary>
        /// Updates element list with isotope variants
        /// </summary>
        public void AddIsotopesToElements()
        {
            foreach (JSONElement element in JsonElements)
            {
                element.Isotopes = Isotopes.Where(e => e.Protons == element.AtomicNo).ToList();
                element.CalculateIsotopeData();
            }
        }

    }
}