using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DefectiveSphere : ModItem
    {
        public static int BaseDamage = 130;
        public static float Speed = 15f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Defective Sphere");
            Tooltip.SetDefault(@"Fires a variety of deadly spheres with different effects
Stacks up to 5
Stealth strikes launch all 4 sphere types at once");
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 44;
            Item.damage = BaseDamage;
            Item.knockBack = 5f;
            Item.useAnimation = 13;
            Item.useTime = 13;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.maxStack = 5;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item15; //phaseblade sound effect

            Item.value = CalamityGlobalItem.RarityYellowBuyPrice / 5; // Stacks up to 5
            Item.rare = ItemRarityID.Yellow;

            Item.DamageType = RogueDamageClass.Instance;
            Item.shoot = ProjectileType<SphereSpiked>();
            Item.shootSpeed = Speed;
        }

        public override bool CanUseItem(Player player)
        {
            int UseMax = Item.stack;

            if (player.Calamity().StealthStrikeAvailable())
            {
                return true;
            }
            else if ((player.ownedProjectileCounts[Item.shoot] + player.ownedProjectileCounts[ProjectileType<SphereBladed>()] + player.ownedProjectileCounts[ProjectileType<SphereYellow>()] + player.ownedProjectileCounts[ProjectileType<SphereBlue>()]) >= UseMax)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.RogueWeapon;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
                    Vector2 shootVelocity = velocity + Main.rand.NextVector2Square(-1.5f, 1.5f);
                    int stealth = Projectile.NewProjectile(source, position, shootVelocity, projectileType, (int)(damage * 0.625f), knockback, player.whoAmI);
                    if (stealth.WithinBounds(Main.maxProjectiles))
                        Main.projectile[stealth].Calamity().stealthStrike = true;
                }
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, sphereType, damage, knockback, player.whoAmI, 0f, 0f);
            }
            return false;
        }
    }
}
