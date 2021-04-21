using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class AstralCrystal : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/AstralFlame";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.penetrate = 5;
            projectile.extraUpdates = 2;
            projectile.tileCollide = false;
            projectile.magic = true;
        }

        public override void Kill(int timeLeft)
        {
            //make dust shape
            bool blue = Main.rand.NextBool();
            float angleStart = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            for (float angle = 0f; angle < MathHelper.TwoPi; angle += 0.05f)
            {
                blue = !blue;
                Vector2 velocity = angle.ToRotationVector2() * (2f + (float)(Math.Sin(angleStart + angle * 3f) + 1) * 2.5f) * Main.rand.NextFloat(0.95f, 1.05f);
                Dust d = Dust.NewDustPerfect(projectile.Center, blue ? ModContent.DustType<AstralBlue>() : ModContent.DustType<AstralOrange>(), velocity);
                d.customData = 0.025f;
            }

            //chunks
            for (int i = 0; i < Main.rand.Next(5, 9); i++)
            {
                Dust d = Dust.NewDustPerfect(projectile.Center, ModContent.DustType<AstralChunkDust>());
            }

            Main.PlaySound(SoundID.Item27, projectile.Center);

            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver4 / 2f)
            {
                Projectile.NewProjectile(projectile.Center, i.ToRotationVector2() * 9f, ModContent.ProjectileType<AstralCrystalInvisibleExplosion>(), projectile.damage, projectile.knockBack, projectile.owner);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Math.Max((int)lightColor.R, 150),
                Math.Max((int)lightColor.G, 150),
                Math.Max((int)lightColor.B, 150));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.Kill();
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
        }

        public override void AI()
        {
            //FRAMING
            projectile.frameCounter++;
            if (projectile.frameCounter > 5)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame > 3)
                {
                    projectile.frame = 0;
                }
            }

            //ROTATION
            projectile.rotation = projectile.velocity.ToRotation();

            //TILE COLLISION
            if (projectile.Center.Y > projectile.ai[0])
            {
                projectile.tileCollide = true;
            }

            //Normal astral dusts
            Vector2 vect = projectile.velocity;
            vect.Normalize();
            vect *= 32;
            Vector2 pos = projectile.Center + vect;
            Vector2 perp = new Vector2(projectile.velocity.Y, -projectile.velocity.X);
            perp.Normalize();
            bool flag = Main.time % 2 == 0;
            int blue = ModContent.DustType<AstralBlue>();
            int orange = ModContent.DustType<AstralOrange>();
            projectile.ai[1] += 0.3141f; //2pi / 20 (total frames for one loop of animation)
            Vector2 posOff = perp * (float)Math.Sin(projectile.ai[1]) * 6f;
            Dust d1 = Dust.NewDustPerfect(pos + posOff, flag ? blue : orange, perp * Main.rand.NextFloat(2.3f, 3.5f));
            Dust d2 = Dust.NewDustPerfect(pos - posOff, flag ? orange : blue, -perp * Main.rand.NextFloat(2.3f, 3.5f));
            d1.customData = d2.customData = 0.035f;

            //Astral chunk dust
            if (Main.rand.NextBool(30))
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, ModContent.DustType<AstralChunkDust>());
                dust.velocity *= 0.3f;
            }
        }
    }
}
