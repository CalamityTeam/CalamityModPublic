using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class MeteorFistProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fist");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 360;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            if (projectile.velocity.Length() >= 4f)
            {
                for (int num246 = 0; num246 < 2; num246++)
                {
                    float num247 = 0f;
                    float num248 = 0f;
                    if (num246 == 1)
                    {
                        num247 = projectile.velocity.X * 0.5f;
                        num248 = projectile.velocity.Y * 0.5f;
                    }
                    int num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 6, 0f, 0f, 100, default, 0.5f);
                    Main.dust[num249].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                    Main.dust[num249].velocity *= 0.2f;
                    Main.dust[num249].noGravity = true;
                }
            }

            // Almost instantly accelerate to very high speed
            if (projectile.velocity.Length() < 12f)
            {
                projectile.velocity *= 1.25f;
            }
            else if (Main.rand.NextBool(2))
            {
                int num252 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 0.5f);
                Main.dust[num252].scale = 0.1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num252].fadeIn = 1.5f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num252].noGravity = true;
                Main.dust[num252].position = projectile.Center + new Vector2(0f, (float)(-(float)projectile.height / 2)).RotatedBy((double)projectile.rotation, default) * 1.1f;
                Main.rand.Next(2);
                num252 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 0.5f);
                Main.dust[num252].scale = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num252].noGravity = true;
                Main.dust[num252].position = projectile.Center + new Vector2(0f, (float)(-(float)projectile.height / 2 - 6)).RotatedBy((double)projectile.rotation, default) * 1.1f;
            }

            projectile.ai[0] += 1f;
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (projectile.ai[0] > 10f || projectile.ai[0] > 5f)
            {
                projectile.ai[0] = 10f;
                if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
                {
                    projectile.velocity.X *= 0.97f;
                    if (Math.Abs(projectile.velocity.X) < 0.01f)
                    {
                        projectile.velocity.X = 0f;
                        projectile.netUpdate = true;
                    }
                }

                if (!projectile.Calamity().stealthStrike)
                    projectile.velocity.Y += 0.1f;
            }
        }

        public override void Kill(int timeLeft)
        {
            CalamityGlobalProjectile.ExpandHitboxBy(projectile, 32);
            projectile.maxPenetrate = projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.Item14, projectile.position);
            for (int num621 = 0; num621 < 40; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int num623 = 0; num623 < 70; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default, 1f);
                Main.dust[num624].velocity *= 2f;
            }
            CalamityUtils.ExplosionGores(projectile.Center, 3);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 120);
            float minDist = 999f;
            int index = 0;
            // Get the closest enemy to the fist
            if (projectile.Calamity().stealthStrike && projectile.ai[1] > 0f && projectile.penetrate != -1)
            {
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(projectile, false) && npc != target)
                    {
                        float dist = (projectile.Center - npc.Center).Length();
                        if (dist < minDist)
                        {
                            minDist = dist;
                            index = i;
                        }
                    }
                }
                Vector2 newFistVelocity;
                if (minDist < 999f)
                {
                    newFistVelocity = Main.npc[index].Center - projectile.Center;
                }
                else
                {
                    newFistVelocity = -projectile.velocity;
                }
                newFistVelocity.Normalize();
                newFistVelocity *= 10f;
                float AI1 = projectile.ai[1] - 1f;
                int p = Projectile.NewProjectile(projectile.position, newFistVelocity, ModContent.ProjectileType<MeteorFistProj>(), projectile.damage, projectile.knockBack, projectile.owner, 0, AI1);
                Main.projectile[p].Calamity().stealthStrike = true;
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }
    }
}
