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
using HK.Domain;
using HollowKnightStarterMod.Domain.Model;
using Modding;
using Newtonsoft.Json;
using UnityEngine;

namespace HollowKnightStarterMod;

public class HollowKnightStarterMod : Mod
{
    private ICommunication _connector;
    public HollowKnightStarterMod() : base("HkEventDistributer")
    {
    }

    public override string GetVersion() => "v0.0.3";


    public override void Initialize()
    {
        _connector = new ServerConnector(new(), LogError);
        // On.HeroController.AddGeo += OnAddGeo;
        On.HeroController.Die += (On.HeroController.orig_Die org, HeroController self) =>
        {
            var res = org(self);
            OnDeath(self);
            return res;
        };

        On.HeroController.Respawn += (org, self) =>
        {
            var res = org(self);
            OnRespawn(self);
            Log("This was a respawn");
            return res;
        };

        On.HeroController.DieFromHazard += (org, self, hazardType, angle) =>
        {
            var res = org(self, hazardType, angle);
            Log($"Died from hazard {hazardType} with float: {angle}");
            return res;
        };

        ModHooks.SetPlayerIntHook += (string fieldName, int value) =>
        {
            switch (fieldName)
            {
                case Constants.GRUBS_COLLECTED:
                    OnGrubSaved(value);
                    break;
            }

            return value;
        };
    }

    private void OnRespawn(HeroController self)
    {
        SendOffCatching(() => _connector.SendRespawnAsync(ToDto(self.playerData)));
    }

    private PlayerDataDto ToDto(PlayerData playerData)
    {
        return new PlayerDataDto(
            playerData.geo,
            playerData.grubsCollected
        );
    }

    private async void OnGrubSaved(int grubCount)
    {
        SendOffCatching(() => _connector.SendGrubSavedAsync(grubCount));
    }

    private void OnDeath(HeroController _)
    {
        SendOffCatching(_connector.SendYouDiedAsync);
    }

    /// <summary>
    /// Takes an async function, awaits it, and catches any thrown exception while logging it.
    /// </summary>
    /// <param name="fun">The async function to await and catch errors from.</param>
    private async void SendOffCatching(Func<Task> fun)
    {
        try
        {
            await fun();
        }
        catch (Exception ex)
        {
            LogError($"Exception thrown during send off {ex}");
        }
    }

    private Dictionary<string, object> ToDictionary(object obj)
    {
        return JsonConvert.DeserializeObject<Dictionary<string, object>>(
            JsonConvert.SerializeObject(obj)
        );
    }
}