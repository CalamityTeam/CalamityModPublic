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
            Projectile.width = 130;
            Projectile.height = 130;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            if (Projectile.ai[0] < 45)
                Projectile.ai[0]++;
            //Alpha
            if (Projectile.timeLeft > 50)
            {
                if (Projectile.alpha > 50)
                    Projectile.alpha -= 5;
                if (Projectile.alpha < 50)
                    Projectile.alpha = 50;
            }
            else
            {
                if(Projectile.alpha < 255)
                    Projectile.alpha += 5;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }

            //Rotation
            Projectile.rotation += 0.025f;
            //Light
            Lighting.AddLight(Projectile.Center, new Vector3(240, 185, 7) * (3f / 255));

            //Dust
            if(Projectile.timeLeft > 40)
            {
                Vector2 Dpos = Projectile.Center + new Vector2(Main.rand.NextFloat(80f,100f), Main.rand.NextFloat(80f,100f)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(1,360)));
                Vector2 Dspeed = Dpos - Projectile.Center;
                Dspeed.Normalize();
                Dspeed *= -5f;
                float Dscale = Main.rand.NextFloat(1.5f, 2f);
                int d1 = Dust.NewDust(Dpos, 1, 1, 244, Dspeed.X, Dspeed.Y, 0, default, Dscale);
                Main.dust[d1].velocity = Dspeed;
            }

            //Suction
            if (Projectile.ai[0] >= 45)
            {
                float num472 = Projectile.Center.X;
                float num473 = Projectile.Center.Y;
                float num474 = 600f;
                for (int num475 = 0; num475 < 200; num475++)
                {
                    NPC npc = Main.npc[num475];
                    if (npc.CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1) && !CalamityPlayer.areThereAnyDamnBosses)
                    {
                        float npcCenterX = npc.position.X + (float)(npc.width / 2);
                        float npcCenterY = npc.position.Y + (float)(npc.height / 2);
                        float num478 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcCenterX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcCenterY);
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
