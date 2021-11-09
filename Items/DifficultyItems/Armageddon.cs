using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DifficultyItems
{
    public class Armageddon : ModItem
    {
        private static readonly Color textColor = Color.Fuchsia;
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Armageddon");
            Tooltip.SetDefault("Enables/disables Armageddon, can be used in any other difficulty mode\n" +
                "While active, any hit while a boss is alive will instantly kill you\n" +
                "If you defeat a boss for the first time with this mode active, they will drop 5 extra treasure bags\n" +
                "These extra bags will drop even if you are not in Expert Mode\n" +
                "Right click with this item to toggle whether your dodges are disabled\n" +
                "Dodges can be disabled independently of whether or not Armageddon is not enabled");
        }

        public override void SetDefaults()
        {
            item.rare = ItemRarityID.Purple;
            item.width = 28;
            item.height = 28;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item123;
            item.consumable = false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool UseItem(Player player)
        {
            // Dodge toggling code is per-player and doesn't need any fancy netcode hedging.
            if (player.altFunctionUse == 2)
            {
                // You still aren't allowed to toggle it during fights though.
                if (CalamityPlayer.areThereAnyDamnBosses || CalamityWorld.DoGSecondStageCountdown > 0 || BossRushEvent.BossRushActive)
                {
                    string key = "Mods.CalamityMod.ChangingTheRules";
                    CalamityUtils.DisplayLocalizedText(key, textColor);
                    return true;
                }

                CalamityPlayer mp = player.Calamity();
                mp.disableAllDodges = !mp.disableAllDodges;
                string dodgeKey = mp.disableAllDodges ? "Mods.CalamityMod.ArmageddonDodgeDisable" : "Mods.CalamityMod.ArmageddonDodgeEnable";
                CalamityUtils.DisplayLocalizedText(dodgeKey, textColor);
                return true;
            }
            
            // This world syncing code should only be run by one entity- the server, to prevent a race condition
            // with the packets.
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return true;

            if (CalamityPlayer.areThereAnyDamnBosses || CalamityWorld.DoGSecondStageCountdown > 0 || BossRushEvent.BossRushActive)
            {
                string key = "Mods.CalamityMod.ChangingTheRules";
                CalamityUtils.DisplayLocalizedText(key, textColor);
                return true;
            }
            CalamityWorld.armageddon = !CalamityWorld.armageddon;
            CalamityWorld.DoGSecondStageCountdown = 0;

            string toggleKey = CalamityWorld.armageddon ? "Mods.CalamityMod.ArmageddonText" : "Mods.CalamityMod.ArmageddonText2";
            CalamityUtils.DisplayLocalizedText(toggleKey, textColor);

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
