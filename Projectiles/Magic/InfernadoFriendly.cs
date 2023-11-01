using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class InfernadoFriendly : ModProjectile, ILocalizedModType
    {

        public new string LocalizationCategory => "Projectiles.Magic";
        bool intersectingSomething = false;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 320;
            Projectile.height = 88;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 500;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                intersectingSomething = true;

            float scaleBase = 44f;
            float scaleMult = 2.5f;
            float baseWidth = 320f;
            float baseHeight = 88f;

            if (Main.rand.NextBool(25))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 244, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            if (Projectile.velocity.X != 0f)
            {
                Projectile.direction = Projectile.spriteDirection = -Math.Sign(Projectile.velocity.X);
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.scale = (scaleBase - Projectile.ai[1]) * scaleMult / scaleBase;
                Projectile.ExpandHitboxBy((int)(baseWidth * Projectile.scale), (int)(baseHeight * Projectile.scale));
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[1] != -1f)
            {
                Projectile.scale = (scaleBase - Projectile.ai[1]) * scaleMult / scaleBase;
                Projectile.width = (int)(baseWidth * Projectile.scale);
                Projectile.height = (int)(baseHeight * Projectile.scale);
            }
            if (!intersectingSomething)
            {
                Projectile.alpha -= 30;
                if (Projectile.alpha < 100)
                {
                    Projectile.alpha = 100;
                }
            }
            else
            {
                Projectile.alpha += 30;
                if (Projectile.alpha > 200)
                {
                    Projectile.alpha = 200;
                }
            }
            if (Projectile.ai[0] > 0f)
            {
                Projectile.ai[0] -= 1f;
            }
            if (Projectile.ai[0] == 1f && Projectile.ai[1] > 0f && Projectile.owner == Main.myPlayer)
            {
                Projectile.netUpdate = true;
                Vector2 center = Projectile.Center;
                center.Y -= baseHeight * Projectile.scale / 2f;
                float nextSegmentHeight = (scaleBase - Projectile.ai[1] + 1f) * scaleMult / scaleBase;
                center.Y -= baseHeight * nextSegmentHeight / 2f;
                center.Y += 2f;
                Projectile segment = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), center, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, 10f, Projectile.ai[1] - 1f);

                // The projectile defaults to magic, but each sub-segment copies the damage class of the previous.
                // Truly a worm boss of weapons.
                if (segment.whoAmI.WithinBounds(Main.maxProjectiles))
                {
                    segment.DamageType = Projectile.DamageType;
                    segment.friendly = Projectile.friendly;
                    segment.hostile = Projectile.hostile;
                }
            }
            if (Projectile.ai[0] <= 0f)
            {
                float swaySize = 0.104719758f;
                float smolWidth = (float)Projectile.width / 5f;
                smolWidth *= 2f;
                float projXChange = (float)(Math.Cos((double)(swaySize * -(double)Projectile.ai[0])) - 0.5) * smolWidth;
                Projectile.position.X -= projXChange * -Projectile.direction;
                Projectile.ai[0] -= 1f;
                projXChange = (float)(Math.Cos((double)(swaySize * -(double)Projectile.ai[0])) - 0.5) * smolWidth;
                Projectile.position.X += projXChange * -Projectile.direction;
                return;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Dragonfire>(), 300);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (!intersectingSomething)
            {
                return new Color(95, 95, 19, 255 - Projectile.alpha);
            }
            return new Color(64, 64, 13, 255 - Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int framing = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = framing * Projectile.frame;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, framing)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
