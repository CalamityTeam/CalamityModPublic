using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class MechwormHead : ModProjectile
    {
        private int dust = 3;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mechworm");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.netImportant = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 18000;
            projectile.alpha = 255;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Lighting.AddLight((int)((projectile.position.X + (float)(projectile.width / 2)) / 16f), (int)((projectile.position.Y + (float)(projectile.height / 2)) / 16f), 0.15f, 0.01f, 0.15f);
            Player player9 = Main.player[projectile.owner];
            if (dust > 0)
            {
                int num501 = 50;
                for (int num502 = 0; num502 < num501; num502++)
                {
                    int num503 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 234, 0f, 0f, 0, default, 1f);
                    Main.dust[num503].velocity *= 2f;
                    Main.dust[num503].scale *= 1.15f;
                }
                dust--;
            }
            CalamityPlayer modPlayer = player9.Calamity();
            player9.AddBuff(ModContent.BuffType<Mechworm>(), 3600);
            if ((int)Main.time % 120 == 0)
            {
                projectile.netUpdate = true;
            }
            int num1051 = 30;
            if (player9.dead)
            {
                modPlayer.mWorm = false;
            }
            if (modPlayer.mWorm)
            {
                projectile.timeLeft = 2;
            }
            Vector2 center14 = player9.Center;
            float num1053 = 1800f; //700
            float num1054 = 2200f; //1000
            int num1055 = -1;
            if (projectile.Distance(center14) > 3000f) //2000
            {
                projectile.Center = center14;
                projectile.netUpdate = true;
            }
            if (player9.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player9.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false) && player9.Distance(npc.Center) < num1054)
                {
                    float num1057 = projectile.Distance(npc.Center);
                    if (num1057 < num1053)
                    {
                        num1055 = npc.whoAmI;
                    }
                }
            }
            else
            {
                for (int num1056 = 0; num1056 < Main.maxNPCs; num1056++)
                {
                    NPC nPC13 = Main.npc[num1056];
                    if (nPC13.CanBeChasedBy(projectile, false) && player9.Distance(nPC13.Center) < num1054)
                    {
                        float num1057 = projectile.Distance(nPC13.Center);
                        if (num1057 < num1053)
                        {
                            num1055 = num1056;
                        }
                    }
                }
            }
            if (num1055 != -1)
            {
                NPC nPC14 = Main.npc[num1055];
                Vector2 vector132 = nPC14.Center - projectile.Center;
                (vector132.X > 0f).ToDirectionInt();
                (vector132.Y > 0f).ToDirectionInt();
                float scaleFactor16 = 0.3f; //.4
                if (vector132.Length() < 900f)
                {
                    scaleFactor16 = 0.45f; //.5
                }
                if (vector132.Length() < 600f)
                {
                    scaleFactor16 = 0.6f; //.6
                }
                if (vector132.Length() < 300f)
                {
                    scaleFactor16 = 0.8f; //.8
                }
                if (vector132.Length() > nPC14.Size.Length() * 0.75f)
                {
                    projectile.velocity += Vector2.Normalize(vector132) * scaleFactor16 * 1.5f;
                    if (Vector2.Dot(projectile.velocity, vector132) < 0.25f)
                    {
                        projectile.velocity *= 0.8f;
                    }
                }
                float num1058 = 50f; //30
                if (projectile.velocity.Length() > num1058)
                {
                    projectile.velocity = Vector2.Normalize(projectile.velocity) * num1058;
                }
            }
            else
            {
                float num1059 = 0.2f;
                Vector2 vector133 = center14 - projectile.Center;
                if (vector133.Length() < 200f)
                {
                    num1059 = 0.12f;
                }
                if (vector133.Length() < 140f)
                {
                    num1059 = 0.06f;
                }
                if (vector133.Length() > 100f)
                {
                    if (Math.Abs(center14.X - projectile.Center.X) > 20f)
                    {
                        projectile.velocity.X = projectile.velocity.X + num1059 * (float)Math.Sign(center14.X - projectile.Center.X);
                    }
                    if (Math.Abs(center14.Y - projectile.Center.Y) > 10f)
                    {
                        projectile.velocity.Y = projectile.velocity.Y + num1059 * (float)Math.Sign(center14.Y - projectile.Center.Y);
                    }
                }
                else if (projectile.velocity.Length() > 2f)
                {
                    projectile.velocity *= 0.96f;
                }
                if (Math.Abs(projectile.velocity.Y) < 1f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - 0.1f;
                }
                float num1060 = 25f; //15
                if (projectile.velocity.Length() > num1060)
                {
                    projectile.velocity = Vector2.Normalize(projectile.velocity) * num1060;
                }
            }
            projectile.rotation = projectile.velocity.ToRotation() + 1.57079637f;
            int direction = projectile.direction;
            projectile.direction = projectile.spriteDirection = (projectile.velocity.X > 0f) ? 1 : -1;
            if (direction != projectile.direction)
            {
                projectile.netUpdate = true;
            }
            float num1061 = MathHelper.Clamp(projectile.localAI[0], 0f, 50f);
            projectile.position = projectile.Center;
            projectile.netSpam = 5;
            projectile.scale = 1f + num1061 * 0.01f;
            projectile.width = projectile.height = (int)((float)num1051 * projectile.scale);
            projectile.Center = projectile.position;
            projectile.netSpam = 5;
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 42;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
            }
            projectile.netSpam = 5;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 origin = new Vector2(21f, 25f);
            spriteBatch.Draw(ModContent.GetTexture("CalamityMod/Projectiles/Summon/MechwormHeadGlow"), projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
        }
        /// <summary>
        /// Determines the distance factor used in mechworm segments to make it adjust based on components of the head.
        /// <para>However, eventually even this equation will falter in use. These conditions should never happen with the mechworm, however.</para> 
        /// </summary>
        /// <param name="initialDistance">The original distance factor.</param>
        /// <param name="speed">The speed of the mechworm's head.</param>
        /// <param name="base">The base of the exponent used in the equation.</param>
        /// <returns></returns>
        public static float ComputeDistance(float initialDistance, float speed, float @base = 0.98f)
        {
            return initialDistance * (float)Math.Pow(@base, speed);
        }
    }
}
