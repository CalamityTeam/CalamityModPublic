using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Revenge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Revengeance");
            Tooltip.SetDefault("Activates Revengeance Mode, can only be used in expert mode.\n" +
                "Activates rage. When rage is maxed press V to activate rage mode.\n" +
                "You gain rage whenever you take damage or hit an enemy with a true melee weapon.\n" +
                "Activates adrenaline. When adrenaline is maxed press B to activate adrenaline mode.\n" +
                "You gain adrenaline whenever a boss is alive. Getting hit drops adrenaline back to 0.\n" +
                "If you hit max adrenaline and don't use it within 3 seconds your adrenaline damage will drop gradually.\n" +
                "All enemies drop 50% more cash.\n" +
                "All enemies gain a new damage reduction stat, which can be reduced by applying various debuffs to them.\n" +
                "The amount of this damage reduction stat varies based on enemy type.\n" +
                "Makes certain enemies immune to life steal and nerfs the effectiveness of life steal.\n" +
                "Increases enemy damage by 25% and spawn rates by 15%.\n" +
                "Nerfs the effectiveness of the Titanium Armor set bonus.\n" +
                "Makes life regen scale with your current HP, the higher your HP the lower your life regen (this is not based on max HP).\n" +
                "Reduces maximum asphalt run speed by 33%.\n" +
                "Increases Nurse healing price.\n" +
                "Allows certain enemies to inflict the Horror and Marked debuffs.\n" +
                "Before you have killed your first boss you take 20% less damage from everything.\n" +
                "Changes the Expert Mode 75% defense back to the Normal Mode 50% defense for the duration of prehardmode.\n" +
                "Changes ALL boss AIs and some enemy AIs in vanilla and the Calamity Mod.\n" +
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
                if (Main.npc[doom].active && (Main.npc[doom].boss || Main.npc[doom].type == NPCID.EaterofWorldsHead || Main.npc[doom].type == NPCID.EaterofWorldsTail || Main.npc[doom].type == ModContent.NPCType<SlimeGodRun>() ||
                    Main.npc[doom].type == ModContent.NPCType<SlimeGodRunSplit>() || Main.npc[doom].type == ModContent.NPCType<SlimeGod>() || Main.npc[doom].type == ModContent.NPCType<SlimeGodSplit>()))
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
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
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
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }

                if (CalamityWorld.death)
                {
                    CalamityWorld.death = false;
                    key = "Mods.CalamityMod.DeathText2";
                    messageColor = Color.Crimson;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
                if (CalamityWorld.defiled)
                {
                    CalamityWorld.defiled = false;
                    key = "Mods.CalamityMod.DefiledText2";
                    messageColor = Color.DarkSeaGreen;
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
