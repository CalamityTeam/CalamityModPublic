using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs;

namespace CalamityMod.Buffs.Amidias
{
	public class AmidiasBlessing : ModBuff
	{
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Amidias' Blessing");
            Description.SetDefault("You are blessed by Amidias\n"
								  +"Lets you breathe underwater, even in the Abyss!\n"
								  +"Just don't get hit...");
            Main.debuff[Type] = false;
			Main.buffNoSave[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
		{
            player.GetModPlayer<CalamityPlayer>(mod).amidiasBlessing = true;
            player.breath = player.breathMax + 91;
        }
	}
}