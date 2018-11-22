using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.BrimstoneWaifu
{
    public class RoseStone : ModItem
    {
    	public override void SetStaticDefaults()
	 	{
	 		DisplayName.SetDefault("Rose Stone");
	 		Tooltip.SetDefault("One of the ancient relics\n" +
            	"Increases max life by 20, life regen by 2, and all damage and crit chance by 2%\n" +
            	"Summons a brimstone elemental to protect you");
	 	}
    	
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = 500000;
            item.rare = 5;
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
        	Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, 0.6f, 0f, 0.25f);
			player.lifeRegen += 2;
			player.statLifeMax2 += 20;
			player.meleeCrit += 2;
			player.meleeDamage += 0.02f;
			player.magicCrit += 2;
			player.magicDamage += 0.02f;
			player.rangedCrit += 2;
			player.rangedDamage += 0.02f;
			player.thrownCrit += 2;
			player.thrownDamage += 0.02f;
			player.minionDamage += 0.02f;
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.brimstoneWaifu = true;
			if (player.whoAmI == Main.myPlayer)
			{
				if (player.FindBuffIndex(mod.BuffType("BrimstoneWaifu")) == -1)
				{
					player.AddBuff(mod.BuffType("BrimstoneWaifu"), 3600, true);
				}
				if (player.ownedProjectileCounts[mod.ProjectileType("BigBustyRose")] < 1)
				{
					Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("BigBustyRose"), 45, 2f, Main.myPlayer, 0f, 0f);
				}
			}
		}
    }
}
