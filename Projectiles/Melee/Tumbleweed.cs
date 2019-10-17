using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class Tumbleweed : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tumbleweed");
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
            projectile.Calamity().trueMelee = true;
        }

        public override void AI()
        {
            Vector2 vector62 = Main.player[projectile.owner].Center - projectile.Center;
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
            if (Main.player[projectile.owner].dead)
            {
                projectile.Kill();
                return;
            }
            Main.player[projectile.owner].itemAnimation = 10;
            Main.player[projectile.owner].itemTime = 10;
            float arg_1DC8F_0 = vector62.X;
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
            Main.player[projectile.owner].itemRotation = (vector62 * -1f * (float)projectile.direction).ToRotation();
            projectile.spriteDirection = (vector62.X > 0f) ? -1 : 1;
            if (projectile.ai[0] == 0f && vector62.Length() > 400f)
            {
                projectile.ai[0] = 1f;
            }
            if (projectile.ai[0] == 1f || projectile.ai[0] == 2f)
            {
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
            float[] var_2_1DE21_cp_0 = projectile.ai;
            int var_2_1DE21_cp_1 = 1;
            float num73 = var_2_1DE21_cp_0[var_2_1DE21_cp_1];
            var_2_1DE21_cp_0[var_2_1DE21_cp_1] = num73 + 1f;
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
            Main.PlaySound(3, (int)projectile.position.X, (int)projectile.position.Y, 11);
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 mountedCenter = Main.player[projectile.owner].MountedCenter;
            Color transparent = Microsoft.Xna.Framework.Color.Transparent;
            Texture2D texture2D2 = mod.GetTexture("ExtraTextures/Chains/TumbleweedChain");
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
            Main.PlaySound(4, (int)projectile.position.X, (int)projectile.position.Y, 15);
            for (int num621 = 0; num621 < 20; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 32, 0f, 0f, 100, default, 1.2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 30; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 85, 0f, 0f, 100, default, 1.7f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 85, 0f, 0f, 100, default, 1f);
                Main.dust[num624].velocity *= 2f;
            }
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<TumbleweedRolling>(), projectile.damage, projectile.knockBack, Main.myPlayer, 0f, 0f);
            }
            projectile.Kill();
        }
    }
}
