using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DefectiveSphere : RogueWeapon
    {
        public static int BaseDamage = 80;
        public static float Speed = 15f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Defective Sphere");
            Tooltip.SetDefault(@"Fires a variety deadly spheres with different effects
Stacks up to 5
Stealth strikes launch all 4 sphere types at once");
        }

        public override void SafeSetDefaults()
        {
            item.width = 42;
            item.height = 44;
            item.damage = BaseDamage;
            item.knockBack = 5f;
            item.useAnimation = 13;
            item.useTime = 13;
            item.autoReuse = true;
            item.noMelee = true;
            item.noUseGraphic = true;
			item.maxStack = 5;

            item.useStyle = 1;
            item.UseSound = SoundID.Item15; //phaseblade sound effect

            item.value = Item.buyPrice(0, 16, 0, 0);
            item.rare = 8;

            item.Calamity().rogue = true;
            item.shoot = ModContent.ProjectileType<SphereSpiked>();
            item.shootSpeed = Speed;
        }

        public override bool CanUseItem(Player player)
        {
			int UseMax = item.stack;

			if (player.Calamity().StealthStrikeAvailable())
			{
				return true;
			}
			else if ((player.ownedProjectileCounts[item.shoot] + player.ownedProjectileCounts[ModContent.ProjectileType<SphereBladed>()] + player.ownedProjectileCounts[ModContent.ProjectileType<SphereYellow>()] + player.ownedProjectileCounts[ModContent.ProjectileType<SphereBlue>()]) >= UseMax)
			{
				return false;
			}
			else
			{
				return true;
			}
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			int sphereType = type;
			if (Main.rand.NextBool(4))
				sphereType = ModContent.ProjectileType<SphereBladed>();
			else if (Main.rand.NextBool(3))
				sphereType = ModContent.ProjectileType<SphereYellow>();
			else if (Main.rand.NextBool(2))
				sphereType = ModContent.ProjectileType<SphereBlue>();
			float SpeedX = speedX + (float)Main.rand.Next(-30, 31) * 0.05f;
			float SpeedY = speedY + (float)Main.rand.Next(-30, 31) * 0.05f;
			float SpeedX2 = speedX + (float)Main.rand.Next(-30, 31) * 0.05f;
			float SpeedY2 = speedY + (float)Main.rand.Next(-30, 31) * 0.05f;
			float SpeedX3 = speedX + (float)Main.rand.Next(-30, 31) * 0.05f;
			float SpeedY3 = speedY + (float)Main.rand.Next(-30, 31) * 0.05f;
			float SpeedX4 = speedX + (float)Main.rand.Next(-30, 31) * 0.05f;
			float SpeedY4 = speedY + (float)Main.rand.Next(-30, 31) * 0.05f;

            if (player.Calamity().StealthStrikeAvailable())
			{
				int stealth = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage / 2, knockBack, player.whoAmI, 0f, 0f);
				int stealth2 = Projectile.NewProjectile(position.X, position.Y, SpeedX2, SpeedY2, ModContent.ProjectileType<SphereBladed>(), damage / 2, knockBack, player.whoAmI, 0f, 0f);
				int stealth3 = Projectile.NewProjectile(position.X, position.Y, SpeedX3, SpeedY3, ModContent.ProjectileType<SphereYellow>(), damage / 2, knockBack, player.whoAmI, 0f, 0f);
				int stealth4 = Projectile.NewProjectile(position.X, position.Y, SpeedX4, SpeedY4, ModContent.ProjectileType<SphereBlue>(), damage / 2, knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[stealth].Calamity().stealthStrike = true;
				Main.projectile[stealth2].Calamity().stealthStrike = true;
				Main.projectile[stealth3].Calamity().stealthStrike = true;
				Main.projectile[stealth4].Calamity().stealthStrike = true;
			}
			else
			{
				Projectile.NewProjectile(position, new Vector2(speedX, speedY), sphereType, damage, knockBack, player.whoAmI, 0f, 0f);
			}
            return false;
        }
    }
}
