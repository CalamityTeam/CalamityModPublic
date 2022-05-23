using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class RedtideSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Redtide Sword");
            Tooltip.SetDefault("Throws short-range whirlpools");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.damage = 35;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 19;
            Item.useTurn = true;
            Item.knockBack = 4;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 42;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<RedtideWhirlpool>();
            Item.shootSpeed = 9f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage / 2, knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VictideBar>(3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
