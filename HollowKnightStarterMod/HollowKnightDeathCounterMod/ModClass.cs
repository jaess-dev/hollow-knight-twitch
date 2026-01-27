// using UObject = UnityEngine.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GlobalEnums;
using Modding;
using Newtonsoft.Json;
using UnityEngine;

namespace HollowKnightDeathCounterMod
{
    public class HollowKnightDeathCounterMod() : Mod("HkDeathCounter"), ILocalSettings<LocalSettings>
    {
        public override string GetVersion() => "v0.0.1";

        public LocalSettings LocalSettings { get; set; }

        public override void Initialize()
        {
            On.HeroController.Die += (On.HeroController.orig_Die org, HeroController self) =>
            {
                Died();
                return org(self);
            };

            Log($"Initialized {Name}");
        }

        private void Died()
        {
            Log($"Death counter before incrementing {LocalSettings.DeathCounter}");
            LocalSettings.DeathCounter += 1;
            Log($"Death counter increased to {LocalSettings.DeathCounter}");
        }

        public void OnLoadLocal(LocalSettings s) => LocalSettings = s;
        public LocalSettings OnSaveLocal() => LocalSettings;
    }

    public class LocalSettings
    {
        public int DeathCounter { get; set; } = 0;
    }
}