using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Melee
{
    public class YateveoBloom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yateveo Bloom");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
            projectile.alpha = 255;
            projectile.Calamity().trueMelee = true;
        }

        public override void AI()
        {
            Vector2 vector62 = Main.player[projectile.owner].MountedCenter - projectile.Center;
            projectile.rotation = vector62.ToRotation() - 1.57f;
            float distance = vector62.Length();

            if (Main.player[projectile.owner].dead)
            {
                projectile.Kill();
                return;
            }

            Main.player[projectile.owner].itemAnimation = 10;
            Main.player[projectile.owner].itemTime = 10;

            if (vector62.X < 0f)
            {
                Main.player[projectile.owner].ChangeDir(1);
                projectile.direction = 1;
            }
            else
            {
                Main.player[projectile.owner].ChangeDir(-1);
                projectile.direction = -1;
            }

            if (projectile.ai[0] == 0f)
            {
                projectile.tileCollide = true;
                if (distance > 210f)
                {
                    projectile.ai[0] = 1f;
                    projectile.netUpdate = true;
                }
                else if (!Main.player[projectile.owner].channel)
                {
                    if (projectile.velocity.Y < 0f)
                        projectile.velocity.Y = projectile.velocity.Y * 0.9f;

                    projectile.velocity.Y = projectile.velocity.Y + 1f;
                    projectile.velocity.X = projectile.velocity.X * 0.9f;
                }
            }
            else
            {
                float num211 = 14f / Main.player[projectile.owner].meleeSpeed * 1.25f;
                float num212 = 0.9f / Main.player[projectile.owner].meleeSpeed * 1.25f;

                if (projectile.ai[1] == 1f)
                    projectile.tileCollide = false;

                if (!Main.player[projectile.owner].channel || distance > 375f || !projectile.tileCollide)
                {
                    projectile.ai[1] = 1f;

                    if (projectile.tileCollide)
                        projectile.netUpdate = true;

                    projectile.tileCollide = false;

                    if (distance < 20f)
                        projectile.Kill();
                }

                if (!projectile.tileCollide)
                    num212 *= 2f;

                if (distance > 80f || !projectile.tileCollide)
                {
                    distance = num211 / distance;
                    vector62.X *= distance;
                    vector62.Y *= distance;

                    Vector2 vector21 = new Vector2(projectile.velocity.X, projectile.velocity.Y);
                    float num217 = vector62.X - projectile.velocity.X;
                    float num218 = vector62.Y - projectile.velocity.Y;
                    float num219 = (float)Math.Sqrt((double)(num217 * num217 + num218 * num218));

                    num219 = num212 / num219;
                    num217 *= num219;
                    num218 *= num219;

                    projectile.velocity.X = projectile.velocity.X * 0.98f;
                    projectile.velocity.Y = projectile.velocity.Y * 0.98f;
                    projectile.velocity.X = projectile.velocity.X + num217;
                    projectile.velocity.Y = projectile.velocity.Y + num218;
                }
                else
                {
                    if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 6f)
                    {
                        projectile.velocity.X = projectile.velocity.X * 0.96f;
                        projectile.velocity.Y = projectile.velocity.Y + 0.2f;
                    }

                    if (Main.player[projectile.owner].velocity.X == 0f)
                        projectile.velocity.X = projectile.velocity.X * 0.96f;
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
                int dust = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, dustType, 0f, 0f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.5f;
            }

            if (projectile.alpha == 255 && distance > 10f)
                projectile.alpha = 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool hitTile = false;
            if (oldVelocity.X != projectile.velocity.X)
            {
                hitTile = Math.Abs(oldVelocity.X) > 4f;

                projectile.position.X = projectile.position.X + projectile.velocity.X;
                projectile.velocity.X = -oldVelocity.X * 0.2f;
            }
            if (oldVelocity.Y != projectile.velocity.Y)
            {
                hitTile = Math.Abs(oldVelocity.Y) > 4f;

                projectile.position.Y = projectile.position.Y + projectile.velocity.Y;
                projectile.velocity.Y = -oldVelocity.Y * 0.2f;
            }

            projectile.ai[0] = 1f;

            if (hitTile)
            {
                projectile.netUpdate = true;
                Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
                Main.PlaySound(0, (int)projectile.position.X, (int)projectile.position.Y, 1, 1f, 0f);
            }

            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
            Color transparent = Microsoft.Xna.Framework.Color.Transparent;
            Texture2D texture2D2 = ModContent.GetTexture("CalamityMod/ExtraTextures/Chains/YateveoBloomChain");
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
            target.AddBuff(BuffID.Poisoned, 180);
            target.AddBuff(BuffID.Venom, 90);
        }
    }
}
