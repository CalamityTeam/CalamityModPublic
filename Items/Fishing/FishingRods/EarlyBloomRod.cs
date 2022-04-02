using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
namespace CalamityMod.Items.Fishing.FishingRods
{
    public class EarlyBloomRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Early Bloom Rod");
            Tooltip.SetDefault("Fires six lines at once. Line never snaps.\n" +
                "The early bird catches the fish.");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.useAnimation = 8;
            item.useTime = 8;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.fishingPole = 60;
            item.shootSpeed = 18f;
            item.shoot = ModContent.ProjectileType<EarlyBloomBobber>();
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 6; ++index)
            {
                float SpeedX = speedX + Main.rand.NextFloat(-3.75f, 3.75f);
                float SpeedY = speedY + Main.rand.NextFloat(-3.75f, 3.75f);
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, 0, 0f, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.WoodFishingPole); //wood -> fossilized wood
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
