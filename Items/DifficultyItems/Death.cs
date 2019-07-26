using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod.NPCs;
using CalamityMod.World;

namespace CalamityMod.Items.DifficultyItems
{
	public class Death : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Death");
			Tooltip.SetDefault("Makes bosses even more EXTREME\n" +
                "Effect can be toggled on and off\n" +
                "Effect will only work if revengeance mode is active\n" +
                "Using this while a boss is alive will instantly kill you and despawn the boss");
		}

		public override void SetDefaults()
		{
            item.rare = 11;
			item.width = 28;
			item.height = 28;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.UseSound = SoundID.Item119;
			item.consumable = false;
		}

        public override bool CanUseItem(Player player)
        {
            if (!CalamityWorld.revenge || CalamityWorld.bossRushActive)
            {
                return false;
            }
            return true;
        }

        public override bool UseItem(Player player)
		{
            for (int doom = 0; doom < 200; doom++)
            {
                if (Main.npc[doom].active && (Main.npc[doom].boss || Main.npc[doom].type == NPCID.EaterofWorldsHead || Main.npc[doom].type == NPCID.EaterofWorldsTail || Main.npc[doom].type == mod.NPCType("SlimeGodRun") ||
					Main.npc[doom].type == mod.NPCType("SlimeGodRunSplit") || Main.npc[doom].type == mod.NPCType("SlimeGod") || Main.npc[doom].type == mod.NPCType("SlimeGodSplit")))
                {
                    player.KillMe(PlayerDeathReason.ByOther(12), 1000.0, 0, false);
                    Main.npc[doom].active = false;
                    Main.npc[doom].netUpdate = true;
                }
            }
            if (!CalamityWorld.death)
            {
                CalamityWorld.death = true;
                string key = "Mods.CalamityMod.DeathText";
                Color messageColor = Color.Crimson;
                if (Main.netMode == 0)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == 2)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
            }
            else
            {
                CalamityWorld.death = false;
                string key = "Mods.CalamityMod.DeathText2";
                Color messageColor = Color.Crimson;
                if (Main.netMode == 0)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == 2)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
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
