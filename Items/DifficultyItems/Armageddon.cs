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
    public class Armageddon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Armageddon");
            Tooltip.SetDefault("Makes any hit while a boss is alive instantly kill you\n" +
                "Effect can be toggled on and off\n" +
                "Using this while a boss is alive will instantly kill you and despawn the boss\n" +
                "If a boss is defeated with this effect active it will drop 6 treasure bags, 5 in normal mode");
        }

        public override void SetDefaults()
        {
            item.rare = 11;
            item.width = 28;
            item.height = 28;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item123;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            if (BossRushEvent.BossRushActive)
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
            if (!CalamityWorld.armageddon)
            {
                CalamityWorld.armageddon = true;
            }
            else
            {
                CalamityWorld.armageddon = false;
            }
            CalamityWorld.DoGSecondStageCountdown = 0;

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

            CalamityNetcode.SyncWorld();

            if (Main.netMode == NetmodeID.Server)
            {
                var netMessage = mod.GetPacket();
                netMessage.Write((byte)CalamityModMessageType.ArmageddonBoolSync);
                netMessage.Write(CalamityWorld.armageddon);
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
