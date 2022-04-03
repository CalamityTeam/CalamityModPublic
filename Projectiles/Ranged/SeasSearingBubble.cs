using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.DamageOverTime;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class SeasSearingBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Searing Bubble");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 480;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            if (Projectile.timeLeft < 475)
            {
                for (int num105 = 0; num105 < 2; num105++)
                {
                    float num99 = Projectile.velocity.X / 3f * (float)num105;
                    float num100 = Projectile.velocity.Y / 3f * (float)num105;
                    int num101 = 4;
                    int num102 = Dust.NewDust(new Vector2(Projectile.position.X + (float)num101, Projectile.position.Y + (float)num101), Projectile.width - num101 * 2, Projectile.height - num101 * 2, 202, 0f, 0f, 150, new Color(60, Main.DiscoG, 190), 1.2f);
                    Dust dust = Main.dust[num102];
                    dust.noGravity = true;
                    dust.velocity *= 0.1f;
                    dust.velocity += Projectile.velocity * 0.1f;
                    dust.position.X -= num99;
                    dust.position.Y -= num100;
                }
                if (Main.rand.NextBool(10))
                {
                    int num103 = 4;
                    int num104 = Dust.NewDust(new Vector2(Projectile.position.X + (float)num103, Projectile.position.Y + (float)num103), Projectile.width - num103 * 2, Projectile.height - num103 * 2, 202, 0f, 0f, 150, new Color(60, Main.DiscoG, 190), 0.6f);
                    Main.dust[num104].velocity *= 0.25f;
                    Main.dust[num104].velocity += Projectile.velocity * 0.5f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(60, Main.DiscoG, 190, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item96, Projectile.position);
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 202, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f, 0, new Color(60, Main.DiscoG, 190));
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHitEffects(target.Center);
            target.AddBuff(BuffID.Wet, 300);
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffects(target.Center);
            target.AddBuff(BuffID.Wet, 300);
            target.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 180);
        }

        private void OnHitEffects(Vector2 targetPos)
        {
            SoundEngine.PlaySound(SoundID.Item96, Projectile.Center);
            if (Projectile.ai[0] == 1f)
            {
                for (int x = 0; x < 2; x++)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                        Projectile bubble = CalamityUtils.ProjectileBarrage(Projectile.Center, targetPos, Main.rand.NextBool(), 1000f, 1400f, 80f, 900f, Main.rand.NextFloat(20f, 25f), ModContent.ProjectileType<SeasSearingBubble>(), Projectile.damage / 2, 1f, Projectile.owner);
                        bubble.rotation = angle;
                        bubble.tileCollide = false;
                        bubble.usesLocalNPCImmunity = true;
                        bubble.localNPCHitCooldown = -1;
                    }
                }
            }
        }
    }
}
