using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.Items.DifficultyItems
{
    public class Armageddon : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Armageddon");
			Tooltip.SetDefault("Makes any hit while a boss is alive instantly kill you\n" +
                "Effect can be toggled on and off\n" +
                "Using this while a boss is alive will instantly kill you and despawn the boss\n" +
                "If a boss is defeated with this effect active it will drop 6 treasure bags, 5 in normal mode\n" +
                "If any player dies while a boss is alive the boss will instantly despawn");
		}

		public override void SetDefaults()
		{
            item.rare = 11;
			item.width = 28;
			item.height = 28;
			item.useAnimation = 45;
			item.useTime = 45;
			item.useStyle = 4;
			item.UseSound = SoundID.Item123;
			item.consumable = false;
		}

        public override bool CanUseItem(Player player)
        {
            if (CalamityWorld.bossRushActive)
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
            if (!CalamityWorld.armageddon)
            {
                CalamityWorld.armageddon = true;
            }
            else
            {
                CalamityWorld.armageddon = false;
            }
            string key = CalamityWorld.armageddon ? "Mods.CalamityMod.ArmageddonText" : "Mods.CalamityMod.ArmageddonText2";
            Color messageColor = Color.Fuchsia;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue(key), messageColor);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
            }

            CalamityMod.UpdateServerBoolean();
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
