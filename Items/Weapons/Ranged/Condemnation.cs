using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Condemnation : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Condemnation");
            Tooltip.SetDefault("Left clicks allows you to charge the repeater and release more arrows the longer the charge lasts\n" +
                "Right clicks release fast moving arrows");
        }

        public override void SetDefaults()
        {
            item.damage = 2700;
            item.ranged = true;
            item.width = 130;
            item.height = 42;
            item.useTime = item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<CondemnationArrow>();
            item.shootSpeed = 14.5f;
            item.useAmmo = AmmoID.Arrow;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-50f, 0f);

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.noUseGraphic = false;
                item.UseSound = SoundID.DD2_BallistaTowerShot;
                item.channel = false;
            }
            else
            {
                item.noUseGraphic = true;
                item.UseSound = SoundID.Item20;
                item.channel = true;
                return player.ownedProjectileCounts[ModContent.ProjectileType<CondemnationHoldout>()] <= 0;
            }
            return base.CanUseItem(player);
        }

        public override float UseTimeMultiplier(Player player) => player.altFunctionUse == 2 ? 0.7f : 1f;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 shootVelocity = new Vector2(speedX, speedY);
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);

            // Single arrow firing.
            if (player.altFunctionUse == 2)
            {
                Vector2 tipPosition = position + shootDirection * 110f;
                Projectile.NewProjectile(tipPosition, shootVelocity, item.shoot, damage, knockBack, player.whoAmI);
            }

            // Charge-up. Done via a holdout projectile.
            else
                Projectile.NewProjectile(position, shootDirection, ModContent.ProjectileType<CondemnationHoldout>(), 0, knockBack, player.whoAmI);
            return false;
        }
    }
}
