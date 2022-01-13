using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class LeadWizard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lead Wizard");
            Tooltip.SetDefault("Something's not right...\n" +
                "33% chance to not consume ammo");
        }

        public override void SetDefaults()
        {
            item.damage = 46;
            item.ranged = true;
            item.width = 66;
            item.height = 34;
            item.useTime = 3;
            item.reuseDelay = 12;
            item.useAnimation = 9;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
			item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item31;
            item.autoReuse = true;
            item.shoot = ProjectileID.BulletHighVelocity;
            item.shootSpeed = 20f;
            item.useAmmo = AmmoID.Bullet;
			item.Calamity().canFirePointBlankShots = true;
		}

		// Terraria seems to really dislike high crit values in SetDefaults
		public override void GetWeaponCrit(Player player, ref int crit) => crit += 30;

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float rotation = MathHelper.ToRadians(6);
            for (int i = 0; i < 2; i++)
            {
                Vector2 perturbedSpeed = new Vector2(speedX, speedY).RotatedBy(MathHelper.Lerp(-rotation, rotation, i == 1 ? 0 : 2));
                Projectile.NewProjectile(position, perturbedSpeed, ProjectileID.BulletHighVelocity, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 33)
                return false;
            return true;
        }
    }
}
