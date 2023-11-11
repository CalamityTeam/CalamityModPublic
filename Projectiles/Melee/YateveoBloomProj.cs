using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class YateveoBloomProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Vector2 projDirection = Main.player[Projectile.owner].MountedCenter - Projectile.Center;
            Projectile.rotation = projDirection.ToRotation() - 1.57f;
            float distance = projDirection.Length();

            if (Main.player[Projectile.owner].dead)
            {
                Projectile.Kill();
                return;
            }

            Main.player[Projectile.owner].itemAnimation = 10;
            Main.player[Projectile.owner].itemTime = 10;

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

            if (Projectile.ai[0] == 0f)
            {
                Projectile.tileCollide = true;
                if (distance > 210f)
                {
                    Projectile.ai[0] = 1f;
                    Projectile.netUpdate = true;
                }
                else if (!Main.player[Projectile.owner].channel)
                {
                    if (Projectile.velocity.Y < 0f)
                        Projectile.velocity.Y = Projectile.velocity.Y * 0.9f;

                    Projectile.velocity.Y = Projectile.velocity.Y + 1f;
                    Projectile.velocity.X = Projectile.velocity.X * 0.9f;
                }
            }
            else
            {
                float meleeSpeedDistance = 14f / Main.player[Projectile.owner].GetAttackSpeed<MeleeDamageClass>() * 1.25f;
                float meleeDamage = 0.9f / Main.player[Projectile.owner].GetAttackSpeed<MeleeDamageClass>() * 1.25f;

                if (Projectile.ai[1] == 1f)
                    Projectile.tileCollide = false;

                if (!Main.player[Projectile.owner].channel || distance > 375f || !Projectile.tileCollide)
                {
                    Projectile.ai[1] = 1f;

                    if (Projectile.tileCollide)
                        Projectile.netUpdate = true;

                    Projectile.tileCollide = false;

                    if (distance < 20f)
                        Projectile.Kill();
                }

                if (!Projectile.tileCollide)
                    meleeDamage *= 2f;

                if (distance > 80f || !Projectile.tileCollide)
                {
                    distance = meleeSpeedDistance / distance;
                    projDirection.X *= distance;
                    projDirection.Y *= distance;

                    float projXDirect = projDirection.X - Projectile.velocity.X;
                    float projYDirect = projDirection.Y - Projectile.velocity.Y;
                    float projDistance = (float)Math.Sqrt((double)(projXDirect * projXDirect + projYDirect * projYDirect));

                    projDistance = meleeDamage / projDistance;
                    projXDirect *= projDistance;
                    projYDirect *= projDistance;

                    Projectile.velocity.X = Projectile.velocity.X * 0.98f;
                    Projectile.velocity.Y = Projectile.velocity.Y * 0.98f;
                    Projectile.velocity.X = Projectile.velocity.X + projXDirect;
                    Projectile.velocity.Y = Projectile.velocity.Y + projYDirect;
                }
                else
                {
                    if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 6f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X * 0.96f;
                        Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
                    }

                    if (Main.player[Projectile.owner].velocity.X == 0f)
                        Projectile.velocity.X = Projectile.velocity.X * 0.96f;
                }
            }

            if (Main.rand.NextBool(3))
            {
                int dustType = Main.rand.Next(5);
                switch (dustType)
                {
                    case 0:
                        dustType = 2;
                        break;
                    case 1:
                        dustType = 44;
                        break;
                    case 2:
                    case 3:
                    case 4:
                        dustType = 136;
                        break;
                    default:
                        break;
                }
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, dustType, 0f, 0f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.5f;
            }

            if (Projectile.alpha == 255 && distance > 10f)
                Projectile.alpha = 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool hitTile = false;
            if (oldVelocity.X != Projectile.velocity.X)
            {
                hitTile = Math.Abs(oldVelocity.X) > 4f;

                Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                Projectile.velocity.X = -oldVelocity.X * 0.2f;
            }
            if (oldVelocity.Y != Projectile.velocity.Y)
            {
                hitTile = Math.Abs(oldVelocity.Y) > 4f;

                Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                Projectile.velocity.Y = -oldVelocity.Y * 0.2f;
            }

            Projectile.ai[0] = 1f;

            if (hitTile)
            {
                Projectile.netUpdate = true;
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Texture2D texture2D2 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/YateveoBloomChain").Value;
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
            target.AddBuff(BuffID.Poisoned, 180);
        }
    }
}
