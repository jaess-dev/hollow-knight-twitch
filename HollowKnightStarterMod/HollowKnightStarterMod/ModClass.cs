// using UObject = UnityEngine.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HK.Domain;
using Modding;
using UnityEngine;

namespace HollowKnightStarterMod;

public class HollowKnightStarterMod : Mod
{
    private ICommunication _connector;
    public HollowKnightStarterMod() : base("HkEventDistributer")
    {
    }

    public override string GetVersion() => "v0.0.2";

    static HollowKnightStarterMod()
    {
        AppDomain.CurrentDomain.AssemblyResolve += ResolveEmbeddedAssembly;
    }

    public override void Initialize()
    {

        _connector = new ServerConnector(new(), LogError);
        // On.HeroController.AddGeo += OnAddGeo;
        On.HeroController.Die += (On.HeroController.orig_Die org, HeroController self) =>
        {
            var res = org(self);
            OnDeath(org, self);
            return res;
        };

        // On.HeroController.Respawn

        ModHooks.SetPlayerIntHook += (string fieldName, int value) =>
        {
            switch (fieldName)
            {
                case Constants.GRUBS_COLLECTED:
                    break;
            }

            return value;
        };
    }

    private void OnDeath(On.HeroController.orig_Die org, HeroController self)
    {
        _ = _connector.SendYouDiedAsync();
    }

    private void OnAddGeo(On.HeroController.orig_AddGeo orig, HeroController self, int amount)
    {
        orig(self, amount);
        if (amount <= 0)
            return;

        _ = _connector.SendGeoEventAsync(amount, self.playerData.geo);
    }

    private static Assembly? ResolveEmbeddedAssembly(object? sender, ResolveEventArgs args)
    {
        string resourceName = Assembly.GetExecutingAssembly()
            .GetManifestResourceNames()
            .FirstOrDefault(r => r.EndsWith("HK.Domain.dll"));

        if (resourceName == null)
            return null;

        using Stream stream =
            Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!;

        byte[] buffer = new byte[stream.Length];
        stream.Read(buffer, 0, buffer.Length);

        return Assembly.Load(buffer);
    }
}