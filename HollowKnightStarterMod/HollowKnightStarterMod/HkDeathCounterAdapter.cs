using System;
using Modding;
using Newtonsoft.Json;

namespace HollowKnightStarterMod
{

    public class HkDeathCounterAdapter(Action<string> log)
    {
        private readonly Action<string> Log = log;

        public int? ReadDeathCounter()
        {
            const string DkModName = "HkDeathCounter";
            try
            {
                if (ModHooks.GetMod(DkModName) is not { } dkMod)
                {
                    Log($"Mod {DkModName} is not loaded");
                    return null;
                }

                // Use reflection to get the SaveData property
                if (dkMod.GetType().GetProperty("LocalSettings") is { } saveDataProperty)
                {
                    var saveData = saveDataProperty.GetValue(dkMod);

                    // Convert to your local copy of the data structure
                    string json = JsonConvert.SerializeObject(saveData);
                    return JsonConvert.DeserializeObject<HkDeathCounterLocalSettings>(json).DeathCounter;
                }

                Log("Get property resulted in a null"); 
            }
            catch (Exception ex)
            {
                Log($"Error reading Mod {DkModName} settings: {ex.Message}");
            }

            return null;
        }
    }

    public class HkDeathCounterLocalSettings
    {
        public int DeathCounter { get; set; } = 0;
    }
}