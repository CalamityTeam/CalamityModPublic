using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class PearlGod : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pearl God");
            Tooltip.SetDefault("Your life is mine...");
        }

        public override void SetDefaults()
        {
            item.damage = 100;
            item.ranged = true;
            item.width = 54;
            item.height = 38;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 3f;
			item.value = CalamityGlobalItem.Rarity13BuyPrice;
			item.Calamity().customRarity = CalamityRarity.PureGreen;
			item.UseSound = SoundID.Item41;
            item.autoReuse = true;
            item.shootSpeed = 24f;
            item.shoot = ModContent.ProjectileType<ShockblastRound>();
            item.useAmmo = AmmoID.Bullet;
			item.Calamity().challengeDrop = true;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 2; ++index)
            {
                int proj = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<FrostsparkBulletProj>(), (int)(damage * 0.75), knockBack, player.whoAmI);
				Main.projectile[proj].extraUpdates += index;
			}
            int proj2 = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<ShockblastRound>(), damage, knockBack, player.whoAmI);
			Main.projectile[proj2].extraUpdates += 2;
			return false;
        }
    }
}
