using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ToxicantTwister : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Toxicant Twister");
            Tooltip.SetDefault("Throws a speedy boomerang\n" +
                               "Stealth strikes fire two boomerangs that go much faster and release sand rapidly");
        }

        public override void SafeSetDefaults()
        {
            item.width = 42;
            item.height = 46;
            item.damage = 950;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = item.useTime = 20;
            item.useStyle = 1;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.Calamity().postMoonLordRarity = 13;
			item.rare = 10;
            item.shoot = ModContent.ProjectileType<ToxicantTwisterProjectile>();
            item.shootSpeed = 18f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			int projType = item.shoot;
			if (Main.rand.NextBool(2))
			{
				projType = ModContent.ProjectileType<ToxicantTwisterTwoPointZero>();
			}

            if (player.Calamity().StealthStrikeAvailable())
			{
				float SpeedX = speedX + (float)Main.rand.Next(-20, 21) * 0.05f;
				float SpeedY = speedY + (float)Main.rand.Next(-20, 21) * 0.05f;
				float SpeedX2 = speedX + (float)Main.rand.Next(-20, 21) * 0.05f;
				float SpeedY2 = speedY + (float)Main.rand.Next(-20, 21) * 0.05f;
				int stealth = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
				int stealth2 = Projectile.NewProjectile(position.X, position.Y, SpeedX2, SpeedY2, ModContent.ProjectileType<ToxicantTwisterTwoPointZero>(), damage, knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[stealth].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
				Main.projectile[stealth2].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
				if (Main.projectile[stealth].Calamity().stealthStrike)
				{
					Main.projectile[stealth].velocity *= 1.5f;
					Main.projectile[stealth].timeLeft = 420;
					Main.projectile[stealth].penetrate = -1;
					Main.projectile[stealth].localNPCHitCooldown = 6;
				}
				if (Main.projectile[stealth2].Calamity().stealthStrike)
				{
					Main.projectile[stealth2].penetrate += 2;
					Main.projectile[stealth2].localNPCHitCooldown = 8;
				}
			}
			else
			{
				Projectile.NewProjectile(position, new Vector2(speedX, speedY), projType, damage, knockBack, player.whoAmI, 0f, 0f);
			}
            return false;
        }
    }
}
