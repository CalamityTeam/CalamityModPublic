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

            if (player.Calamity().StealthStrikeAvailable())
            {
                // This is done to allow looping when creating projectiles, instead of having to create many projectile/velocity variables all at once,
                // which this shoot code used to do.
                int[] projectilesToShoot = new int[]
                {
                    type,
                    ProjectileType<SphereBladed>(),
                    ProjectileType<SphereYellow>(),
                    ProjectileType<SphereBlue>()
                };

                foreach (int projectileType in projectilesToShoot)
                {
                    Vector2 shootVelocity = new Vector2(speedX, speedY) + Main.rand.NextVector2Square(-1.5f, 1.5f);
                    int stealth = Projectile.NewProjectile(position, shootVelocity, projectileType, (int)(damage * 0.625f), knockBack, player.whoAmI);
                    if (stealth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                }
            }
            else
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), sphereType, damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
