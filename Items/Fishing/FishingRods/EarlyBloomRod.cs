using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.FishingRods
{
    public class EarlyBloomRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
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
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int index = 0; index < 6; ++index)
            {
                float SpeedX = velocity.X + Main.rand.NextFloat(-3.75f, 3.75f);
                float SpeedY = velocity.Y + Main.rand.NextFloat(-3.75f, 3.75f);
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, 0, 0f, player.whoAmI);
            }
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WoodFishingPole).
                AddIngredient<UelibloomBar>(10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
