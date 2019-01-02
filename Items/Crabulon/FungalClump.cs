using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.GameContent;
using Terraria.IO;
using Terraria.ObjectData;
using Terraria.Utilities;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Crabulon 
{
	public class FungalClump : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fungal Clump");
			Tooltip.SetDefault("Summons a fungal clump to fight for you\n" +
	                   "The clump latches onto enemies and steals their life for you");
		}
		
	    public override void SetDefaults()
	    {
	        item.width = 20;
	        item.height = 26;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.expert = true;
	        item.accessory = true;
	    }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            if (modPlayer.fungalClump)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
		{
	    	CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.fungalClump = true;
			if (player.whoAmI == Main.myPlayer)
			{
				if (player.FindBuffIndex(mod.BuffType("FungalClump")) == -1)
				{
					player.AddBuff(mod.BuffType("FungalClump"), 3600, true);
				}
				if (player.ownedProjectileCounts[mod.ProjectileType("FungalClump")] < 1)
				{
					Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("FungalClump"), 10, 1f, Main.myPlayer, 0f, 0f);
				}
			}
		}
	}
}