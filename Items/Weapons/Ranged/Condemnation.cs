using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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
            SacrificeTotal = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 1914;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 130;
            Item.height = 42;
            Item.useTime = Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CondemnationArrow>();
            Item.shootSpeed = 14.5f;
            Item.useAmmo = AmmoID.Arrow;
            Item.Calamity().canFirePointBlankShots = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-50f, 0f);

        public override bool AltFunctionUse(Player player) => true;

        public override void HoldItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                player.Calamity().rightClickListener = true;

            if (player.Calamity().mouseRight && player.ownedProjectileCounts[ModContent.ProjectileType<CondemnationHoldout>()] <= 0)
            {
                Item.noUseGraphic = false;
            }
            else
            {
                Item.noUseGraphic = true;
            }
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<CondemnationHoldout>()] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootVelocity = velocity;
            Vector2 shootDirection = shootVelocity.SafeNormalize(Vector2.UnitX * player.direction);



            // Single arrow firing.
            if (player.Calamity().mouseRight)
            {
                Vector2 tipPosition = position + shootDirection * 110f;
                Projectile.NewProjectile(source, tipPosition, shootVelocity, Item.shoot, damage, knockback, player.whoAmI);
            }

            // Charge-up. Done via a holdout projectile.
            else
                Projectile.NewProjectile(source, position, shootDirection, ModContent.ProjectileType<CondemnationHoldout>(), 0, 0f, player.whoAmI);
            return false;
        }
    }
}
