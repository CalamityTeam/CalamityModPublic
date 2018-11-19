using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace CalamityMod.Items.Astrageldon
{
	public class AstralJelly : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aureus Cell");
            Tooltip.SetDefault("Gives mana regeneration and magic power for 6 minutes");
        }
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 18;
			item.useTurn = true;
			item.maxStack = 30;
            item.healMana = 200;
            item.useAnimation = 17;
			item.useTime = 17;
            item.rare = 7;
			item.useStyle = 2;
			item.UseSound = SoundID.Item3;
			item.consumable = true;
            item.value = 50000;
            item.buffType = 26;
            item.buffTime = 108000;
        }

        public override bool UseItem(Player player)
        {
            player.AddBuff(BuffID.ManaRegeneration, 21600);
            player.AddBuff(BuffID.MagicPower, 21600);
            return true;
        }
	}
}