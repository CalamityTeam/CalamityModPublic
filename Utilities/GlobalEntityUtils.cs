using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace CalamityMod
{
    public static partial class CalamityUtils
    {
        /// <summary>
        /// Actual "Thread-Safe" version of TryGetGlobalNPC
        /// </summary>
        /// <typeparam name="T">GlobalNPC type to get</typeparam>
        /// <param name="npc">Input NPC</param>
        /// <param name="globalNPC">GlobalNPC output</param>
        /// <returns>true If we successfully get GlobalNPC. otherwise false</returns>
        public static bool TryGetGlobalNPCSafer<T>(this NPC npc, out T globalNPC) where T : GlobalNPC
        {
            try
            {
                if (npc is null)
                {
                    globalNPC = null;
                    return false;
                }

                int slot = ModContent.GetInstance<T>()?.PerEntityIndex ?? -1;
                int length = npc.EntityGlobals.Length;
                if (slot < 0 || slot >= length)
                {
                    globalNPC = null;
                    return false;
                }
                npc.TryGetGlobalNPC(out globalNPC);
                return globalNPC != null;
            }
            catch
            {
                globalNPC = null;
                return false;
            }
        }
    }
}
