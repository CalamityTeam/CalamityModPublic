using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Rogue
{
	public class DefectiveSphere : RogueWeapon
    {
        public static int BaseDamage = 130;
        public static float Speed = 15f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Defective Sphere");
            Tooltip.SetDefault(@"Fires a variety of deadly spheres with different effects
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

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item15; //phaseblade sound effect

            item.value = Item.buyPrice(0, 16, 0, 0);
            item.rare = ItemRarityID.Yellow;

            item.Calamity().rogue = true;
            item.shoot = ProjectileType<SphereSpiked>();
            item.shootSpeed = Speed;
        }

        public override bool CanUseItem(Player player)
        {
			int UseMax = item.stack;

			if (player.Calamity().StealthStrikeAvailable())
			{
				return true;
			}
			else if ((player.ownedProjectileCounts[item.shoot] + player.ownedProjectileCounts[ProjectileType<SphereBladed>()] + player.ownedProjectileCounts[ProjectileType<SphereYellow>()] + player.ownedProjectileCounts[ProjectileType<SphereBlue>()]) >= UseMax)
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
			int sphereType = Utils.SelectRandom(Main.rand, new int[]
			{
				type,
				ProjectileType<SphereBladed>(),
				ProjectileType<SphereYellow>(),
				ProjectileType<SphereBlue>()
			});

			//Kinda ugly but idk how to make it cleaner
			float SpeedX = speedX + Main.rand.NextFloat(-30, 30) * 0.05f;
			float SpeedY = speedY + Main.rand.NextFloat(-30, 30) * 0.05f;
			float SpeedX2 = speedX + Main.rand.NextFloat(-30, 30) * 0.05f;
			float SpeedY2 = speedY + Main.rand.NextFloat(-30, 30) * 0.05f;
			float SpeedX3 = speedX + Main.rand.NextFloat(-30, 30) * 0.05f;
			float SpeedY3 = speedY + Main.rand.NextFloat(-30, 30) * 0.05f;
			float SpeedX4 = speedX + Main.rand.NextFloat(-30, 30) * 0.05f;
			float SpeedY4 = speedY + Main.rand.NextFloat(-30, 30) * 0.05f;

            if (player.Calamity().StealthStrikeAvailable())
			{
				int stealth = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage / 2, knockBack, player.whoAmI);
				int stealth2 = Projectile.NewProjectile(position.X, position.Y, SpeedX2, SpeedY2, ProjectileType<SphereBladed>(), damage / 2, knockBack, player.whoAmI);
				int stealth3 = Projectile.NewProjectile(position.X, position.Y, SpeedX3, SpeedY3, ProjectileType<SphereYellow>(), damage / 2, knockBack, player.whoAmI);
				int stealth4 = Projectile.NewProjectile(position.X, position.Y, SpeedX4, SpeedY4, ProjectileType<SphereBlue>(), damage / 2, knockBack, player.whoAmI);
				if (stealth.WithinBounds(Main.maxProjectiles))
					Main.projectile[stealth].Calamity().stealthStrike = true;
				if (stealth2.WithinBounds(Main.maxProjectiles))
					Main.projectile[stealth2].Calamity().stealthStrike = true;
				if (stealth3.WithinBounds(Main.maxProjectiles))
					Main.projectile[stealth3].Calamity().stealthStrike = true;
				if (stealth4.WithinBounds(Main.maxProjectiles))
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
