using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Magic
{
	public class SlitheringEelProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eel");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 360;
            projectile.alpha = 0;
        }

        public override void AI()
        {
            if (projectile.frameCounter++ % 8f == 7f)
            {
                projectile.frame++;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
            projectile.spriteDirection = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = (projectile.spriteDirection * projectile.velocity).ToRotation();
            if (projectile.ai[0] >= 3f)
            {
                projectile.alpha += 5;
                if (projectile.alpha >= 255)
                {
                    projectile.Kill();
                }
            }
            if (projectile.timeLeft % 80f < 35f && projectile.Distance(Main.MouseWorld) > 70f)
            {
                float angleToTarget = projectile.AngleTo(Main.MouseWorld);
                float angleDifference = MathHelper.WrapAngle(angleToTarget - projectile.velocity.ToRotation());
                projectile.velocity = projectile.velocity.RotatedBy(angleDifference / 9f);
            }
            if (projectile.timeLeft % 65f == 64f)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.UnitY * 7f, ModContent.ProjectileType<EelDrop>(), projectile.damage / 2, 2f, projectile.owner);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], new Color(255, 255, 255, 127), 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            projectile.ai[0]++;
            target.AddBuff(BuffID.Venom, 180);
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            projectile.ai[0]++;
            target.AddBuff(BuffID.Venom, 180);
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void Kill(int timeLeft)
        {
            for (int dust = 0; dust <= 22; dust++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f);
            }
        }
    }
}
