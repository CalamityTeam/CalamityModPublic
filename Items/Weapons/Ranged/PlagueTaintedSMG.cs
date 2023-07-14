using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class PlagueTaintedSMG : ModItem, ILocalizedModType
    {
        private const float Spread = 0.15f;

        public new string LocalizationCategory => "Items.Weapons.Ranged";
        
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 85;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 98;
            Item.height = 50;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.Calamity().donorItem = true;
            Item.UseSound = SoundID.Item11;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<PlagueTaintedProjectile>();
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-20, 5);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Item.UseSound = player.altFunctionUse == 2 ? SoundID.Item61 : SoundID.Item11;
            return base.CanUseItem(player);
        }

        public override float UseSpeedMultiplier(Player player) => player.altFunctionUse == 2 ? (1f / 6f) : 1f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 barrelPosition = position + Vector2.Normalize(velocity) * 70f;
            if (player.altFunctionUse == 2)
            {
                // Fire drones to the left and right.
                for (int i = 0; i < 3; i++)
                {
                    Projectile.NewProjectile(source, barrelPosition, velocity.RotatedBy(-Spread * (i + 1)), ModContent.ProjectileType<PlagueTaintedDrone>(), damage, knockback, player.whoAmI);
                    Projectile.NewProjectile(source, barrelPosition, velocity.RotatedBy(Spread * (i + 1)), ModContent.ProjectileType<PlagueTaintedDrone>(), damage, knockback, player.whoAmI);
                }
            }
            else
            {
                float SpeedX = velocity.X + Main.rand.Next(-5, 6) * 0.05f;
                float SpeedY = velocity.Y + Main.rand.Next(-5, 6) * 0.05f;
                Vector2 newVelocity = new Vector2(SpeedX, SpeedY);
                Projectile.NewProjectile(source, barrelPosition, newVelocity, ModContent.ProjectileType<PlagueTaintedProjectile>(), damage, knockback, player.whoAmI);
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BlackHawkRemote>().
                AddIngredient(ItemID.Uzi).
                AddIngredient<Helstorm>().
                AddIngredient<InfectedArmorPlating>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
