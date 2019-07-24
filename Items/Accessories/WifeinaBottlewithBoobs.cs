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

namespace CalamityMod.Items.Accessories 
{
	public class WifeinaBottlewithBoobs : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rare Elemental in a Bottle");
			Tooltip.SetDefault("Summons a sand elemental to heal you\n" +
				";D");
		}
		
	    public override void SetDefaults()
	    {
	        item.width = 20;
	        item.height = 26;
            item.value = Item.buyPrice(0, 15, 0, 0);
            item.expert = true;
			item.rare = 9;
			item.accessory = true;
	    }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            if (modPlayer.elementalHeart)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
		{
	    	CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.sandBoobWaifu = true;
			if (player.whoAmI == Main.myPlayer)
			{
				if (player.FindBuffIndex(mod.BuffType("DrewsSandyWaifu")) == -1)
				{
					player.AddBuff(mod.BuffType("DrewsSandyWaifu"), 3600, true);
				}
				if (player.ownedProjectileCounts[mod.ProjectileType("DrewsSandyWaifu")] < 1)
				{
					Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("DrewsSandyWaifu"), (int)(45f * player.minionDamage), 2f, Main.myPlayer, 0f, 0f);
				}
			}
		}
	}
}