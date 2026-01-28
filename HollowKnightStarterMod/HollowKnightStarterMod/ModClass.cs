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
using HollowKnightStarterMod.Domain.Model;
using Modding;
using Newtonsoft.Json;
using UnityEngine;

namespace HollowKnightStarterMod;

public class HollowKnightStarterMod : Mod
{
    private readonly HkDeathCounterAdapter _deathCounter;
    private ICommunication _connector;

    public HollowKnightStarterMod() : base("HkEventDistributer")
    {
        _deathCounter = new HkDeathCounterAdapter(Log);
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

        // On.HeroController.Respawn += (org, self) =>
        // {
        //     var res = org(self);
        //     OnRespawn(self);
        //     return res;
        // };

        On.HeroController.DieFromHazard += (org, self, hazardType, angle) =>
        {
            var res = org(self, hazardType, angle);
            OnDieFromHazard(self, hazardType, angle);
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

    /// <summary>
    /// Event gets fired every time we are killed by something and need to be respawned.
    /// For example when we fall onto spikes.
    /// </summary>
    private void OnDieFromHazard(HeroController self, HazardType hazardType, float angle)
    {
        SendOffCatching(() => _connector.SendDiedFromHazardAsync(new HazardDeathDto(hazardType switch
        {
            HazardType.NON_HAZARD => HazardTypeDto.NON_HAZARD,
            HazardType.ACID => HazardTypeDto.ACID,
            HazardType.LAVA => HazardTypeDto.LAVA,
            HazardType.PIT => HazardTypeDto.PIT,
            HazardType.SPIKES => HazardTypeDto.SPIKES,
            _ => throw new ArgumentException($"Unknown enum {hazardType}"),
        })));
    }

    private void OnRespawn(HeroController self)
    {
        int? dc = _deathCounter.ReadDeathCounter();

        Log(dc is not null
            ? $"Got the death counter of {dc}"
            : $"Could not fetch the death counter!");

        SendOffCatching(() => _connector.SendRespawnAsync(new PlayerDataDto(
            self.playerData.geo,
            self.playerData.grubsCollected,
            dc
        )));
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