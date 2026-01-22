// using UObject = UnityEngine.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Modding;
using UnityEngine;

namespace HollowKnightStarterMod;

public class HollowKnightStarterMod : Mod
{
    private readonly BotConnector _botConnector;
    public HollowKnightStarterMod() : base("HkEventDistributer")
    {
        _botConnector = new BotConnector(new(), LogError);
    }

    public override string GetVersion() => "v0.0.2";

    public override void Initialize()
    {
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
        _ = _botConnector.SendYouDiedAsync();
    }

    private void OnAddGeo(On.HeroController.orig_AddGeo orig, HeroController self, int amount)
    {
        orig(self, amount);
        if (amount <= 0)
            return;

        _ = _botConnector.SendGeoEventAsync(amount, self.playerData.geo);
    }

}