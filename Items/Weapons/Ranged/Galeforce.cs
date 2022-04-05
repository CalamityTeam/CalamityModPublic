using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 52;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = -8; i <= 8; i += 8)
            {
                Vector2 perturbedSpeed = velocity.RotatedBy(MathHelper.ToRadians(i));
                Projectile.NewProjectile(source, position, perturbedSpeed, ModContent.ProjectileType<FeatherLarge>(), damage / 4, 0f, player.whoAmI);
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<AerialiteBar>(), 8).AddIngredient(ItemID.SunplateBlock, 3).AddTile(TileID.SkyMill).Register();
        }
    }
}
