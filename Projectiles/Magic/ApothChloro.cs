using Microsoft.Xna.Framework;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class ApothChloro : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rays of Annihilation");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.alpha = 70;
            projectile.timeLeft = 120;
            projectile.penetrate = 1;
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.light = 0.5f;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0.4f, 0.2f, 0.4f);
            for (int i = 0; i < 5; i++)
            {
                Dust dust4 = Main.dust[Dust.NewDust(projectile.position, projectile.width, projectile.height, 242, projectile.velocity.X, projectile.velocity.Y, 100, default, 1f)];
                dust4.velocity = Vector2.Zero;
                dust4.position -= projectile.velocity / 5f * (float)i;
                dust4.noGravity = true;
                dust4.scale = 0.8f;
                dust4.noLight = true;
            }
            float num1 = (float)(projectile.position.X - projectile.velocity.X / 10.0 * 9.0);
            float num2 = (float)(projectile.position.Y - projectile.velocity.Y / 10.0 * 9.0);
            float num3 = (float)Math.Sqrt((double)(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y));
            float num4 = projectile.localAI[0];
            if ((double)num4 == 0.0)
            {
                projectile.localAI[0] = num3;
                num4 = num3;
            }
            if (projectile.alpha > 0)
                projectile.alpha = projectile.alpha - 25;
            if (projectile.alpha < 0)
                projectile.alpha = 0;
            float num5 = (float)projectile.position.X;
            float num6 = (float)projectile.position.Y;
            float num7 = 300f;
            bool flag2 = false;
            int num8 = 0;
            float num9;
            if ((double)projectile.ai[1] == 0.0)
            {
                for (int index = 0; index < 200; ++index)
                {
                    if (Main.npc[index].CanBeChasedBy((object)this, false) && ((double)projectile.ai[1] == 0.0 || (double)projectile.ai[1] == (double)(index + 1)))
                    {
                        num1 = (float)Main.npc[index].position.X + (float)(Main.npc[index].width / 2);
                        num2 = (float)Main.npc[index].position.Y + (float)(Main.npc[index].height / 2);
                        num9 = Math.Abs((float)projectile.position.X + (float)(projectile.width / 2) - num1) + Math.Abs((float)projectile.position.Y + (float)(projectile.height / 2) - num2);
                        if ((double)num9 < (double)num7 && Collision.CanHit(new Vector2((float)projectile.position.X + (float)(projectile.width / 2), (float)projectile.position.Y + (float)(projectile.height / 2)), 1, 1, Main.npc[index].position, Main.npc[index].width, Main.npc[index].height))
                        {
                            num7 = num9;
                            num5 = num1;
                            num6 = num2;
                            flag2 = true;
                            num8 = index;
                        }
                    }
                }
                if (flag2)
                    projectile.ai[1] = (float)(num8 + 1);
                flag2 = false;
            }
            if ((double)projectile.ai[1] > 0.0)
            {
                int index = (int)((double)projectile.ai[1] - 1.0);
                if (Main.npc[index].active && Main.npc[index].CanBeChasedBy((object)this, true) && !Main.npc[index].dontTakeDamage)
                {
                    if ((double)Math.Abs((float)projectile.position.X + (float)(projectile.width / 2) - ((float)Main.npc[index].position.X + (float)(Main.npc[index].width / 2))) + (double)Math.Abs((float)projectile.position.Y + (float)(projectile.height / 2) - ((float)Main.npc[index].position.Y + (float)(Main.npc[index].height / 2))) < 3000.0)
                    {
                        flag2 = true;
                        num5 = (float)Main.npc[index].position.X + (float)(Main.npc[index].width / 2);
                        num6 = (float)Main.npc[index].position.Y + (float)(Main.npc[index].height / 2);
                    }
                }
                else
                    projectile.ai[1] = 0.0f;
            }
            if (!projectile.friendly)
                flag2 = false;
            if (flag2)
            {
                double num15 = (double)num4;
                Vector2 vector2 = new Vector2((float)(projectile.position.X + (double)projectile.width * 0.5), (float)(projectile.position.Y + (double)projectile.height * 0.5));
                num2 = num5 - (float)vector2.X;
                num9 = num6 - (float)vector2.Y;
                double num10 = Math.Sqrt((double)num2 * (double)num2 + (double)num9 * (double)num9);
                float num11 = (float)(num15 / num10);
                float num12 = num2 * num11;
                float num13 = num9 * num11;
                int num14 = 8;
                projectile.velocity.X = (float)((projectile.velocity.X * (double)(num14 - 1) + (double)num12) / (double)num14);
                projectile.velocity.Y = (float)((projectile.velocity.Y * (double)(num14 - 1) + (double)num13) / (double)num14);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 600, true);
            target.AddBuff(ModContent.BuffType<DemonFlames>(), 600, true);
        }
    }
}
