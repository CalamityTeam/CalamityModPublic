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
            Item.width = 24;
            Item.height = 28;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.fishingPole = 60;
            Item.shootSpeed = 18f;
            Item.shoot = ModContent.ProjectileType<EarlyBloomBobber>();
            Item.value = Item.buyPrice(1, 20, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
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
            CreateRecipe(1).AddIngredient(ItemID.WoodFishingPole).AddIngredient(ModContent.ItemType<UeliaceBar>(), 10).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
