using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using HarmonyLib;
using MyBhapticsTactsuit;
using UnityEngine;

namespace FruitNinjaVR2_bhaptics
{
    public class FruitNinjaVR2_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;
        public static bool bowHandIsRight = false;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }

        
        [HarmonyPatch(typeof(Blade), "ProcessCollision", new Type[] { typeof(SliceableObjectBase), typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(bool) })]
        public class bhaptics_BladeCollision
        {
            [HarmonyPostfix]
            public static void Postfix(Blade __instance)
            {
                bool isRightHand = (__instance.Hand.m_controller.m_handSide == Platform.ControllerInputBase.HandSide.Right);
                //tactsuitVr.LOG("Blade collision: " + __instance.Hand.name + " " + isRightHand.ToString() + " " + tipMotion.magnitude.ToString());
                tactsuitVr.Recoil("Blade", isRightHand);
            }
        }

        [HarmonyPatch(typeof(Bow), "OnBowGrabbed", new Type[] { typeof(Hands.Hand) })]
        public class bhaptics_BowGrabBow
        {
            [HarmonyPostfix]
            public static void Postfix(Bow __instance, Hands.Hand hand)
            {
                bowHandIsRight = (hand.Side == Platform.ControllerInputBase.HandSide.Right);
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
