using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sumfulla.Atomica
{
    public class Atom
    {
        public List<Subshell> Subshells;
        public string Electroconfig;

        public Shell Shell1;
        public Shell Shell2;
        public Shell Shell3;
        public Shell Shell4;
        public Shell Shell5;
        public Shell Shell6;
        public Shell Shell7;

        private const string NG_HE = "[He] ";
        private const string NG_NE = "[Ne] ";
        private const string NG_AR = "[Ar] ";
        private const string NG_KR = "[Kr] ";
        private const string NG_XN = "[Xn] ";
        private const string NG_RN = "[Rn] ";
        private const string SUPER = "<sup>{0}</sup>";

        public ParticlesCount Particles { get; set; }

        public Atom()
        {
            Subshells = CreateSubshellArray();
        }

        /// <summary>
        /// Region of shell where electrons (grouped by angular momentum) can move around freely. 
        /// </summary>
        public struct Subshell
        {
            public int ShellLevel; // indicates the shell/level in which electron found.
            public Orbital OrbitalType; // indicates subshell and shape of orbital (s, p, d, f)

            public Subshell(int principle, Orbital angular)
            {
                ShellLevel = principle;
                OrbitalType = angular;
            }
        }

        /// <summary>
        /// Angular momentum (l) of orbitals
        /// </summary>
        public enum Orbital
        {
            s = 1,
            p = 3,
            d = 5,
            f = 7
        }

        /// <summary>
        /// AKA energy level - region of space around atomic nucleus where electrons are likely to be found
        /// </summary>
        public struct Shell
        {
            public int AngularCount_S;
            public int AngularCount_P;
            public int AngularCount_D;
            public int AngularCount_F;

            /// <summary>
            /// Incrementer method for public use
            /// </summary>
            public void UpdateCount(Orbital angular)
            {
                switch (angular)
                {
                    case Orbital.s: AngularCount_S++; break;
                    case Orbital.p: AngularCount_P++; break;
                    case Orbital.d: AngularCount_D++; break;
                    case Orbital.f: AngularCount_F++; break;
                }
            }

            /// <summary>
            /// Returns true if any orbital layer has a member
            /// </summary>
            public bool HasElectrons()
            {
                return AngularCount_S != 0 ||
                     AngularCount_P != 0 ||
                     AngularCount_D != 0 ||
                     AngularCount_F != 0;
            }
        }

        /// <summary>
        /// Creates preset instances of all potential subshells
        /// </summary>
        private static List<Subshell> CreateSubshellArray()
        {
            List<Subshell> subshells = new()
            {
                new Subshell(1, Orbital.s), // [He]

                new Subshell(2, Orbital.s),
                new Subshell(2, Orbital.p), // [Ne]

                new Subshell(3, Orbital.s),
                new Subshell(3, Orbital.p), // [Ar]

                new Subshell(4, Orbital.s),
                new Subshell(3, Orbital.d),
                new Subshell(4, Orbital.p), // [Kr]

                new Subshell(5, Orbital.s),
                new Subshell(4, Orbital.d),
                new Subshell(5, Orbital.p), // [Xe]

                new Subshell(6, Orbital.s),
                new Subshell(4, Orbital.f),
                new Subshell(5, Orbital.d),
                new Subshell(6, Orbital.p), // [Rn]

                new Subshell(7, Orbital.s),
                new Subshell(5, Orbital.f),
                new Subshell(6, Orbital.d),
                new Subshell(7, Orbital.p)  // [Og]
            };

            return subshells;
        }

        /// <summary>
        /// Returns string of orbital symbol (s, p, d, f)
        /// </summary>
        public static string GetOrbitalSymbol(Orbital angular)
        {
            return Enum.GetName(typeof(Orbital), angular);
        }

        /// <summary>
        /// Update this energy level's count/L-particle
        /// </summary>
        public void UpdateOrbitalCount(Subshell subshell)
        { 
            // Update this energy level count/L-particle
            switch (subshell.ShellLevel)
            {
                case 1: Shell1.UpdateCount(subshell.OrbitalType); break;
                case 2: Shell2.UpdateCount(subshell.OrbitalType); break;
                case 3: Shell3.UpdateCount(subshell.OrbitalType); break;
                case 4: Shell4.UpdateCount(subshell.OrbitalType); break;
                case 5: Shell5.UpdateCount(subshell.OrbitalType); break;
                case 6: Shell6.UpdateCount(subshell.OrbitalType); break;
                case 7: Shell7.UpdateCount(subshell.OrbitalType); break;
            }
        }

        /// <summary>
        /// Calculates correct distribution of orbital based on the Aufbau Principle
        /// </summary>
        public void GenerateElectronDistribution(int numOfElectrons = 0)
        {
            if (numOfElectrons == 0)
            {
                numOfElectrons = Particles.Electrons;
            }

            Shell1 = new();
            Shell2 = new();
            Shell3 = new();
            Shell4 = new();
            Shell5 = new();
            Shell6 = new();
            Shell7 = new();

            string electroconfig = "";
            int electronCount = 0;
            for (int ss = 0; ss < Subshells.Count; ss++)
            {
                // Updates electro-config prefix at certain intervals (deleting previous entries)
                switch (ss)
                {
                    case 1: electroconfig = NG_HE; break;
                    case 3: electroconfig = NG_NE; break;
                    case 5: electroconfig = NG_AR; break;
                    case 8: electroconfig = NG_KR; break;
                    case 11: electroconfig = NG_XN; break;
                    case 15: electroconfig = NG_RN; break;
                }

                // Update shell level
                electroconfig += Subshells[ss].ShellLevel;

                // Update orbital symbol
                electroconfig += GetOrbitalSymbol(Subshells[ss].OrbitalType);

                // Each subshell has +/- slot so multiply by 2
                int electronLimit = (int)Subshells[ss].OrbitalType * 2;

                // Iterate through all +/1 spin-magnetic quantum numbers (smqn)
                for (int smqn = 0; smqn < electronLimit; smqn++)
                {
                    // Update electron count for this shell
                    UpdateOrbitalCount(Subshells[ss]);

                    // Update electons tally
                    electronCount++;

                    // Update string with orbital count (break if last available electron)
                    if (electronCount == numOfElectrons)
                    {
                        electroconfig += string.Format(SUPER, smqn + 1);
                        break;
                    }
                    else if (smqn == electronLimit - 1)
                    {
                        electroconfig += string.Format(SUPER, smqn + 1);
                    }
                }

                if (electronCount == numOfElectrons)
                {
                    break;
                }
            }

            Electroconfig = electroconfig;
        }

    }

}
