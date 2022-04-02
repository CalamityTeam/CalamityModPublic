using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.PermanentBoosters
{
    public class Elderberry : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elderberry");
            Tooltip.SetDefault("A tangy, tart flavor with a somewhat earthen touch\n" +
                               "Permanently increases maximum life by 25\n" +
                               "Can only be used if the max amount of life fruit has been consumed");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useAnimation = 30;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item4;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.eBerry || player.statLifeMax < 500)
            {
                return false;
            }
            return true;
        }

        public override bool UseItem(Player player)
        {
            if (player.itemAnimation > 0 && player.itemTime == 0)
            {
                player.itemTime = item.useTime;
                if (Main.myPlayer == player.whoAmI)
                {
                    player.HealEffect(25);
                }
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.eBerry = true;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LifeFruit, 5);
            recipe.AddIngredient(ItemID.BlueBerries);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 10);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>(), 20);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
