using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using CalamityMod.NPCs;
namespace CalamityMod.Items.DifficultyItems
{
    public class DefiledRune : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Defiled Rune");
            Tooltip.SetDefault("Wings do nothing and enemies can critically hit you\n" +
                "Increases most rare item drop chances and enemies drop 50% more cash\n" +
                "Using this while a boss is alive will instantly kill you and despawn the boss\n" +
                "Can only be used in revengeance and death mode\n" +
                "Can be toggled on and off");
        }

        public override void SetDefaults()
        {
            item.rare = 11;
            item.width = 28;
            item.height = 28;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = 4;
            item.UseSound = SoundID.Item100;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            if (CalamityWorld.bossRushActive || !CalamityWorld.revenge)
            {
                return false;
            }
            return true;
        }

        public override bool UseItem(Player player)
        {
            for (int doom = 0; doom < 200; doom++)
            {
                if (Main.npc[doom].active && (Main.npc[doom].boss || Main.npc[doom].type == NPCID.EaterofWorldsHead || Main.npc[doom].type == NPCID.EaterofWorldsTail || Main.npc[doom].type == ModContent.NPCType<SlimeGodRun>() ||
                    Main.npc[doom].type == ModContent.NPCType<SlimeGodRunSplit>() || Main.npc[doom].type == ModContent.NPCType<SlimeGod>() || Main.npc[doom].type == ModContent.NPCType<SlimeGodSplit>()))
                {
                    player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " tried to change the rules."), 1000.0, 0, false);
                    Main.npc[doom].active = false;
                    Main.npc[doom].netUpdate = true;
                }
            }
            if (!CalamityWorld.defiled)
            {
                CalamityWorld.defiled = true;
                string key = "Mods.CalamityMod.DefiledText";
                Color messageColor = Color.DarkSeaGreen;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
            }
            else
            {
                CalamityWorld.defiled = false;
                string key = "Mods.CalamityMod.DefiledText2";
                Color messageColor = Color.DarkSeaGreen;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
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
