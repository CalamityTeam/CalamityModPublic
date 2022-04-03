using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class Nanomachine : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bullet");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
            aiType = ProjectileID.Bullet;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.localAI[1] == 0)
                Projectile.localAI[1] = Main.rand.Next(1, 3);
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/Nanomachine");
            if (Projectile.localAI[1] == 2)
                texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/Nanomachine2");
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1, texture);
            return false;
        }

        public override void AI()
        {
            //Animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 2)
            {
                Projectile.frame = 0;
            }

            //Rotation
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * Projectile.direction;


            Vector2 dspeed = -Projectile.velocity * Main.rand.NextFloat(0.5f, 0.8f);
            float x2 = Projectile.Center.X - Projectile.velocity.X / 10f;
            float y2 = Projectile.Center.Y - Projectile.velocity.Y / 10f;
            int num137 = Dust.NewDust(new Vector2(x2, y2), 1, 1, 229, 0f, 0f, 0, default, 0.8f);
            Main.dust[num137].alpha = Projectile.alpha;
            Main.dust[num137].position.X = x2;
            Main.dust[num137].position.Y = y2;
            Main.dust[num137].velocity = dspeed;
            Main.dust[num137].noGravity = true;

            float num138 = (float)Math.Sqrt((double)(Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y));
            float num139 = Projectile.localAI[0];
            if (num139 == 0f)
            {
                Projectile.localAI[0] = num138;
                num139 = num138;
            }
            float num140 = Projectile.position.X;
            float num141 = Projectile.position.Y;
            float num142 = 800f;
            bool flag4 = false;
            int num143 = 0;
            if (Projectile.ai[1] == 0f)
            {
                for (int num144 = 0; num144 < 200; num144++)
                {
                    if (Main.npc[num144].CanBeChasedBy(Projectile, false) && (Projectile.ai[1] == 0f || Projectile.ai[1] == (float)(num144 + 1)))
                    {
                        float num145 = Main.npc[num144].position.X + (float)(Main.npc[num144].width / 2);
                        float num146 = Main.npc[num144].position.Y + (float)(Main.npc[num144].height / 2);
                        float num147 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num145) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num146);
                        if (num147 < num142 && Collision.CanHit(new Vector2(Projectile.position.X + (float)(Projectile.width / 2), Projectile.position.Y + (float)(Projectile.height / 2)), 1, 1, Main.npc[num144].position, Main.npc[num144].width, Main.npc[num144].height))
                        {
                            num142 = num147;
                            num140 = num145;
                            num141 = num146;
                            flag4 = true;
                            num143 = num144;
                        }
                    }
                }
                if (flag4)
                {
                    Projectile.ai[1] = (float)(num143 + 1);
                }
                flag4 = false;
            }
            if (Projectile.ai[1] > 0f)
            {
                int num148 = (int)(Projectile.ai[1] - 1f);
                if (Main.npc[num148].active && Main.npc[num148].CanBeChasedBy(Projectile, true) && !Main.npc[num148].dontTakeDamage)
                {
                    float num149 = Main.npc[num148].position.X + (float)(Main.npc[num148].width / 2);
                    float num150 = Main.npc[num148].position.Y + (float)(Main.npc[num148].height / 2);
                    float num151 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num149) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num150);
                    if (num151 < 1000f)
                    {
                        flag4 = true;
                        num140 = Main.npc[num148].position.X + (float)(Main.npc[num148].width / 2);
                        num141 = Main.npc[num148].position.Y + (float)(Main.npc[num148].height / 2);
                    }
                }
                else
                {
                    Projectile.ai[1] = 0f;
                }
            }
            if (!Projectile.friendly)
            {
                flag4 = false;
            }
            if (flag4)
            {
                float num152 = num139;
                Vector2 vector13 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float num153 = num140 - vector13.X;
                float num154 = num141 - vector13.Y;
                float num155 = (float)Math.Sqrt((double)(num153 * num153 + num154 * num154));
                num155 = num152 / num155;
                num153 *= num155;
                num154 *= num155;
                int num156 = 12;
                Projectile.velocity.X = (Projectile.velocity.X * (float)(num156 - 1) + num153) / (float)num156;
                Projectile.velocity.Y = (Projectile.velocity.Y * (float)(num156 - 1) + num154) / (float)num156;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Confused, 180);
        }
    }
}
