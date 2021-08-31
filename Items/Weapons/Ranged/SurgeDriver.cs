using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SurgeDriver : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Surge Driver");
            Tooltip.SetDefault("Left clicks release a laser ray that explodes on collision\n" +
                "Right clicks release a barrage of laser beams that release homing energy on enemy hits");
        }

        public override void SetDefaults()
        {
            item.damage = 156;
            item.ranged = true;
            item.width = 164;
            item.height = 58;
            item.useTime = item.useAnimation = 56;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 8f;
            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<PrismEnergyBullet>();
            item.shootSpeed = 11f;
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-50f, -8f);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.noUseGraphic = false;
                item.reuseDelay = 0;
                item.UseSound = SoundID.Item14;
                item.channel = false;
            }
            else
            {
                item.noUseGraphic = true;
                item.reuseDelay = 28;
                item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");
                item.channel = true;
                return player.ownedProjectileCounts[ModContent.ProjectileType<SurgeDriverHoldout>()] <= 0;
            }
            return base.CanUseItem(player);
        }

        public override float UseTimeMultiplier(Player player) => player.altFunctionUse == 2 ? 5f : 1f;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 shootVelocity = new Vector2(speedX, speedY);
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            Vector2 gunTip = position + shootDirection * item.scale * 126f;
            gunTip.Y -= 6f;

            // Large bullet/rocket releasing.
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 newShootVelocity = shootVelocity * Main.rand.NextFloat(1f, 1.45f);
                    newShootVelocity = newShootVelocity.RotatedByRandom(0.15f);
                    Projectile.NewProjectile(gunTip, newShootVelocity, item.shoot, (int)(damage * 1.08), knockBack, player.whoAmI);
                }
            }

            // Snipe blast. Done via a held projectile.
            else
                Projectile.NewProjectile(gunTip, shootVelocity, ModContent.ProjectileType<SurgeDriverHoldout>(), 0, knockBack, player.whoAmI);
            return false;
        }
    }
}
