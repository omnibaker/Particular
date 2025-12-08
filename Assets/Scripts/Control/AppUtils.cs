using Sumfulla.Atomica;
using UnityEngine;


namespace Sumfulla.Atomica
{
    public class AppUtils
    {
        public static Color GetColor(string typeName)
        {
            switch (typeName)
            {
                case "Hydrogen":
                case "Other Non-Metals":
                    return AppColor.OTHER_NON_METALS;
                //return AppColor.HYDROGEN;
                case "Halogens":
                    return AppColor.HALOGONS;
                case "Nobel Gases":
                    return AppColor.NOBLE_GASES;
                case "Alkali Metals":
                    return AppColor.ALKALI_METALS;
                case "Alkaline Earth Metals":
                    return AppColor.ALKALINE_EARTH_METALS;
                case "Metalloids":
                    return AppColor.METALOIDS;
                case "Transition Metals":
                    return AppColor.TRANSITION_METALS;
                case "Post-Transition Metal":
                    return AppColor.POST_TRANSITION_METALS;
                case "Lanthanides":
                    return AppColor.LANTHANIDES;
                case "Actinides":
                    return AppColor.ACTINIDES;
                default:
                    return Color.red;
            }
        }

        public static Color WhitenColor(Color col)
        {
            return Color.Lerp(col, Color.white, AppColor.LABEL_WHITENING);
        }

        public static void MakeLightColors()
        {
            AppColor.HalogonsLight = WhitenColor(AppColor.HALOGONS);
            AppColor.NobleGasesLight = WhitenColor(AppColor.NOBLE_GASES);
            AppColor.AlkaliMetalsLight = WhitenColor(AppColor.ALKALI_METALS);
            AppColor.AlkalineEarthMetalsLight = WhitenColor(AppColor.ALKALINE_EARTH_METALS);
            AppColor.MetaloidsLight = WhitenColor(AppColor.METALOIDS);
            AppColor.OtherNonMetalsLight = WhitenColor(AppColor.OTHER_NON_METALS);
            AppColor.TransitionMetalsLight = WhitenColor(AppColor.TRANSITION_METALS);
            AppColor.PostTransitionMetalsLight = WhitenColor(AppColor.POST_TRANSITION_METALS);
            AppColor.LanthanidesLight = WhitenColor(AppColor.LANTHANIDES);
            AppColor.ActinidesLight = WhitenColor(AppColor.ACTINIDES);
        }

    }
}