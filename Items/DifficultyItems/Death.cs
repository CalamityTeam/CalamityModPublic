using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
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
                "Enemies and bosses buffed by Revengeance Mode are even stronger in this mode.\n" +
                "Greatly boosts enemy spawn rates during the blood moon.\n" +
                "Nerfs the effectiveness of life steal.\n" +
                "Makes the abyss more treacherous to navigate.\n" +
                "Nurse no longer heals while a boss is alive.\n" +
                "Increases damage done by 50% for several debuffs and all alcohols that reduce life regen.\n" +
                "Effect can be toggled on and off.\n" +
                "Effect will only work if Revengeance Mode is active.");
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Purple;
            item.width = 28;
            item.height = 28;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item119;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player) => CalamityWorld.revenge;

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
            if (!CalamityWorld.death)
            {
                CalamityWorld.death = true;
                string key = "Mods.CalamityMod.DeathText";
                Color messageColor = Color.Crimson;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }
            else
            {
                CalamityWorld.death = false;
                string key = "Mods.CalamityMod.DeathText2";
                Color messageColor = Color.Crimson;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }
            CalamityWorld.DoGSecondStageCountdown = 0;
            CalamityNetcode.SyncWorld();

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
