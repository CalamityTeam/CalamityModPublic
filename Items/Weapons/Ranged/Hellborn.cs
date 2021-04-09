using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Hellborn : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hellborn");
            Tooltip.SetDefault("Fires a spread of 3 bullets\n" +
				"Converts musket balls into explosive rounds\n" +
                "Enemies that touch the gun while it's being fired take massive damage");
        }

        public override void SetDefaults()
        {
            item.damage = 18;
            item.ranged = true;
            item.width = 50;
            item.height = 24;
            item.useTime = 9;
            item.useAnimation = 9;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 1f;
			item.value = CalamityGlobalItem.Rarity5BuyPrice;
			item.rare = ItemRarityID.Pink;
			item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 14f;
            item.useAmmo = AmmoID.Bullet;
			item.Calamity().challengeDrop = true;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 3; ++index)
            {
                float SpeedX = speedX + Main.rand.Next(-15, 16) * 0.05f;
                float SpeedY = speedY + Main.rand.Next(-15, 16) * 0.05f;

				if (type == ProjectileID.Bullet)
				{
					int bullet = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ProjectileID.ExplosiveBullet, damage, knockBack, player.whoAmI);
					Main.projectile[bullet].usesLocalNPCImmunity = true;
					Main.projectile[bullet].localNPCHitCooldown = 10;
				}
				else
					Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI);
			}
            return false;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            damage *= 15;
        }

        public override void ModifyHitPvp(Player player, Player target, ref int damage, ref bool crit)
        {
            damage *= 15;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 360);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 360);
        }
    }
}
