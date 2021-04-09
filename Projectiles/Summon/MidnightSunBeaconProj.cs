using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
	public class MidnightSunBeaconProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Summon/MidnightSunBeacon";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Beacon");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 32;
            projectile.height = 32;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 420;
        }

        public override void AI()
        {
            projectile.rotation = projectile.rotation.AngleLerp(-MathHelper.PiOver4, 0.03f);
            if (Math.Abs(projectile.rotation + MathHelper.PiOver4) < 0.02f && projectile.ai[0] == 0f)
            {
                for (int i = 1; i <= 4; i++)
                {
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<MidnightSunSkyBeam>(), projectile.damage / 4, 0f, projectile.owner, 
                        projectile.whoAmI, i - 2);
                }
                projectile.ai[1] = MidnightSunSkyBeam.TrueTimeLeft + 60f;
                projectile.ai[0] = 1f;
            }
            if (projectile.ai[1] == 1f)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.UnitY * 30f, ModContent.ProjectileType<MidnightSunUFO>(), projectile.damage, projectile.knockBack,
                    projectile.owner);
                projectile.Kill();
            }
            if (projectile.ai[1] > 0)
                projectile.ai[1]--;
            if (projectile.ai[1] > 1f &&
                projectile.ai[1] <= 60f)
            {
                projectile.velocity.Y -= 0.4f;
            }
            else
                projectile.velocity *= 0.96f;
		}
        public override bool CanDamage() => false;
    }
}
