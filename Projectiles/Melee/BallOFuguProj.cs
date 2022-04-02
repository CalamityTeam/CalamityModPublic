using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 projVector = player.Center - projectile.Center;
            projectile.rotation = projVector.ToRotation() - 1.57f;
            if (player.dead)
            {
                projectile.Kill();
                return;
            }
            player.itemAnimation = 10;
            player.itemTime = 10;
            if (projVector.X < 0f)
            {
                player.ChangeDir(1);
                projectile.direction = 1;
            }
            else
            {
                player.ChangeDir(-1);
                projectile.direction = -1;
            }
            player.itemRotation = (projVector * -1f * (float)projectile.direction).ToRotation();
            projectile.spriteDirection = (projVector.X > 0f) ? -1 : 1;
            if (projectile.ai[0] == 0f && projVector.Length() > 400f)
            {
                projectile.ai[0] = 1f;
            }
            if (projectile.ai[0] == 1f || projectile.ai[0] == 2f)
            {
                float playerDist = projVector.Length();
                if (playerDist > 1500f)
                {
                    projectile.Kill();
                    return;
                }
                if (playerDist > 600f)
                {
                    projectile.ai[0] = 2f;
                }
                projectile.tileCollide = false;
                float num694 = 20f;
                if (projectile.ai[0] == 2f)
                {
                    num694 = 40f;
                }
                projectile.velocity = Vector2.Normalize(projVector) * num694;
                if (projVector.Length() < num694)
                {
                    projectile.Kill();
                    return;
                }
            }
            projectile.ai[1] += 1f;
            if (projectile.ai[1] > 5f)
            {
                projectile.alpha = 0;
            }
            if (projectile.ai[1] % 6f == 0f && projectile.owner == Main.myPlayer)
            {
                Vector2 vector63 = projVector * -1f;
                vector63.Normalize();
                vector63 *= (float)Main.rand.Next(45, 65) * 0.1f;
                vector63 = vector63.RotatedBy((Main.rand.NextDouble() - 0.5) * 1.5707963705062866, default);
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vector63.X, vector63.Y, ModContent.ProjectileType<UrchinSpikeFugu>(), (int)(projectile.damage * 0.6), projectile.knockBack * 0.2f, projectile.owner, -10f, 0f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            projectile.ai[0] = 1f;
            projectile.netUpdate = true;
            Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
            Color transparent = Microsoft.Xna.Framework.Color.Transparent;
            Texture2D texture2D2 = ModContent.GetTexture("CalamityMod/ExtraTextures/Chains/BallOFuguChain");
            Vector2 vector17 = projectile.Center;
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
                    Main.spriteBatch.Draw(texture2D2, vector17 - Main.screenPosition, sourceRectangle, color17, rotation15, origin, 1f, SpriteEffects.None, 0f);
                }
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 180);
            projectile.ai[0] = 1f;
            projectile.netUpdate = true;
        }
    }
}
