using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class BallOFuguProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 projVector = player.Center - Projectile.Center;
            Projectile.rotation = projVector.ToRotation() - 1.57f;
            if (player.dead)
            {
                Projectile.Kill();
                return;
            }
            player.itemAnimation = 10;
            player.itemTime = 10;
            if (projVector.X < 0f)
            {
                player.ChangeDir(1);
                Projectile.direction = 1;
            }
            else
            {
                player.ChangeDir(-1);
                Projectile.direction = -1;
            }
            player.itemRotation = (projVector * -1f * (float)Projectile.direction).ToRotation();
            Projectile.spriteDirection = (projVector.X > 0f) ? -1 : 1;
            if (Projectile.ai[0] == 0f && projVector.Length() > 400f)
            {
                Projectile.ai[0] = 1f;
            }
            if (Projectile.ai[0] == 1f || Projectile.ai[0] == 2f)
            {
                float playerDist = projVector.Length();
                if (playerDist > 1500f)
                {
                    Projectile.Kill();
                    return;
                }
                if (playerDist > 600f)
                {
                    Projectile.ai[0] = 2f;
                }
                Projectile.tileCollide = false;
                float returnSpeed = 20f;
                if (Projectile.ai[0] == 2f)
                {
                    returnSpeed = 40f;
                }
                Projectile.velocity = Vector2.Normalize(projVector) * returnSpeed;
                if (projVector.Length() < returnSpeed)
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
            if (Projectile.ai[1] % 6f == 0f && Projectile.owner == Main.myPlayer)
            {
                Vector2 spikeVector = projVector * -1f;
                spikeVector.Normalize();
                spikeVector *= (float)Main.rand.Next(45, 65) * 0.1f;
                spikeVector = spikeVector.RotatedBy((Main.rand.NextDouble() - 0.5) * 1.5707963705062866, default);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, spikeVector.X, spikeVector.Y, ModContent.ProjectileType<UrchinSpikeFugu>(), (int)(Projectile.damage * 0.6), Projectile.knockBack * 0.2f, Projectile.owner, -10f, 0f);
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
            Texture2D texture2D2 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/BallOFuguChain").Value;
            Vector2 projCenter = Projectile.Center;
            Rectangle? sourceRectangle = null;
            Vector2 origin = new Vector2((float)texture2D2.Width * 0.5f, (float)texture2D2.Height * 0.5f);
            float projHeight = (float)texture2D2.Height;
            Vector2 actualCenter = mountedCenter - projCenter;
            float drawRotation = (float)Math.Atan2((double)actualCenter.Y, (double)actualCenter.X) - 1.57f;
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
                    Vector2 drawCenter = actualCenter;
                    drawCenter.Normalize();
                    projCenter += drawCenter * projHeight;
                    actualCenter = mountedCenter - projCenter;
                    Color drawArea = Lighting.GetColor((int)projCenter.X / 16, (int)(projCenter.Y / 16f));
                    Main.spriteBatch.Draw(texture2D2, projCenter - Main.screenPosition, sourceRectangle, drawArea, drawRotation, origin, 1f, SpriteEffects.None, 0);
                }
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 180);
            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;
        }
    }
}
