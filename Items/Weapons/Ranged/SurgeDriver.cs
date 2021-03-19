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
                "Right clicks release a barrage of laser beams that release homing energy on enemy hitss");
        }

        public override void SetDefaults()
        {
            item.damage = 1200;
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

        public override Vector2? HoldoutOffset() => Vector2.UnitX * -50f;

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
                item.UseSound = SoundID.Item14;
            else
                item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon");
            return base.CanUseItem(player);
        }
        public override float UseTimeMultiplier(Player player) => player.altFunctionUse == 2 ? 5f : 1f;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 shootVelocity = new Vector2(speedX, speedY);
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);
            Vector2 gunTip = position + shootDirection * item.scale * 132f;

            // Large bullet/rocket releasing.
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 newShootVelocity = shootVelocity * Main.rand.NextFloat(1f, 1.45f);
                    newShootVelocity = newShootVelocity.RotatedByRandom(0.15f);
                    Projectile.NewProjectile(gunTip, newShootVelocity, item.shoot, damage, knockBack, player.whoAmI);
                }
            }

            // Snipe blast.
            else
            {
                shootVelocity *= 0.64f;
                damage = (int)(damage * 6.66);
                Projectile.NewProjectile(gunTip, shootVelocity, ModContent.ProjectileType<PrismaticEnergyBlast>(), damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}
