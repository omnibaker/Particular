using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sumfulla.Atomica
{
    public enum ParticleType
    {
        Proton,
        Neutron,
        Electron
    }

    public class JSONRefs
    {
        public const string JSON_EXT = ".json";
        public const string JSON_FN_ELEMENTS = "elements";
        public const string JSON_FN_ISOTOPES = "isotopes";
        public const string JSON_FN_TOE_POSITION = "toePositions";
    }

    public class UIRef
    {
        public const string STYLESHEET_CORE = "elemental";
    }

    public class PrefRef
    {
        public const string LAST_ATOMIC_NO = "LastAtomicNumber";
    }

    public class AppColor
    {
        // Constants
        public const float LABEL_WHITENING = 0.5f;
        public static readonly Color HYDROGEN = new Color(0.437f, 0.625f, 1f, 1f); // 6F9FFF
        public static readonly Color OTHER_NON_METALS = new Color(0.437f, 0.625f, 1f, 1f); // 6F9FFF
        public static readonly Color HALOGONS = new Color(0.317f, 0.893f, 0.449f, 1f); // 51E473
        public static readonly Color NOBLE_GASES = new Color(0.907f, 0.918f, 0.419f, 1f); //E8EB6B
        public static readonly Color ALKALI_METALS = new Color(0.949f, 0.487f, 0.569f, 1f); // F37D92
        public static readonly Color ALKALINE_EARTH_METALS = new Color(1f, 0.622f, 0.318f, 1f); // FF9F51
        public static readonly Color TRANSITION_METALS = new Color(0.7f, 0.7f, 0.7f, 1f); // B2B2B2
        public static readonly Color POST_TRANSITION_METALS = new Color(0.578f, 0.747f, 0.901f, 1f); // 93BEE6
        public static readonly Color METALOIDS = new Color(0f, 1f, 1f, 1f); // 00FFFF
        public static readonly Color LANTHANIDES = new Color(0.719f, 0.396f, 0.918f, 1f); // B865EB
        public static readonly Color ACTINIDES = new Color(0.931f, 0.547f, 0.931f, 1f); // EE8CEE

        // Static variables
        public static Color HalogonsLight = Color.white;
        public static Color NobleGasesLight = Color.white;
        public static Color AlkaliMetalsLight = Color.white;
        public static Color AlkalineEarthMetalsLight = Color.white;
        public static Color MetaloidsLight = Color.white;
        public static Color OtherNonMetalsLight = Color.white;
        public static Color TransitionMetalsLight = Color.white;
        public static Color PostTransitionMetalsLight = Color.white;
        public static Color LanthanidesLight = Color.white;
        public static Color ActinidesLight = Color.white;
        public static bool LightColorsCreated;
    }

}