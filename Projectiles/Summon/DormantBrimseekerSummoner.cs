using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class DormantBrimseekerSummoner : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Summon/DormantBrimseeker";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dormant Brimseeker");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 30;
            projectile.height = 30;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.rotation.AngleLerp(-MathHelper.PiOver4, 0.045f);
            projectile.velocity *= 0.975f;
            if (projectile.ai[0]++ == 110f)
            {
                Main.PlaySound(SoundID.Item100, projectile.Center);
            }
            if (projectile.ai[0]++ >= 90f)
            {
                for (int i = 0; i < (180 - (int)projectile.ai[0]) / 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.Brimstone);
                    dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1f, 4f);
                    dust.noGravity = true;
                }
                projectile.alpha = (int)(255 * (projectile.ai[0] - 90f) / 90f);
            }
            if (projectile.ai[0] >= 180f)
            {
                projectile.Kill();
            }
		}

        public override void Kill(int timeLeft)
        {
			Projectile.NewProjectile(projectile.Center, Vector2.UnitY * 7f, ModContent.ProjectileType<DormantBrimseekerBab>(), projectile.damage, projectile.knockBack, projectile.owner);
		}
    }
}
