using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod.NPCs;

namespace CalamityMod.Items.Accessories
{
	public class Revenge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Revengeance");
			Tooltip.SetDefault("READ ALL OF THIS\n" +
			    "Activates revengeance mode, can only be used in expert mode.\n" +
                "Activates rage, which is a new stat.  When rage is maxed press V to activate rage mode.\n" +
                "You gain rage whenever you take damage.\n" +
                "Activates adrenaline, which is a new stat.  When adrenaline is maxed press B to activate adrenaline mode.\n" +
                "You gain adrenaline whenever a boss is alive.  Getting hit drops adrenaline back to 0.\n" +
                "If you hit max adrenaline and don't use it within 5 seconds your adrenaline damage will drop gradually.\n" +
				"All enemies drop twice the cash and enemy spawn rates are boosted.\n" +
				"Before you have killed your first boss you take 20% LESS damage from everything.\n" +
                "Changes the Expert Mode 75% defense back to the Normal Mode 50% defense for the duration of prehardmode.\n" +
                "Changes ALL boss AIs in vanilla and the Calamity Mod.\n" +
				"DO NOT USE IF A BOSS IS ALIVE!\n" +
				"Only use if you are absolutely sure you want to go through with this!");
		}
		
		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 28;
			item.expert = true;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.UseSound = SoundID.Item119;
			item.consumable = true;
		}
		
		public override bool CanUseItem(Player player)
		{
			if (!Main.expertMode || CalamityWorld.bossRushActive)
			{
				return false;
			}
			return true;
		}

		public override bool UseItem(Player player)
		{
			for (int doom = 0; doom < 200; doom++)
			{
				if (Main.npc[doom].active && Main.npc[doom].boss)
				{
					player.KillMe(PlayerDeathReason.ByOther(12), 1000.0, 0, false);
					Main.npc[doom].active = false;
                    Main.npc[doom].netUpdate = true;
                }
			}
			if (!CalamityWorld.revenge)
			{
				CalamityWorld.revenge = true;
                if (Main.netMode == 2)
                {
                    NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
                }
            }
			return true;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}