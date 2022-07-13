using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class CosmicViperHomingRocket : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Homing Rocket");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.light = 0.5f;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 1;
            Projectile.scale = 1.18f;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 1;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            float colorScale = (float)Projectile.alpha / 255f;
            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 1f * colorScale, 0.1f * colorScale, 1f * colorScale);

            Player player = Main.player[Projectile.owner];
            Vector2 center = Projectile.Center;
            float maxDistance = 800f;
            bool homeIn = false;

            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float extraDistance = (float)(npc.width / 2) + (float)(npc.height / 2);

                    if (Vector2.Distance(npc.Center, Projectile.Center) < (maxDistance + extraDistance))
                    {
                        center = npc.Center;
                        homeIn = true;
                    }
                }
            }
            else if (Main.npc[(int)Projectile.ai[0]].active && Projectile.ai[0] != -1f)
            {
                NPC npc = Main.npc[(int)Projectile.ai[0]];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float extraDistance = (float)(npc.width / 2) + (float)(npc.height / 2);

                    if (Vector2.Distance(npc.Center, Projectile.Center) < (maxDistance + extraDistance))
                    {
                        center = npc.Center;
                        homeIn = true;
                    }
                }
            }
            if (!homeIn)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile, false))
                    {
                        float extraDistance = (float)(Main.npc[i].width / 2) + (float)(Main.npc[i].height / 2);

                        if (Vector2.Distance(Main.npc[i].Center, Projectile.Center) < (maxDistance + extraDistance))
                        {
                            center = Main.npc[i].Center;
                            homeIn = true;
                            break;
                        }
                    }
                }
            }

            if (homeIn)
            {
                Vector2 moveDirection = Projectile.SafeDirectionTo(center, Vector2.UnitY);

                float homingInertia = 20f;
                float homingVelocity = 20f;
                Projectile.velocity = (Projectile.velocity * homingInertia + moveDirection * homingVelocity) / (homingInertia + 1f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            for (int num621 = 0; num621 < 3; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, Main.rand.NextBool(3) ? 56 : 242, 0f, 0f, 100, default, 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale = 0.5f;
                }
                if (Main.rand.NextBool(2))
                {
                    Main.dust[num622].scale *= 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }
}
