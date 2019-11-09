using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class SeasSearing : ModItem
    {
        public static int BaseDamage = 90;
		
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sea's Searing");
            Tooltip.SetDefault("Legendary Drop\n" +
                "Fires a string of bubbles summoning a shower of bubbles on hit\n" +
                "Right click to fire a slower, larger water blast that summons a water spout\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.damage = BaseDamage;
            item.ranged = true;
            item.width = 88;
            item.height = 44;
            item.useTime = 5;
            item.useAnimation = 15;
            item.reuseDelay = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5f;
            item.rare = 5;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.Calamity().postMoonLordRarity = 17;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SeasSearingBubble>();
            item.shootSpeed = 11f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, -5);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
				item.damage = 130;
                item.useTime = 30;
                item.useAnimation = 30;
				item.reuseDelay = 0;
            }
            else
            {
				item.damage = BaseDamage;
                item.useTime = 5;
                item.useAnimation = 15;
				item.reuseDelay = 20;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<SeasSearingSecondary>(), damage, knockBack, player.whoAmI, 0f, 0f);
            }
            else
            {
				Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<SeasSearingBubble>(), damage, knockBack, player.whoAmI, 1f, 0f);
            }
			return false;
        }
    }
}
