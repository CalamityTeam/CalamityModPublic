using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class SicknessRound : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.25f;
            Projectile.extraUpdates = 4;
            AIType = ProjectileID.Bullet;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool PreAI()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                Vector2 dspeed = -Projectile.velocity * 0.5f;
                float x2 = Projectile.Center.X - Projectile.velocity.X / 10f;
                float y2 = Projectile.Center.Y - Projectile.velocity.Y / 10f;
                int dust = Dust.NewDust(new Vector2(x2, y2), 1, 1, 107, dspeed.X, dspeed.Y, 0, default, 1f);
                Main.dust[dust].alpha = Projectile.alpha;
                Main.dust[dust].position.X = x2;
                Main.dust[dust].position.Y = y2;
                Main.dust[dust].velocity = dspeed;
                Main.dust[dust].noGravity = true;
            }

            //Rotation
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = (Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi)) + MathHelper.ToRadians(90) * Projectile.direction;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<Sickness>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                for (int k = 0; k < 3; k++)
                {
                    Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 107, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-35, 36) * 0.2f, Main.rand.Next(-35, 36) * 0.2f, ModContent.ProjectileType<SicknessRound2>(),
                    (int)(Projectile.damage * 0.5), Projectile.knockBack * 0.5f, Main.myPlayer);
                }
            }
        }
    }
}
