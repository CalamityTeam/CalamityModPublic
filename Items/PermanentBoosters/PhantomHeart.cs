using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.PermanentBoosters
{
    public class PhantomHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Heart");
            Tooltip.SetDefault("Permanently increases maximum mana by 50");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item29;
            item.consumable = true;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool CanUseItem(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.pHeart)
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
                    player.ManaEffect(50);
                }
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.pHeart = true;
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 100);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
