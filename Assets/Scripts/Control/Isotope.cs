using System.Collections;
using System.Collections.Generic;

public class Isotope
{
    public string IsotopeName;
    public string Symbol;
    public bool IsStable;
    public int Protons;
    public int Neutrons;
    public float RelativeAtomicMass;
    public float Composition;

    public Isotope(string isotopeName, string symbol, bool isStable, int protons, int neutrons, float relativeAtomicMass, float composition)
    {
        IsotopeName = isotopeName;
        Symbol = symbol;
        IsStable = isStable;
        Protons = protons;
        Neutrons = neutrons;
        RelativeAtomicMass = relativeAtomicMass;
        Composition = composition;
    }
}