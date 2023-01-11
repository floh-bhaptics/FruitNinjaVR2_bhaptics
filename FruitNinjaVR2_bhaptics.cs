using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using UnityEngine;
using Il2Cpp;

[assembly: MelonInfo(typeof(FruitNinjaVR2_bhaptics.FruitNinjaVR2_bhaptics), "FruitNinjaVR2_bhaptics", "1.3.1", "Florian Fahrenberger")]
[assembly: MelonGame("Halfbrick", "Fruit Ninja VR 2")]


namespace FruitNinjaVR2_bhaptics
{
    public class FruitNinjaVR2_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr = null!;
        public static bool bowHandIsRight = false;

        public override void OnInitializeMelon()
        {
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        
        [HarmonyPatch(typeof(Blade), "ProcessCollision", new Type[] { typeof(SliceableObjectBase), typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(bool) })]
        public class bhaptics_BladeCollision
        {
            [HarmonyPostfix]
            public static void Postfix(Blade __instance)
            {
                bool isRightHand = (__instance.Hand.Controller.m_handSide == Il2CppPlatform.ControllerInputBase.HandSide.Right);
                //tactsuitVr.LOG("Blade collision: " + __instance.Hand.name + " " + isRightHand.ToString() + " " + tipMotion.magnitude.ToString());
                tactsuitVr.Recoil("Blade", isRightHand);
            }
        }

        [HarmonyPatch(typeof(Bow), "OnBowGrabbed", new Type[] { typeof(IGrabbable), typeof(Il2CppHands.IHand) })]
        public class bhaptics_BowGrabBow
        {
            [HarmonyPostfix]
            public static void Postfix(Bow __instance, Il2CppHands.IHand hand)
            {
                bowHandIsRight = (hand.Side == Il2CppPlatform.ControllerInputBase.HandSide.Right);
                //tactsuitVr.LOG("Grab bow: " + bowHandIsRight.ToString());
            }
        }


        [HarmonyPatch(typeof(Bow), "FireArrow", new Type[] { typeof(bool) })]
        public class bhaptics_BowFireArrow
        {
            [HarmonyPostfix]
            public static void Postfix(Bow __instance, bool snappedString)
            {
                //tactsuitVr.LOG("FireBow: " + bowHandIsRight.ToString());
                tactsuitVr.Recoil("Bow", !bowHandIsRight);
            }
        }

        [HarmonyPatch(typeof(Bow), "Release", new Type[] { typeof(bool), typeof(bool) })]
        public class bhaptics_BowReleaseString
        {
            [HarmonyPostfix]
            public static void Postfix(Bow __instance)
            {
                //tactsuitVr.LOG("Release: " + bowHandIsRight.ToString());
                tactsuitVr.Recoil("Bow", !bowHandIsRight);
            }
        }


        [HarmonyPatch(typeof(Bomb), "TriggerBombExplosionEvent", new Type[] { typeof(Bomb.ExplosionReason) })]
        public class bhaptics_BombExplode
        {
            [HarmonyPostfix]
            public static void Postfix(Bomb __instance)
            {
                //tactsuitVr.LOG("BombExplode");
                tactsuitVr.PlaybackHaptics("ExplosionBelly");
            }
        }
    }
}
