using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.DifficultyItems
{
	public class IronHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iron Heart");
            Tooltip.SetDefault("Healing with potions and all positive life regen is disabled.\n" +
				"Enemy damage scales with your max health.\n" +
                "Can be toggled on and off.");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.expert = true;
            item.rare = ItemRarityID.Cyan;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item119;
            item.consumable = false;
        }

        public override bool UseItem(Player player)
        {
            // This world syncing code should only be run by one entity- the server, to prevent a race condition
            // with the packets.
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return true;

            if (CalamityPlayer.areThereAnyDamnBosses || CalamityWorld.DoGSecondStageCountdown > 0 || BossRushEvent.BossRushActive)
			{
                string key = "Mods.CalamityMod.ChangingTheRules";
                Color messageColor = Color.LightSkyBlue;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
				return true;
			}
			if (!CalamityWorld.ironHeart)
            {
                CalamityWorld.ironHeart = true;
                string key = "Mods.CalamityMod.IronHeartText";
                Color messageColor = Color.LightSkyBlue;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }
            else
            {
                CalamityWorld.ironHeart = false;
                string key = "Mods.CalamityMod.IronHeartText2";
                Color messageColor = Color.LightSkyBlue;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }
            CalamityWorld.DoGSecondStageCountdown = 0;
            CalamityNetcode.SyncWorld();

            return true;
        }
    }
}
