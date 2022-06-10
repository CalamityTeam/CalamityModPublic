using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class SpineOfThanatos : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spine of Thanatos");
            Tooltip.SetDefault("Releases 3 fast metallic whips outward\n" +
                               "Once all three collide, a prism of light is shot outward\n" +
                               "If an enemy is within the line of sight of the whips on collision, the light will fire towards it");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 28;
            Item.damage = 260;
            Item.rare = ItemRarityID.Red;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item68;
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<SpineOfThanatosProjectile>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 2; i++)
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0f, (i == 0f).ToDirectionInt());

            // Create a third, final whip that does not swing around at all and instead simply flies towards the mouse.
            Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
