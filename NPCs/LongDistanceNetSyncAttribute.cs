using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    /// <summary>
    /// This attribute allows ModNPC to always sync for their position and rotation data at least every 45 frames
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class LongDistanceNetSyncAttribute : Attribute
    {
        /// <summary>
        /// Syncs this NPC to other NPC's sync frame
        /// <para>If this is not present, We cannot properly sync full NPC bodyparts in same frame! (This is important for Worm-type NPCs)</para>
        /// </summary>
        public Type SyncWith { get; set; } = null;

        public LongDistanceNetSyncAttribute()
        {

        }
    }
}
