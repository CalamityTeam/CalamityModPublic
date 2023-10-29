using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class Exocomet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.alpha = 50;
            Projectile.timeLeft = 360;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 240 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 4)
                Projectile.frame = 0;

            if (Projectile.timeLeft > 30 && Projectile.alpha > 0)
                Projectile.alpha -= 25;
            if (Projectile.timeLeft > 30 && Projectile.alpha < 128 && Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                Projectile.alpha = 128;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (Projectile.alpha < 40)
            {
                int exo = Dust.NewDust(new Vector2(Projectile.position.X - Projectile.velocity.X * 4f + 2f, Projectile.position.Y + 2f - Projectile.velocity.Y * 4f), 8, 8, 107, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, new Color(0, 255, 255), 0.5f);
                Main.dust[exo].velocity *= -0.25f;
                exo = Dust.NewDust(new Vector2(Projectile.position.X - Projectile.velocity.X * 4f + 2f, Projectile.position.Y + 2f - Projectile.velocity.Y * 4f), 8, 8, 107, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, new Color(0, 255, 255), 0.5f);
                Main.dust[exo].velocity *= -0.25f;
                Main.dust[exo].position -= Projectile.velocity * 0.5f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            Lighting.AddLight(Projectile.Center, 0f, 0.5f, 0.5f);

            if (Projectile.timeLeft < 240)
                CalamityUtils.HomeInOnNPC(Projectile, true, 600f, 12f, 20f);
            else
            {
                float projVelocity = 100f * Projectile.ai[1];
                float scaleFactor = 20f * Projectile.ai[1];
                if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
                {
                    if (Projectile.Distance(Main.player[Projectile.owner].Center) > 40f)
                    {
                        Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center, Vector2.UnitY);
                        Projectile.velocity = (Projectile.velocity * (projVelocity - 1f) + moveDirection * scaleFactor) / projVelocity;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);

            float healAmt = hit.Damage * 0.01f;
            if ((int)healAmt == 0)
                return;

            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

            if (healAmt > CalamityMod.lifeStealCap)
                healAmt = CalamityMod.lifeStealCap;

            CalamityGlobalProjectile.SpawnLifeStealProjectile(Projectile, Main.player[Projectile.owner], healAmt, ModContent.ProjectileType<Exoheal>(), 1200f, 3f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);

            float healAmt = info.Damage * 0.01f;
            if ((int)healAmt == 0)
                return;

            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

            if (healAmt > CalamityMod.lifeStealCap)
                healAmt = CalamityMod.lifeStealCap;

            CalamityGlobalProjectile.SpawnLifeStealProjectile(Projectile, Main.player[Projectile.owner], healAmt, ModContent.ProjectileType<Exoheal>(), 1200f, 3f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(0, 255, 255, Projectile.alpha);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Zombie103, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 80;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
            }
            for (int j = 0; j < 20; j++)
            {
                int exoDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 107, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
                Main.dust[exoDust].noGravity = true;
                Main.dust[exoDust].velocity *= 3f;
                exoDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
                Main.dust[exoDust].velocity *= 2f;
                Main.dust[exoDust].noGravity = true;
            }
        }
    }
}
