using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ChargedBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/LaserProj";
        public Color baseColor = Color.White;
        public bool outOfTime = false;
        public Vector2 baseVel;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 6;
            Projectile.timeLeft = 360;
        }

        public override void AI()
        {
            bool Shredder = Projectile.ai[2] == 0;
            bool Infinity = Projectile.ai[2] == 1;
            bool Svant = Projectile.ai[2] == 2 || Projectile.ai[2] == 3 || Projectile.ai[2] == 4;

            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            if (baseColor == Color.White)
            {
                baseColor = (Infinity ? new Color(229, 49, 39) : Svant ? Color.DarkViolet : Color.DodgerBlue);
                if (Projectile.ai[2] == 3)
                {
                    baseColor = Color.DarkOrchid;
                }
                if (Projectile.ai[2] == 4)
                {
                    baseColor = Color.MediumOrchid;
                }
                baseVel = Projectile.velocity;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.timeLeft == 2)
                outOfTime = true;
            if ((Svant || Infinity) && Projectile.timeLeft % 2 == 0 && targetDist < 1400f && Projectile.timeLeft < 340)
            {
                Particle spark = new LineParticle(Projectile.Center - Projectile.velocity * 3, -Projectile.velocity * 0.05f, false, 5, 2f, baseColor * 0.65f);
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (!outOfTime)
            {
                SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
                if (Projectile.owner == Main.myPlayer)
                {
                    for (int k = 0; k < 2; k++)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, -baseVel.RotatedByRandom(0.9f) * Main.rand.NextFloat(0.6f, 0.7f), ModContent.ProjectileType<ChargedBlastSplit>(), (int)(Projectile.damage * (Projectile.ai[2] > 0 ? 0.3f : 0.5f)), Projectile.knockBack * 0.8f, Main.myPlayer, 0, 0, Projectile.ai[2]);
                }
                for (int k = 0; k < 3; k++)
                {
                    Particle spark2 = new LineParticle(Projectile.Center, (-baseVel * 4).RotatedByRandom(0.9f) * Main.rand.NextFloat(0.8f, 1.2f), false, Main.rand.Next(25, 32 + 1), Main.rand.NextFloat(1.5f, 2f), baseColor);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
                if (Projectile.ai[2] == 1)
                {
                    Particle spark2 = new LineParticle(Projectile.Center, (-baseVel * 5).RotatedByRandom(0.9f) * Main.rand.NextFloat(0.8f, 1.2f), false, Main.rand.Next(35, 48 + 1), Main.rand.NextFloat(2.3f, 3f), baseColor);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (baseColor == Color.White)
                return false;
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Particles/DrainLineBloom").Value;
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], (baseColor * 0.7f) with { A = 0 }, 1, texture);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, baseColor with { A = 0 }, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 20, targetHitbox);
    }
}
