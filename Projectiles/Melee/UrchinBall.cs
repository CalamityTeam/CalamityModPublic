using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class UrchinBall : ModProjectile
    {
        public bool spike = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("UrchinBall");
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.alpha = 255;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 3;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 vector62 = player.Center - projectile.Center;
            projectile.rotation = vector62.ToRotation() - 1.57f;
            if (player.dead)
            {
                projectile.Kill();
                return;
            }
            player.itemAnimation = 10;
            player.itemTime = 10;
            if (vector62.X < 0f)
            {
                player.ChangeDir(1);
                projectile.direction = 1;
            }
            else
            {
                player.ChangeDir(-1);
                projectile.direction = -1;
            }
            player.itemRotation = (vector62 * -1f * (float)projectile.direction).ToRotation();
            projectile.spriteDirection = (vector62.X > 0f) ? -1 : 1;
            if (projectile.ai[0] == 0f && vector62.Length() > 400f)
            {
                projectile.ai[0] = 1f;
            }
            if (projectile.ai[0] == 1f || projectile.ai[0] == 2f)
            {
                if (spike)
                {
                    spike = false;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<UrchinBallSpike>(), (int)((double)projectile.damage * 0.5), 0f, Main.myPlayer, 0f, 0f);
                }
                projectile.usesLocalNPCImmunity = false;
                float num693 = vector62.Length();
                if (num693 > 1500f)
                {
                    projectile.Kill();
                    return;
                }
                if (num693 > 600f)
                {
                    projectile.ai[0] = 2f;
                }
                projectile.tileCollide = false;
                float num694 = 20f;
                if (projectile.ai[0] == 2f)
                {
                    num694 = 40f;
                }
                projectile.velocity = Vector2.Normalize(vector62) * num694;
                if (vector62.Length() < num694)
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
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
            projectile.ai[0] = 1f;
            projectile.netUpdate = true;
            Main.PlaySound(SoundID.Dig, projectile.position);
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
            Color transparent = Microsoft.Xna.Framework.Color.Transparent;
            Texture2D texture2D2 = ModContent.GetTexture("CalamityMod/ExtraTextures/Chains/UrchinFlailChain");
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
            spike = true;
            projectile.ai[0] = 1f;
            projectile.netUpdate = true;
            target.AddBuff(BuffID.Venom, 180);
        }
    }
}
