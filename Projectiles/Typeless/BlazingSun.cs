using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using CalamityMod.CalPlayer;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Typeless
{
    public class BlazingSun : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blazing Sun");
        }

        public override void SetDefaults()
        {
            projectile.width = 130;
            projectile.height = 130;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (projectile.ai[0] < 45)
                projectile.ai[0]++;
            //Alpha
            if (projectile.timeLeft > 50)
            {
                if (projectile.alpha > 50)
                    projectile.alpha -= 5;
                if (projectile.alpha < 50)
                    projectile.alpha = 50;
            }
            else
            {
                if(projectile.alpha < 255)
                    projectile.alpha += 5;
                if (projectile.alpha > 255)
                    projectile.alpha = 255;
            }

            //Rotation
            projectile.rotation += 0.025f;
            //Light
            Lighting.AddLight(projectile.Center, new Vector3(240, 185, 7) * (3f / 255));
            
            //Dust
            if(projectile.timeLeft > 40)
            {
                Vector2 Dpos = projectile.Center + new Vector2(Main.rand.NextFloat(80f,100f), Main.rand.NextFloat(80f,100f)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(1,360)));
                Vector2 Dspeed = Dpos - projectile.Center;
                Dspeed.Normalize();
                Dspeed *= -5f;
                float Dscale = Main.rand.NextFloat(1.5f, 2f);
                int d1 = Dust.NewDust(Dpos, 1, 1, 244, Dspeed.X, Dspeed.Y, 0, default, Dscale);
                Main.dust[d1].velocity = Dspeed;
            }

            //Suction
            if (projectile.ai[0] >= 45)
            {
                float num472 = projectile.Center.X;
                float num473 = projectile.Center.Y;
                float num474 = 600f;
                for (int num475 = 0; num475 < 200; num475++)
                {
                    NPC npc = Main.npc[num475];
                    if (npc.CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1) && !CalamityPlayer.areThereAnyDamnBosses)
                    {
                        float npcCenterX = npc.position.X + (float)(npc.width / 2);
                        float npcCenterY = npc.position.Y + (float)(npc.height / 2);
                        float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - npcCenterX) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - npcCenterY);
                        if (num478 < num474)
                        {
                            if (npc.position.X < num472)
                            {
                                npc.velocity.X += 0.1f;
                            }
                            else
                            {
                                npc.velocity.X -= 0.1f;
                            }
                            if (npc.position.Y < num473)
                            {
                                npc.velocity.Y += 0.1f;
                            }
                            else
                            {
                                npc.velocity.Y -= 0.1f;
                            }
                        }
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);
        }
    }
}
