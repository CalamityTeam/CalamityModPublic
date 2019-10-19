using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Galeforce : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Galeforce");
            Tooltip.SetDefault("Fires a spread of low-damage feathers");
        }

        public override void SetDefaults()
        {
            item.damage = 18;
            item.ranged = true;
            item.width = 32;
            item.height = 52;
            item.useTime = 17;
            item.useAnimation = 17;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 3f;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 20f;
            item.useAmmo = 40;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int numProj = 2;
            float rotation = MathHelper.ToRadians(4);
            for (int i = 0; i < numProj + 1; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                Projectile.NewProjectile(position.X, position.Y, perturbedSpeed.X * 0.75f, perturbedSpeed.Y * 0.75f, ModContent.ProjectileType<FeatherLarge>(), (int)((double)damage * 0.25), 0f, player.whoAmI, 0f, 0f);
            }
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 8);
            recipe.AddIngredient(ItemID.SunplateBlock, 3);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
