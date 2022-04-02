using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Condemnation : ModItem
    {
        public const int MaxLoadedArrows = 8;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Condemnation");
            Tooltip.SetDefault("Fires powerful scarlet bolts suffused with hateful magics\n" +
                "Hold left click to load up to eight bolts for powerful burst fire\n" +
                "Hold right click to use the repeater full auto");
        }

        public override void SetDefaults()
        {
            item.damage = 1914;
            item.ranged = true;
            item.width = 130;
            item.height = 42;
            item.useTime = item.useAnimation = 23;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.channel = true;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<CondemnationArrow>();
            item.shootSpeed = 14.5f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-50f, 0f);

        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.Calamity().rightClickListener = true;

            if (player.Calamity().mouseRight && player.ownedProjectileCounts[ModContent.ProjectileType<CondemnationHoldout>()] <= 0)
            {
                item.noUseGraphic = false;
            }
            else
            {
                item.noUseGraphic = true;
            }
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<CondemnationHoldout>()] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 shootVelocity = new Vector2(speedX, speedY);
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);



            // Single arrow firing.
            if (player.Calamity().mouseRight)
            {
                Vector2 tipPosition = position + shootDirection * 110f;
                Projectile.NewProjectile(tipPosition, shootVelocity, item.shoot, damage, knockBack, player.whoAmI);
            }

            // Charge-up. Done via a holdout projectile.
            else
                Projectile.NewProjectile(position, shootDirection, ModContent.ProjectileType<CondemnationHoldout>(), 0, 0f, player.whoAmI);
            return false;
        }
    }
}
