using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Terraria.DataStructures;

namespace CalamityMod.Items.Fishing.FishingRods
{
    public class TheDevourerofCods : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            ItemID.Sets.CanFishInLava[Item.type] = true;

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
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int index = 0; index < 10; ++index)
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
                AddIngredient<CosmiliteBar>(10).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
