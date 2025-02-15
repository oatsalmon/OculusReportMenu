using BepInEx;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OculusReportMenu {
    [BepInPlugin("org.oatsalmon.gorillatag.oculusreportmenu", "OculusReportMenu", "1.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        static bool ModEnabled = true;
        public static bool Menu = false;
        static float debounceTime = 0.0f;

        public void Update()
        {
            if (!ModEnabled) return;
            if (Menu)
            {
                // hide the fact that they're in report menu to prevent comp cheating
                GorillaLocomotion.Player.Instance.disableMovement = false;
                GorillaLocomotion.Player.Instance.inOverlay = false;
            }

            if (ControllerInputPoller.instance.leftControllerSecondaryButton || Keyboard.current.rightAltKey.wasPressedThisFrame)
            {
                if (!Menu && Time.time >= debounceTime) // stop it from opening the menu a whole ton
                {
                    debounceTime = Time.time + 1; // for some reason, just opening it too fast after another thing makes it black out your game until you restart
                                                  // this should hopefully fix that issue

                    GorillaMetaReport gr = GameObject.Find("Miscellaneous Scripts").transform.Find("MetaReporting").GetComponent<GorillaMetaReport>();
                    gr.gameObject.SetActive(true);
                    Menu = true;
                    gr.enabled = true;
                    MethodInfo inf = typeof(GorillaMetaReport).GetMethod("StartOverlay", BindingFlags.NonPublic | BindingFlags.Instance);
                    inf.Invoke(gr, null);
                }
            }
        }

        public void OnEnable() {
            HarmonyPatches.ApplyHarmonyPatches();
            ModEnabled = true;
        }
        
        public void OnDisable() {
            HarmonyPatches.RemoveHarmonyPatches();
            ModEnabled = false;
        }
    }

    [HarmonyPatch(typeof(GorillaMetaReport), "Teardown")] // GorillaMetaReport.Teardown() is called when X is pressed
    public class CheckMenuClosed
    {
        static void Postfix()
        {
            Plugin.Menu = false;
        }
    }
}