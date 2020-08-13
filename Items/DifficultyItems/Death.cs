using CalamityMod.Events;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
namespace CalamityMod.Items.DifficultyItems
{
    public class Death : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Death");
            Tooltip.SetDefault("Makes bosses even more EXTREME.\n" +
                "Allows certain bosses to spawn naturally.\n" +
				"Certain biomes and events have additional weather effects.\n" +
				"Lethal lava effects are always enabled.\n" +
                "Increases enemy damage by 15%.\n" +
                "Greatly boosts enemy spawn rates during the blood moon.\n" +
                "Nerfs the effectiveness of life steal.\n" +
                "Makes the abyss more treachorous to navigate.\n" +
                "Nurse no longer heals while a boss is alive.\n" +
                "Increases damage done by several debuffs.\n" +
                "Effect can be toggled on and off.\n" +
                "Effect will only work if Revengeance Mode is active.\n" +
                "Using this while a boss is alive will instantly kill you and despawn the boss.");
        }

        public override void SetDefaults()
        {
            item.rare = 11;
            item.width = 28;
            item.height = 28;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item119;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            if (!CalamityWorld.revenge || BossRushEvent.BossRushActive)
            {
                return false;
            }
            return true;
        }

        public override bool UseItem(Player player)
        {
            for (int doom = 0; doom < Main.npc.Length; doom++)
            {
                if ((Main.npc[doom].active && (Main.npc[doom].boss || Main.npc[doom].type == NPCID.EaterofWorldsHead || Main.npc[doom].type == NPCID.EaterofWorldsTail || Main.npc[doom].type == ModContent.NPCType<SlimeGodRun>() ||
                    Main.npc[doom].type == ModContent.NPCType<SlimeGodRunSplit>() || Main.npc[doom].type == ModContent.NPCType<SlimeGod>() || Main.npc[doom].type == ModContent.NPCType<SlimeGodSplit>())) || CalamityWorld.DoGSecondStageCountdown > 0)
                {
                    player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " tried to change the rules."), 1000.0, 0, false);
                    Main.npc[doom].active = Main.npc[doom].friendly;
                    Main.npc[doom].netUpdate = true;
                }
            }
            if (!CalamityWorld.death)
            {
                CalamityWorld.death = true;
                string key = "Mods.CalamityMod.DeathText";
                Color messageColor = Color.Crimson;
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
                CalamityWorld.death = false;
                string key = "Mods.CalamityMod.DeathText2";
                Color messageColor = Color.Crimson;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
            }
            CalamityWorld.DoGSecondStageCountdown = 0;

            CalamityNetcode.SyncWorld();

            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.RevengeanceBoolSync);
                netMessage.Write(CalamityWorld.revenge);
                netMessage.Send();
            }
            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.DeathBoolSync);
                netMessage.Write(CalamityWorld.death);
                netMessage.Send();
            }
            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                netMessage.Send();
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
