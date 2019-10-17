using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Aeries : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aeries");
            Tooltip.SetDefault("Fires shockblast rounds that can steal enemy life");
        }

        public override void SetDefaults()
        {
            item.damage = 35;
            item.ranged = true;
            item.width = 50;
            item.height = 32;
            item.useTime = 10;
            item.useAnimation = 10;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5.5f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.UseSound = SoundID.Item41;
            item.autoReuse = true;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<ShockblastRound>();
            item.useAmmo = 97;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<ShockblastRound>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpectreBar, 5);
            recipe.AddIngredient(null, "CursedCapper");
            recipe.AddIngredient(ItemID.ShroomiteBar, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
