using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ShockblastRound : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Round");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
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
            Projectile.light = 0.5f;
            Projectile.extraUpdates = 3;
            AIType = ProjectileID.Bullet;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Shockblast>(), Projectile.damage, 0f, Projectile.owner, 0f, Projectile.ai[1]);
                Main.projectile[proj].scale = (Projectile.ai[1] * 0.5f) + 1f;
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool PreAI()
        {
            //Rotation
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * Projectile.direction;

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                for (int num136 = 0; num136 < 2; num136++)
                {
                    Vector2 dspeed = -Projectile.velocity * Main.rand.NextFloat(0.5f, 0.7f);
                    float x2 = Projectile.Center.X - Projectile.velocity.X / 10f * num136;
                    float y2 = Projectile.Center.Y - Projectile.velocity.Y / 10f * num136;
                    int num137 = Dust.NewDust(new Vector2(x2, y2), 1, 1, 185, 0f, 0f, 0, default, 1f);
                    Main.dust[num137].alpha = Projectile.alpha;
                    Main.dust[num137].position.X = x2;
                    Main.dust[num137].position.Y = y2;
                    Main.dust[num137].velocity = dspeed;
                    Main.dust[num137].noGravity = true;
                }
            }

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Shockblast>(), Projectile.damage, 0f, Projectile.owner, 0f, Projectile.ai[1]);
                Main.projectile[proj].scale = (Projectile.ai[1] * 0.5f) + 1f;
            }

            if (Main.player[Projectile.owner].moonLeech)
                return;

            float healAmt = damage * 0.05f;
            if ((int)healAmt == 0)
                return;

            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

            if (healAmt > CalamityMod.lifeStealCap)
                healAmt = CalamityMod.lifeStealCap;

            CalamityGlobalProjectile.SpawnLifeStealProjectile(Projectile, Main.player[Projectile.owner], healAmt, ModContent.ProjectileType<TransfusionTrail>(), 1200f, 3f);
        }
    }
}
