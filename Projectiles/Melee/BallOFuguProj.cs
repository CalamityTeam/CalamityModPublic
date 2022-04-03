using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class BallOFuguProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ball O Fugu");
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
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
                float num694 = 20f;
                if (Projectile.ai[0] == 2f)
                {
                    num694 = 40f;
                }
                Projectile.velocity = Vector2.Normalize(projVector) * num694;
                if (projVector.Length() < num694)
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
                Vector2 vector63 = projVector * -1f;
                vector63.Normalize();
                vector63 *= (float)Main.rand.Next(45, 65) * 0.1f;
                vector63 = vector63.RotatedBy((Main.rand.NextDouble() - 0.5) * 1.5707963705062866, default);
                Projectile.NewProjectile(Projectile.Center.X, Projectile.Center.Y, vector63.X, vector63.Y, ModContent.ProjectileType<UrchinSpikeFugu>(), (int)(Projectile.damage * 0.6), Projectile.knockBack * 0.2f, Projectile.owner, -10f, 0f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;
            SoundEngine.PlaySound(SoundID.Dig, (int)Projectile.position.X, (int)Projectile.position.Y, 1, 1f, 0f);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Color transparent = Microsoft.Xna.Framework.Color.Transparent;
            Texture2D texture2D2 = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Chains/BallOFuguChain");
            Vector2 vector17 = Projectile.Center;
            Rectangle? sourceRectangle = null;
            Vector2 origin = new Vector2((float)texture2D2.Width * 0.5f, (float)texture2D2.Height * 0.5f);
            float num91 = (float)texture2D2.Height;
            Vector2 vector18 = mountedCenter - vector17;
            float rotation15 = (float)Math.Atan2((double)vector18.Y, (double)vector18.X) - 1.57f;
            bool flag13 = true;
            if (float.IsNaN(vector17.X) && float.IsNaN(vector17.Y))
            {
                flag13 = false;
            }
            if (float.IsNaN(vector18.X) && float.IsNaN(vector18.Y))
            {
                flag13 = false;
            }
            while (flag13)
            {
                if (vector18.Length() < num91 + 1f)
                {
                    flag13 = false;
                }
                else
                {
                    Vector2 value2 = vector18;
                    value2.Normalize();
                    vector17 += value2 * num91;
                    vector18 = mountedCenter - vector17;
                    Color color17 = Lighting.GetColor((int)vector17.X / 16, (int)(vector17.Y / 16f));
                    Main.spriteBatch.Draw(texture2D2, vector17 - Main.screenPosition, sourceRectangle, color17, rotation15, origin, 1f, SpriteEffects.None, 0);
                }
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 180);
            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;
        }
    }
}
