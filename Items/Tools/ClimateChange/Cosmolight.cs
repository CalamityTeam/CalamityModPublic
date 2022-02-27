using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools.ClimateChange
{
    public class Cosmolight : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmolight");
            Tooltip.SetDefault("Changes night to day and vice versa\n" +
				"Does not work while a boss is alive");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.LightRed;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item60;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !CalamityPlayer.areThereAnyDamnBosses;
        }

        public override bool UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Main.time = 0.0;
                Main.dayTime = !Main.dayTime;
                if (Main.dayTime)
                {
                    if (++Main.moonPhase >= 8)
                    {
                        Main.moonPhase = 0;
                    }
                }
                CalamityNetcode.SyncWorld();
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FallenStar, 10); // Made it slightly more expensive than requested. - Merkalto
			recipe.AddIngredient(ItemID.SoulofLight, 7);
			recipe.AddIngredient(ItemID.SoulofNight, 7);
			recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>(), 5);
			recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
