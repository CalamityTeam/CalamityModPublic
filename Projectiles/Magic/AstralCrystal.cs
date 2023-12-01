using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class AstralCrystal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/Boss/AstralFlame";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void OnKill(int timeLeft)
        {
            //make dust shape
            bool blue = Main.rand.NextBool();
            float angleStart = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            for (float angle = 0f; angle < MathHelper.TwoPi; angle += 0.05f)
            {
                blue = !blue;
                Vector2 velocity = angle.ToRotationVector2() * (2f + (float)(Math.Sin(angleStart + angle * 3f) + 1) * 2.5f) * Main.rand.NextFloat(0.95f, 1.05f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, blue ? ModContent.DustType<AstralBlue>() : ModContent.DustType<AstralOrange>(), velocity);
                d.customData = 0.025f;
            }

            //chunks
            for (int i = 0; i < Main.rand.Next(5, 9); i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<AstralChunkDust>());
            }

            SoundEngine.PlaySound(SoundID.Item27, Projectile.Center);

            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver4 / 2f)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, i.ToRotationVector2() * 9f, ModContent.ProjectileType<AstralCrystalInvisibleExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Math.Max((int)lightColor.R, 150),
                Math.Max((int)lightColor.G, 150),
                Math.Max((int)lightColor.B, 150));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240);
        }

        public override void AI()
        {
            //FRAMING
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 0;
                }
            }

            //ROTATION
            Projectile.rotation = Projectile.velocity.ToRotation();

            //TILE COLLISION
            if (Projectile.Center.Y > Projectile.ai[0])
            {
                Projectile.tileCollide = true;
            }

            //Normal astral dusts
            Vector2 vect = Projectile.velocity;
            vect.Normalize();
            vect *= 32;
            Vector2 pos = Projectile.Center + vect;
            Vector2 perp = new Vector2(Projectile.velocity.Y, -Projectile.velocity.X);
            perp.Normalize();
            bool flag = Main.time % 2 == 0;
            int blue = ModContent.DustType<AstralBlue>();
            int orange = ModContent.DustType<AstralOrange>();
            Projectile.ai[1] += 0.3141f; //2pi / 20 (total frames for one loop of animation)
            Vector2 posOff = perp * (float)Math.Sin(Projectile.ai[1]) * 6f;
            Dust d1 = Dust.NewDustPerfect(pos + posOff, flag ? blue : orange, perp * Main.rand.NextFloat(2.3f, 3.5f));
            Dust d2 = Dust.NewDustPerfect(pos - posOff, flag ? orange : blue, -perp * Main.rand.NextFloat(2.3f, 3.5f));
            d1.customData = d2.customData = 0.035f;

            //Astral chunk dust
            if (Main.rand.NextBool(30))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<AstralChunkDust>());
                dust.velocity *= 0.3f;
            }
        }
    }
}
