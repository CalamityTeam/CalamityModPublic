using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class BrimlashProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.aiStyle = ProjAIStyleID.Beam;
            AIType = ProjectileID.LightBeam;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.99f;
            if (Projectile.velocity.Length() < 3f)
            {
                Projectile.Kill();
            }
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.05f / 255f, (255 - Projectile.alpha) * 0.05f / 255f);
            int brimDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 1f);
            Main.dust[brimDust].noGravity = true;
            Main.dust[brimDust].velocity *= 0.5f;
            Main.dust[brimDust].velocity += Projectile.velocity * 0.1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 295)
                return false;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            int inc;
            for (int i = 4; i < 31; i = inc + 1)
            {
                float dustX = Projectile.oldVelocity.X * (30f / (float)i);
                float dustY = Projectile.oldVelocity.Y * (30f / (float)i);
                int deathDust = Dust.NewDust(new Vector2(Projectile.oldPosition.X - dustX, Projectile.oldPosition.Y - dustY), 8, 8, (int)CalamityDusts.Brimstone, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.8f);
                Main.dust[deathDust].noGravity = true;
                Dust dust = Main.dust[deathDust];
                dust.velocity *= 0.5f;
                deathDust = Dust.NewDust(new Vector2(Projectile.oldPosition.X - dustX, Projectile.oldPosition.Y - dustY), 8, 8, (int)CalamityDusts.Brimstone, Projectile.oldVelocity.X, Projectile.oldVelocity.Y, 100, default, 1.4f);
                dust = Main.dust[deathDust];
                dust.velocity *= 0.05f;
                inc = i;
            }

            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 velocity = ((MathHelper.TwoPi * i / 3f) + Projectile.velocity.ToRotation()).ToRotationVector2() * 4f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<Brimlash2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
    }
}
