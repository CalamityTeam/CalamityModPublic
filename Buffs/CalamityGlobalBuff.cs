using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class CalamityGlobalBuff : GlobalBuff
	{
        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (type == BuffID.Shine)
            {
                player.GetModPlayer<CalamityPlayer>(mod).shine = true;
            }
            if (type == BuffID.IceBarrier)
            {
                player.endurance -= 0.1f;
            }
        }
    }
}
