using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Magic
{
    public class GranitePulse : ModProjectile
    {
        public bool initialized = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pulse");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
        }

        public override void AI()
        {
            Projectile.velocity = new Vector2(0f, (float)Math.Sin((double)(MathHelper.TwoPi * Projectile.ai[0] / 300f)) * 0.5f);
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 300f)
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] >= 7200f)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= 10f)
            {
                Projectile.localAI[0] = 0f;
                int projCount = 0;
                int index = 0;
                float findOldest = 0f;
                int projType = Projectile.type;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.owner == Projectile.owner && proj.type == projType && proj.ai[1] < 3600f)
                    {
                        projCount++;
                        if (proj.ai[1] > findOldest)
                        {
                            index = i;
                            findOldest = proj.ai[1];
                        }
                    }
                }
                if (projCount > 1)
                {
                    Main.projectile[index].netUpdate = true;
                    Main.projectile[index].ai[1] = 36000f;
                    return;
                }
            }
            if (!initialized)
            {
                SoundEngine.PlaySound(SoundID.NPCHit53, Projectile.Center);
                CalamityGlobalProjectile.ExpandHitboxBy(Projectile, 20);
                for (int d = 0; d < 5; d++)
                {
                    int ecto = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0f, 0f, 100, default, 0.5f);
                    Main.dust[ecto].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[ecto].scale = 0.5f;
                        Main.dust[ecto].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int d = 0; d < 10; d++)
                {
                    int ecto = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 206, 0f, 0f, 100, default, 1f);
                    Main.dust[ecto].noGravity = true;
                    Main.dust[ecto].velocity *= 5f;
                    ecto = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 206, 0f, 0f, 100, default, 0.5f);
                    Main.dust[ecto].velocity *= 2f;
                }
                initialized = true;
            }
            if (Projectile.timeLeft % 50 == 1 && Projectile.alpha <= 0)
            {
                SoundEngine.PlaySound(SoundID.NPCHit53, Projectile.Center);
                CalamityGlobalProjectile.ExpandHitboxBy(Projectile, 20);
                for (int d = 0; d < 5; d++)
                {
                    int ecto = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 229, 0f, 0f, 100, default, 0.5f);
                    Main.dust[ecto].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[ecto].scale = 0.5f;
                        Main.dust[ecto].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int d = 0; d < 10; d++)
                {
                    int ecto = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 206, 0f, 0f, 100, default, 1f);
                    Main.dust[ecto].noGravity = true;
                    Main.dust[ecto].velocity *= 5f;
                    ecto = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 206, 0f, 0f, 100, default, 0.5f);
                    Main.dust[ecto].velocity *= 2f;
                }
                float spread = 45f * 0.0174f;
                double startAngle = Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y) - spread / 2;
                double deltaAngle = spread / 8f;
                double offsetAngle;
                if (Projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<GraniteEnergy>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                        Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<GraniteEnergy>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int height = texture.Height / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, height / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? CanDamage() => false;
    }
}
