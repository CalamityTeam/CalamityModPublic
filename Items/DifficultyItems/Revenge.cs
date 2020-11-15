using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
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
                "You gain rage whenever you take damage or hit an enemy with a true melee weapon.\n" +
                "ADRENALINE TOOLTIP LINE HERE\n" +
                "You gain adrenaline whenever a boss is alive. Getting hit drops adrenaline back to 0.\n" +
                "All enemies drop 50% more cash and spawn 15% more frequently\n" +
                "Certain enemies and projectiles deal between 5% and 25% more damage.\n" +
                "Makes certain enemies immune to life steal and nerfs the effectiveness of life steal.\n" +
                "Nerfs the effectiveness of the Titanium Armor set bonus.\n" +
                "Makes life regen scale with your current HP, the higher your HP the lower your life regen (this is not based on max HP).\n" +
                "Asphalt run speed is reduced by 33%, and the Nurse's healing cost is increased\n" +
                "Before you have killed your first boss you take 20% less damage from everything.\n" +
                "Changes ALL boss AIs and some enemy AIs in vanilla and the Calamity Mod.");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.rare = 11;
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
                    line2.text = "Activates rage. When rage is maxed press " + rageKey + " to activate rage mode.";
                }
                if (line2.mod == "Terraria" && line2.Name == "Tooltip3")
                {
                    line2.text = "Activates adrenaline. When adrenaline is maxed press " + adrenKey + " to activate adrenaline mode.";
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
                if (CalamityWorld.defiled)
                {
                    CalamityWorld.defiled = false;
                    key = "Mods.CalamityMod.DefiledText2";
                    messageColor = Color.DarkSeaGreen;
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
