using System;
using System.Collections.Generic;
using Terraria;

namespace CalamityMod.CalPlayer.Dashes
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public static class PlayerDashManager
    {
        internal static Dictionary<string, PlayerDashEffect> DashIdentificationTable = new();

        public static bool FindByID(string id, out PlayerDashEffect dashEffect)
        {
            return DashIdentificationTable.TryGetValue(id, out dashEffect);
        }

        // A new function that allows to add in new modded dashes for Calamity to recognize, it attempts
        // to make it as safe as possible.


        /// <summary>
        /// Attempts to add a dash effect into the dash manager. It should be executed in a Load() function.
        /// </summary>
        /// <param name="dashEffect">An instance of the desired dash effect to add.</param>

        public static void TryAddDash(PlayerDashEffect dashEffect)
        {
            //If DashIdentificationTable is not loaded or modder tries to load an abstract type of
            //PlayerDashEffect, then don't add the dash and stop the method.
            if (DashIdentificationTable == null || dashEffect.GetType().IsAbstract) return;

            string id = (string)dashEffect.GetType().GetProperty("ID").GetValue(null);

            //This chunk of the code only executes if the dash isn't already added and
            //the dash id isn't empty.
            if (!DashIdentificationTable.ContainsKey(id) && !String.IsNullOrEmpty(id))
            {
                DashIdentificationTable[id] = dashEffect;
            }

        }

        internal static void Load()
        {
            DashIdentificationTable = new();
            Type baseType = typeof(PlayerDashEffect);
            foreach (Type type in CalamityMod.Instance.Code.GetTypes())
            {
                // Ignore any types which are not dash effects or are abstract.
                // This eliminates the PlayerDashEffect template type, which cannot have instances.
                if (!type.IsSubclassOf(baseType) || type.IsAbstract)
                    continue;

                // Use reflection to get the static ID manually. This shouldn't be a performance problem, as this only happens at load-time.
                string id = (string)type.GetProperty("ID").GetValue(null);

                PlayerDashEffect dashEffect = (PlayerDashEffect)Activator.CreateInstance(type);
                DashIdentificationTable[id] = dashEffect;
            }
        }


        internal static void Unload()
        {
            DashIdentificationTable = null;
        }
    }
}
