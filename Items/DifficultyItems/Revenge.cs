using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items.DifficultyItems
{
    public class Revenge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Revengeance");
			Tooltip.SetDefault("Activates revengeance mode, can only be used in expert mode.\n" +
                "Activates rage. When rage is maxed press V to activate rage mode.\n" +
                "You gain rage whenever you take damage.\n" +
                "Activates adrenaline. When adrenaline is maxed press B to activate adrenaline mode.\n" +
                "You gain adrenaline whenever a boss is alive. Getting hit drops adrenaline back to 0.\n" +
                "If you hit max adrenaline and don't use it within 3 seconds your adrenaline damage will drop gradually.\n" +
				"All enemies drop 50% more cash and enemy spawn rates are boosted.\n" +
				"Before you have killed your first boss you take 20% LESS damage from everything.\n" +
                "Changes the Expert Mode 75% defense back to the Normal Mode 50% defense for the duration of prehardmode.\n" +
                "Changes ALL boss AIs in vanilla and the Calamity Mod.\n" +
				"DO NOT USE IF A BOSS IS ALIVE!\n" +
				"Can be toggled on and off.");
		}

		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 28;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
            item.rare = 11;
			item.UseSound = SoundID.Item119;
			item.consumable = false;
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
				if (Main.npc[doom].active && (Main.npc[doom].boss || Main.npc[doom].type == NPCID.EaterofWorldsHead || Main.npc[doom].type == NPCID.EaterofWorldsTail || Main.npc[doom].type == mod.NPCType("SlimeGodRun") ||
					Main.npc[doom].type == mod.NPCType("SlimeGodRunSplit") || Main.npc[doom].type == mod.NPCType("SlimeGod") || Main.npc[doom].type == mod.NPCType("SlimeGodSplit")))
				{
					player.KillMe(PlayerDeathReason.ByOther(12), 1000.0, 0, false);
					Main.npc[doom].active = false;
                    Main.npc[doom].netUpdate = true;
                }
			}
			if (!CalamityWorld.revenge)
			{
				CalamityWorld.revenge = true;
				string key = "Mods.CalamityMod.RevengeText";
				Color messageColor = Color.Crimson;
				if (Main.netMode == 0)
				{
					Main.NewText(Language.GetTextValue(key), messageColor);
				}
				else if (Main.netMode == 2)
				{
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}

                CalamityMod.UpdateServerBoolean();
            }
			else
			{
				CalamityWorld.revenge = false;
				string key = "Mods.CalamityMod.RevengeText2";
				Color messageColor = Color.Crimson;
				if (Main.netMode == 0)
				{
					Main.NewText(Language.GetTextValue(key), messageColor);
				}
				else if (Main.netMode == 2)
				{
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}

				if (CalamityWorld.death)
				{
					CalamityWorld.death = false;
					key = "Mods.CalamityMod.DeathText2";
					messageColor = Color.Crimson;
					if (Main.netMode == 0)
					{
						Main.NewText(Language.GetTextValue(key), messageColor);
					}
					else if (Main.netMode == 2)
					{
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
					}
				}
				if (CalamityWorld.defiled)
				{
					CalamityWorld.defiled = false;
					key = "Mods.CalamityMod.DefiledText2";
					messageColor = Color.DarkSeaGreen;
					if (Main.netMode == 0)
					{
						Main.NewText(Language.GetTextValue(key), messageColor);
					}
					else if (Main.netMode == 2)
					{
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
					}
				}

                CalamityMod.UpdateServerBoolean();
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
