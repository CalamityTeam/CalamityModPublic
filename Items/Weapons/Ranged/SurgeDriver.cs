using Terraria.DataStructures;
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
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 156;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 164;
            Item.height = 58;
            Item.useTime = Item.useAnimation = 56;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.channel = true;
            Item.knockBack = 8f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Violet;
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


            if (player.Calamity().mouseRight && player.ownedProjectileCounts[ModContent.ProjectileType<SurgeDriverHoldout>()] <= 0)
            {
                Item.noUseGraphic = false;
                Item.reuseDelay = 0;
            }
            else
            {
                Item.noUseGraphic = true;
                Item.reuseDelay = 28;
            }
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<SurgeDriverHoldout>()] <= 0;

        public override float UseTimeMultiplier(Player player) => player.Calamity().mouseRight ? 5f : 1f;

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
                    Projectile.NewProjectile(source, gunTip, newShootVelocity, Item.shoot, (int)(damage * 1.08), knockback, player.whoAmI);
                }
            }

            // Snipe blast. Done via a held projectile.
            else
                Projectile.NewProjectile(source, gunTip, shootVelocity, ModContent.ProjectileType<SurgeDriverHoldout>(), 0, knockback, player.whoAmI);
            return false;
        }
    }
}
