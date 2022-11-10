using Terraria.DataStructures;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Megalodon : ModItem
    {
        private int shotType = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Megalodon");
            Tooltip.SetDefault("50% chance to not consume ammo\n" +
                "Fires streams of water every other shot");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 72;
            Item.height = 32;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2.5f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float SpeedX = velocity.X + Main.rand.Next(-10, 11) * 0.05f;
            float SpeedY = velocity.Y + Main.rand.Next(-10, 11) * 0.05f;

            if (shotType > 2)
                shotType = 1;

            if (shotType == 1)
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, damage, knockback, player.whoAmI, 0.0f, 0.0f);
            else
                Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<ArcherfishShot>(), damage, knockback, player.whoAmI, 0f, 0f);

            shotType++;

            return false;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (shotType == 2)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Megashark).
                AddIngredient<Archerfish>().
                AddIngredient<DepthCells>(10).
                AddIngredient<Lumenyl>(10).
                AddIngredient<PlantyMush>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
