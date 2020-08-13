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
    public class DefiledRune : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Defiled Rune");
            Tooltip.SetDefault("Wing flight is disabled and enemies can critically hit you\n" +
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
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item100;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            if (BossRushEvent.BossRushActive || !CalamityWorld.revenge)
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
            CalamityWorld.DoGSecondStageCountdown = 0;

            CalamityNetcode.SyncWorld();

            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.DefiledBoolSync);
                netMessage.Write(CalamityWorld.defiled);
                netMessage.Send();
            }
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
