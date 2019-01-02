using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs
{
	public class CrushDepth : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Crush Depth");
			Description.SetDefault("Aquatic pressure");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
			longerExpertDebuff = false;
		}

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<CalamityPlayer>(mod).cDepth = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
		{
			npc.GetGlobalNPC<CalamityGlobalNPC>(mod).cDepth = true;
		}
	}
}