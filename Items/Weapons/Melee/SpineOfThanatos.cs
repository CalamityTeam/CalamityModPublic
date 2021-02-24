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
        }

        public override void SetDefaults()
        {
            item.width = item.height = 28;
            item.damage = 1450;
            item.rare = ItemRarityID.Red;
            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.autoReuse = true;
            item.melee = true;
            item.useAnimation = item.useTime = 24;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item68;
            item.shootSpeed = 1f;
            item.shoot = ModContent.ProjectileType<SpineOfThanatosProjectile>();
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 2; i++)
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, (i == 0f).ToDirectionInt());

            // Create a third, final whip that does not swing around at all and instead simply flies towards the mouse.
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
