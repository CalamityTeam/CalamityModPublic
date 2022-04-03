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
            Item.width = 24;
            Item.height = 28;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.fishingPole = 75;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<DevourerofCodsBobber>();
            Item.value = Item.buyPrice(1, 80, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
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
            CreateRecipe(1).AddIngredient(ModContent.ItemType<CosmiliteBar>(), 10).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
