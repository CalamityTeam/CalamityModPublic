using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Melee
{
    public class TumbleweedFlail : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Vector2 projDirection = Main.player[Projectile.owner].Center - Projectile.Center;
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;
            if (Main.player[Projectile.owner].dead)
            {
                Projectile.Kill();
                return;
            }
            Main.player[Projectile.owner].itemAnimation = 10;
            Main.player[Projectile.owner].itemTime = 10;
            float arg_1DC8F_0 = projDirection.X;
            if (projDirection.X < 0f)
            {
                Main.player[Projectile.owner].ChangeDir(1);
                Projectile.direction = 1;
            }
            else
            {
                Main.player[Projectile.owner].ChangeDir(-1);
                Projectile.direction = -1;
            }
            Main.player[Projectile.owner].itemRotation = (projDirection * -1f * (float)Projectile.direction).ToRotation();
            Projectile.spriteDirection = (projDirection.X > 0f) ? -1 : 1;
            if (Projectile.ai[0] == 0f && projDirection.Length() > 400f)
            {
                Projectile.ai[0] = 1f;
            }
            if (Projectile.ai[0] == 1f || Projectile.ai[0] == 2f)
            {
                float projDistance = projDirection.Length();
                if (projDistance > 1500f)
                {
                    Projectile.Kill();
                    return;
                }
                if (projDistance > 600f)
                {
                    Projectile.ai[0] = 2f;
                }
                Projectile.tileCollide = false;
                float returnLength = 20f;
                if (Projectile.ai[0] == 2f)
                {
                    returnLength = 40f;
                }
                Projectile.velocity = Vector2.Normalize(projDirection) * returnLength;
                if (projDirection.Length() < returnLength)
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
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;
            SoundEngine.PlaySound(SoundID.NPCHit11, Projectile.position);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Texture2D texture2D2 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/TumbleweedChain").Value;
            Vector2 projCenter = Projectile.Center;
            Rectangle? sourceRectangle = null;
            Vector2 origin = new Vector2((float)texture2D2.Width * 0.5f, (float)texture2D2.Height * 0.5f);
            float projHeight = (float)texture2D2.Height;
            Vector2 actualCenter = mountedCenter - projCenter;
            float projRotation = (float)Math.Atan2((double)actualCenter.Y, (double)actualCenter.X) - 1.57f;
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
                    Vector2 centerCopy = actualCenter;
                    centerCopy.Normalize();
                    projCenter += centerCopy * projHeight;
                    actualCenter = mountedCenter - projCenter;
                    Color drawArea = Lighting.GetColor((int)projCenter.X / 16, (int)(projCenter.Y / 16f));
                    Main.spriteBatch.Draw(texture2D2, projCenter - Main.screenPosition, sourceRectangle, drawArea, projRotation, origin, 1f, SpriteEffects.None, 0);
                }
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath15, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                int tumbleDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 32, 0f, 0f, 100, default, 1.2f);
                Main.dust[tumbleDust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[tumbleDust].scale = 0.5f;
                    Main.dust[tumbleDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 30; j++)
            {
                int tumbleDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 85, 0f, 0f, 100, default, 1.7f);
                Main.dust[tumbleDust2].noGravity = true;
                Main.dust[tumbleDust2].velocity *= 5f;
                tumbleDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 85, 0f, 0f, 100, default, 1f);
                Main.dust[tumbleDust2].velocity *= 2f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, ModContent.ProjectileType<TumbleweedRolling>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, 0f, 0f);
            }
            Projectile.Kill();
        }
    }
}
