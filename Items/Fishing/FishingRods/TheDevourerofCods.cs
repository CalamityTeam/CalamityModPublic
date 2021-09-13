using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace CalamityMod.Items.Fishing.FishingRods
{
    public class TheDevourerofCods : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Devourer of Cods");
            Tooltip.SetDefault("Fires ten lines at once. Line never snaps and can fish from lava.\n" +
                "The devourer was once just an Eater of Shoals.");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.useAnimation = 8;
            item.useTime = 8;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.fishingPole = 75;
            item.shootSpeed = 20f;
            item.shoot = ModContent.ProjectileType<DevourerofCodsBobber>();
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 10; ++index)
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
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
