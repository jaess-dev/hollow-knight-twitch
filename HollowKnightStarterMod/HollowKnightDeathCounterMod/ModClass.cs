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
using HK.Domain;
using HollowKnightDeathCounterMod.Domain.Model;
using Modding;
using Newtonsoft.Json;
using UnityEngine;

namespace HollowKnightDeathCounterMod;

public class HollowKnightDeathCounterMod : Mod
{
    private ICommunication _connector;
    public HollowKnightDeathCounterMod() : base("HkDeathCounter")
    {
    }

    public override string GetVersion() => "v0.0.1";


    public override void Initialize()
    {
        On.HeroController.Die += (On.HeroController.orig_Die org, HeroController self) =>
        {
            var res = org(self);
            OnDeath(self);
            return res;
        };
    }
}