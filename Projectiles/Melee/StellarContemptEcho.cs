using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class StellarContemptEcho : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public static readonly SoundStyle SlamHamSound = new("CalamityMod/Sounds/Item/StellarContemptImpact") { Volume = 0.6f };
        public static readonly SoundStyle Kunk = new("CalamityMod/Sounds/Item/TF2PanHit") { Volume = 1.1f };
        public float rotatehammer = 35f;
        public int ColorAlpha = 225;
        public float speed = 0f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        // This is all copied straight from PwnagehammerEcho with some minor edits.
        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 78;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(64, 224, 208, ColorAlpha);
        }

        public override void AI()
        {
            Projectile.scale = 1.8f;
            ColorAlpha -= 4;
            rotatehammer--;
            Projectile.rotation += MathHelper.ToRadians(rotatehammer) * Projectile.direction;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] < 42f)
            {
                Projectile.velocity *= 0.95f;
                Projectile.rotation += MathHelper.ToRadians(Projectile.ai[0] * 0.5f) * Projectile.localAI[0];
            }
            else if (Projectile.ai[0] > 42f)
            {
                Projectile.extraUpdates = 1;
                if (Projectile.ai[1] < 0f)
                {
                    CalamityUtils.HomeInOnNPC(Projectile, Projectile.tileCollide, 2000f, speed, 12f);
                    if (Projectile.ai[0] > 80f)
                        Projectile.Kill();
                    return;
                }

                NPC target = Main.npc[(int)Projectile.ai[1]];
                if (!target.CanBeChasedBy(Projectile, false) || !target.active)
                {
                    Projectile.Kill();
                }
                else
                {
                    float velConst = 7f;
                    velConst--;
                    Projectile.velocity = new Vector2((target.Center.X - Projectile.Center.X) / velConst, (target.Center.Y - Projectile.Center.Y) / velConst);
                    Projectile.rotation += MathHelper.ToRadians(48f) * Projectile.localAI[0];
                }
            }
            

            if (Main.rand.NextBool())
            {
                Vector2 offset = new Vector2(7, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(3, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.Terragrim, new Vector2(Projectile.velocity.X * 0.2f + velOffset.X, Projectile.velocity.Y * 0.2f + velOffset.Y), 100);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(2.2f, 3.6f);
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 offset = new Vector2(7, 0).RotatedByRandom(MathHelper.ToRadians(360f));
                Vector2 velOffset = new Vector2(3, 0).RotatedBy(offset.ToRotation());
                Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X, Projectile.Center.Y) + offset, DustID.Vortex, new Vector2(Projectile.velocity.X * 0.5f + velOffset.X, Projectile.velocity.Y * 0.5f + velOffset.Y), 100);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(2.2f, 3.6f);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] <= 42f)
                return false;

            return null;
        }

        public override bool CanHitPvp(Player target) => Projectile.ai[0] > 42f;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.localAI[0] = target.whoAmI;
        }

        public override bool PreKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            // This is what we call fucking IMPACT (2).
            Main.player[Projectile.owner].Calamity().GeneralScreenShakePower = 7;

            if (Main.zenithWorld)
                SoundEngine.PlaySound(Kunk, Projectile.Center);
            else
                SoundEngine.PlaySound(SlamHamSound, Projectile.Center);

            float numberOfDusts = 156f;
            float rotFactor = 360f / numberOfDusts;
            for (int i = 0; i < numberOfDusts; i++)
            {
                float rot = MathHelper.ToRadians(i * rotFactor);
                Vector2 offset = new Vector2(30f, 5.8f).RotatedBy(rot);
                Vector2 velOffset = new Vector2(20.8f, 10.5f).RotatedBy(rot);
                int dust = Dust.NewDust(Projectile.position + offset, Projectile.width, Projectile.height, 229, velOffset.X, velOffset.Y);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = velOffset;
                Main.dust[dust].scale = 3.8f;
            }

            for (int i = 0; i < 88; ++i)
            {
                // Pick a random type of dust.
                int dustID;
                switch (Main.rand.Next(6))
                {
                    case 0:
                        dustID = 229;
                        break;
                    case 1:
                    case 2:
                        dustID = 156;
                        break;
                    default:
                        dustID = 156;
                        break;
                }

                // Choose a random speed and angle for the dust.
                float dustSpeed = Main.rand.NextFloat(6.0f, 29.0f);
                float angleRandom = 0.09f;
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                // Pick a size for the dust particle.
                float scale = Main.rand.NextFloat(2.2f, 4.8f);

                // Actually spawn the dust.
                int idx = Dust.NewDust(Projectile.Center, 1, 1, dustID, dustVel.X, dustVel.Y, 0, default, scale);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].position = Projectile.Center;
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 0f, ModContent.ProjectileType<StellarContemptBlast>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0f);

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width + 12, texture.Height + 12);
            Vector2 origin = sourceRectangle.Size() / 2f;

            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float rot = MathHelper.ToRadians(22.5f) * Math.Sign(Projectile.velocity.X);
                Vector2 drawPos = Projectile.oldPos[i] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(), color, Projectile.rotation - i * rot, origin, Projectile.scale, spriteEffects, 0);
            }

            return false;
        }
    }
}
