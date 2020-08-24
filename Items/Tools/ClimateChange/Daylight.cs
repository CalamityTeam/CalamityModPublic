using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools.ClimateChange
{
    public class Daylight : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daylight");
            Tooltip.SetDefault("Summons the sun");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 5;
            item.useAnimation = 20;
            item.useTime = 20;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item60;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !Main.dayTime && !CalamityPlayer.areThereAnyDamnBosses;
        }

        public override bool UseItem(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
				Main.dayTime = true;
				CalamityNetcode.SyncWorld();
			}
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SoulofLight, 7);
            recipe.AddIngredient(ItemID.HellstoneBar, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
