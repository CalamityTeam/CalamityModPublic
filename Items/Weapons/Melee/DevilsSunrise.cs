using Terraria.DataStructures;
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
    public class DevilsSunrise : ModItem
    {
        public static int BaseDamage = 480;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devil's Sunrise");
            Tooltip.SetDefault("Balls? Smalls.");
        }

        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 66;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.damage = BaseDamage;
            Item.knockBack = 4f;
            Item.useAnimation = 25;
            Item.useTime = 5;
            Item.autoReuse = false;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.Calamity().donorItem = true;

            Item.Calamity().trueMelee = true;
            Item.shoot = ModContent.ProjectileType<DevilsSunriseProj>();
            Item.shootSpeed = 24f;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref int crit) => crit += 10;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<DevilsSunriseProj>(), damage, knockback, player.whoAmI, 0f, 0f);
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<DevilsSunriseCyclone>(), damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Arkhalis).
                AddIngredient<DemonicBoneAsh>(10).
                AddIngredient<BloodstoneCore>(25).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
