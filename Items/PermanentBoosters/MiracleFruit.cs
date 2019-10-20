using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Potions;

namespace CalamityMod.Items.PermanentBoosters
{
    public class MiracleFruit : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Miracle Fruit");
            Tooltip.SetDefault("Refreshing and cool, perhaps even a bit minty\n" +
                               "Permanently increases maximum life by 25\n" +
                               "Can only be used if the max amount of life fruit has been consumed");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useAnimation = 30;
            item.rare = 7;
            item.useTime = 30;
            item.useStyle = 4;
            item.UseSound = SoundID.Item4;
            item.consumable = true;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.mFruit || player.statLifeMax < 500)
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
                modPlayer.mFruit = true;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.LifeFruit, 5);
            recipe.AddIngredient(ModContent.ItemType<AstralJelly>(), 5);
            recipe.AddIngredient(ModContent.ItemType<LivingShard>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
