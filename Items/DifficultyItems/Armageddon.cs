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
    public class Armageddon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Armageddon");
            Tooltip.SetDefault("Makes any hit while a boss is alive instantly kill you\n" +
                "Effect can be toggled on and off\n" +
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

        public override bool UseItem(Player player)
        {
			if (CalamityPlayer.areThereAnyDamnBosses || CalamityWorld.DoGSecondStageCountdown > 0 || BossRushEvent.BossRushActive)
			{
                string key = "Mods.CalamityMod.ChangingTheRules";
                Color messageColor = Color.Fuchsia;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
				return true;
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

            string key2 = CalamityWorld.armageddon ? "Mods.CalamityMod.ArmageddonText" : "Mods.CalamityMod.ArmageddonText2";
            Color messageColor2 = Color.Fuchsia;
            CalamityUtils.DisplayLocalizedText(key2, messageColor2);

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
