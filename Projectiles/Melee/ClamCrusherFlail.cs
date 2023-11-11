using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.NPCs.SunkenSea;

namespace CalamityMod.Projectiles.Melee
{
    public class ClamCrusherFlail : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public int finalDamage;

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] == 1f)
            {
                finalDamage = Projectile.damage * 4;
            }
            if (Projectile.ai[1] >= 5f && (Projectile.ai[1] <= 10f))
            {
                for (int i = 0; i < 10; i++)
                {
                    float shortXVel = Projectile.velocity.X / 3f * (float)i;
                    float shortYVel = Projectile.velocity.Y / 3f * (float)i;
                    int dustPos = 4;
                    int waterDust = Dust.NewDust(new Vector2(Projectile.position.X + (float)dustPos, Projectile.position.Y + (float)dustPos), Projectile.width - dustPos * 2, Projectile.height - dustPos * 2, 33, 0f, 0f, 0, new Color(0, 142, 255), 1.5f);
                    Dust waterdust = Main.dust[waterDust];
                    waterdust.noGravity = true;
                    waterdust.velocity *= 0.1f;
                    waterdust.velocity += Projectile.velocity * 0.1f;
                    waterdust.position.X -= shortXVel;
                    waterdust.position.Y -= shortYVel;
                }
            }
            if (Projectile.ai[1] == 5f)
            {
                Projectile.tileCollide = true;
            }
            Vector2 flailDirection = Main.player[Projectile.owner].Center - Projectile.Center;
            Projectile.rotation = flailDirection.ToRotation() - 1.57f;
            if (Main.player[Projectile.owner].dead)
            {
                Projectile.Kill();
                return;
            }
            Main.player[Projectile.owner].itemAnimation = 10;
            Main.player[Projectile.owner].itemTime = 10;
            if (flailDirection.X < 0f)
            {
                Main.player[Projectile.owner].ChangeDir(1);
                Projectile.direction = 1;
            }
            else
            {
                Main.player[Projectile.owner].ChangeDir(-1);
                Projectile.direction = -1;
            }
            Main.player[Projectile.owner].itemRotation = (flailDirection * -1f * (float)Projectile.direction).ToRotation();
            Projectile.spriteDirection = (flailDirection.X > 0f) ? -1 : 1;
            if (Projectile.ai[1] >= 45f && (Projectile.ai[0] != 1f || Projectile.ai[0] != 2f))
            {
                Projectile.velocity.Y += 1f;
                Projectile.velocity.X *= 0.995f;
                Projectile.damage = finalDamage;
            }
            if (Projectile.ai[0] == 0f && flailDirection.Length() > 1000f)
            {
                Projectile.ai[0] = 1f;
            }
            if (Projectile.ai[0] == 1f || Projectile.ai[0] == 2f)
            {
                float flailDistance = flailDirection.Length();
                if (flailDistance > 1500f)
                {
                    Projectile.Kill();
                    return;
                }
                if (flailDistance > 600f)
                {
                    Projectile.ai[0] = 2f;
                }
                Projectile.tileCollide = false;
                float flailSpeed = 20f;
                if (Projectile.ai[0] == 2f)
                {
                    flailSpeed = 40f;
                }
                Projectile.velocity = Vector2.Normalize(flailDirection) * flailSpeed;
                if (flailDirection.Length() < flailSpeed)
                {
                    Projectile.Kill();
                    return;
                }
            }
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] > 5f)
            {
                Projectile.alpha = 0;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[1] >= 5f)
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
                SoundEngine.PlaySound(GiantClam.SlamSound, Projectile.position);
                for (int i = 0; i < 50; i++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    int waterDust = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y), Projectile.width / 2, Projectile.height / 2, 33, velocity.X, velocity.Y, 0, new Color(0, 142, 255), 1.5f);
                    Main.dust[waterDust].velocity *= 2f;
                }
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Texture2D texture2D2 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/ClamCrusherChain").Value;
            Vector2 projCenter = Projectile.Center;
            Rectangle? sourceRectangle = null;
            Vector2 origin = new Vector2((float)texture2D2.Width * 0.5f, (float)texture2D2.Height * 0.5f);
            float projHeight = (float)texture2D2.Height;
            Vector2 actualCenter = mountedCenter - projCenter;
            float flailRotate = (float)Math.Atan2((double)actualCenter.Y, (double)actualCenter.X) - 1.57f;
            bool isActive = true;
            if (float.IsNaN(projCenter.X) && float.IsNaN(projCenter.Y))
            {
                isActive = false;
            }
            if (float.IsNaN(actualCenter.X) && float.IsNaN(actualCenter.Y))
            {
                isActive = false;
            }
            while (isActive)
            {
                if (actualCenter.Length() < projHeight + 1f)
                {
                    isActive = false;
                }
                else
                {
                    Vector2 value2 = actualCenter;
                    value2.Normalize();
                    projCenter += value2 * projHeight;
                    actualCenter = mountedCenter - projCenter;
                    Color colorArea = Lighting.GetColor((int)projCenter.X / 16, (int)(projCenter.Y / 16f));
                    Main.spriteBatch.Draw(texture2D2, projCenter - Main.screenPosition, sourceRectangle, colorArea, flailRotate, origin, 1f, SpriteEffects.None, 0);
                }
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] >= 45f && (Projectile.ai[0] != 1f || Projectile.ai[0] != 2f))
                target.AddBuff(ModContent.BuffType<Eutrophication>(), 120);
            else
                target.AddBuff(ModContent.BuffType<Eutrophication>(), 60);

            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;
            SoundEngine.PlaySound(GiantClam.SlamSound, Projectile.position);
        }
    }
}
