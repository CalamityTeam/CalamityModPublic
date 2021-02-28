using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DifficultyItems
{
    public class Revenge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Revengeance");
            Tooltip.SetDefault("Enables/disables Revengeance Mode, can only be used in expert mode.\n" +
                "RAGE TOOLTIP LINE HERE\n" +
                "You gain Rage when you take damage or by using special items.\n" +
                "ADRENALINE TOOLTIP LINE HERE\n" +
                "You gain Adrenaline whenever a boss is alive. Getting hit drops Adrenaline back to 0.\n" +
                "All enemies drop 50% more cash and spawn 15% more frequently\n" +
                "Certain enemies and projectiles deal between 5% and 25% more damage.\n" +
                "Makes certain enemies block life steal and nerfs the effectiveness of life steal.\n" +
                "Nerfs the effectiveness of the Titanium Armor set bonus.\n" +
                "Changes ALL boss AIs and some enemy AIs in vanilla and the Calamity Mod.");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.rare = ItemRarityID.Purple;
            item.UseSound = SoundID.Item119;
            item.consumable = false;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            string rageKey = CalamityMod.RageHotKey.TooltipHotkeyString();
            string adrenKey = CalamityMod.AdrenalineHotKey.TooltipHotkeyString();
            foreach (TooltipLine line2 in list)
            {
                if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
                {
                    line2.text = "Enables the Rage mechanic. When Rage is maxed press " + rageKey + " to activate Rage Mode.";
                }
                if (line2.mod == "Terraria" && line2.Name == "Tooltip3")
                {
                    line2.text = "Enables the Adrenaline mechanic. When Adrenaline is maxed press " + adrenKey + " to activate Adrenaline Mode.";
                }
            }
        }

        public override bool CanUseItem(Player player) => Main.expertMode;

        public override bool UseItem(Player player)
        {
            // This world syncing code should only be run by one entity- the server, to prevent a race condition
            // with the packets.
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return true;

            if (CalamityPlayer.areThereAnyDamnBosses || CalamityWorld.DoGSecondStageCountdown > 0 || BossRushEvent.BossRushActive)
            {
                string key = "Mods.CalamityMod.ChangingTheRules";
                Color messageColor = Color.Crimson;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
                return true;
            }
            if (!CalamityWorld.revenge)
            {
                CalamityWorld.revenge = true;
                string key = "Mods.CalamityMod.RevengeText";
                Color messageColor = Color.Crimson;
                CalamityUtils.DisplayLocalizedText(key, messageColor);

                CalamityNetcode.SyncWorld();
            }
            else
            {
                CalamityWorld.revenge = false;
                string key = "Mods.CalamityMod.RevengeText2";
                Color messageColor = Color.Crimson;
                CalamityUtils.DisplayLocalizedText(key, messageColor);

                if (CalamityWorld.death)
                {
                    CalamityWorld.death = false;
                    key = "Mods.CalamityMod.DeathText2";
                    messageColor = Color.Crimson;
                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
                CalamityWorld.DoGSecondStageCountdown = 0;
                CalamityNetcode.SyncWorld();
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
