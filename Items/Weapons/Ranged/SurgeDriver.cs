using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SurgeDriver : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 140;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 164;
            Item.height = 58;
            Item.useTime = Item.useAnimation = 28;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 8f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PrismEnergyBullet>();
            Item.shootSpeed = 11f;
            Item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-50f, -8f);

        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.Calamity().rightClickListener = true;

            Item.noUseGraphic = !player.Calamity().mouseRight && player.ownedProjectileCounts[ModContent.ProjectileType<SurgeDriverHoldout>()] > 0;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<SurgeDriverHoldout>()] <= 0;

        public override float UseSpeedMultiplier(Player player) => player.altFunctionUse == 2f ? 2.5f : 1f;

        // Spawning the holdout cannot consume ammo
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.altFunctionUse == 2f || player.ownedProjectileCounts[ModContent.ProjectileType<SurgeDriverHoldout>()] > 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootVelocity = velocity;
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            Vector2 gunTip = position + shootDirection * Item.scale * 126f;
            gunTip.Y -= 6f;

            // Large bullet/rocket releasing.
            if (player.Calamity().mouseRight)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 newShootVelocity = shootVelocity * Main.rand.NextFloat(1f, 1.45f);
                    newShootVelocity = newShootVelocity.RotatedByRandom(0.15f);
                    Projectile.NewProjectile(source, gunTip, newShootVelocity, Item.shoot, damage, knockback, player.whoAmI);
                }
            }

            // Snipe blast. Done via a held projectile.
            else
                Projectile.NewProjectile(source, gunTip, shootVelocity, ModContent.ProjectileType<SurgeDriverHoldout>(), 0, knockback, player.whoAmI);

            return false;
        }
    }
}
