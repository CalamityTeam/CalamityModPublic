using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class UrchinBall : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public bool spike = false;

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 projDirection = player.Center - Projectile.Center;
            Projectile.rotation = projDirection.ToRotation() - 1.57f;
            if (player.dead)
            {
                Projectile.Kill();
                return;
            }
            player.itemAnimation = 10;
            player.itemTime = 10;
            if (projDirection.X < 0f)
            {
                player.ChangeDir(1);
                Projectile.direction = 1;
            }
            else
            {
                player.ChangeDir(-1);
                Projectile.direction = -1;
            }
            player.itemRotation = (projDirection * -1f * (float)Projectile.direction).ToRotation();
            Projectile.spriteDirection = (projDirection.X > 0f) ? -1 : 1;
            if (Projectile.ai[0] == 0f && projDirection.Length() > 400f)
            {
                Projectile.ai[0] = 1f;
            }
            if (Projectile.ai[0] == 1f || Projectile.ai[0] == 2f)
            {
                if (spike)
                {
                    spike = false;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Projectile.velocity.X, Projectile.velocity.Y, ModContent.ProjectileType<UrchinBallSpike>(), (int)((double)Projectile.damage * 0.5), 0f, Main.myPlayer, 0f, 0f);
                }
                Projectile.usesLocalNPCImmunity = false;
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
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Texture2D texture2D2 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/UrchinFlailChain").Value;
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
            spike = true;
            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;
            target.AddBuff(BuffID.Poisoned, 180);
        }
    }
}
