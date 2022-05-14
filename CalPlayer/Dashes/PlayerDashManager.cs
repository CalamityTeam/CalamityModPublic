using System;
using System.Collections.Generic;

namespace CalamityMod.CalPlayer.Dashes
{
    public static class PlayerDashManager
    {
        internal static Dictionary<string, PlayerDashEffect> DashIdentificationTable = new();

        public static bool FindByID(string id, out PlayerDashEffect dashEffect)
        {
            return DashIdentificationTable.TryGetValue(id, out dashEffect);
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
