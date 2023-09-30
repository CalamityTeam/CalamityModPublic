using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class CosmicViperSplittingRocket : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            float colorScale = (float)Projectile.alpha / 255f;
            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 1f * colorScale, 0.1f * colorScale, 1f * colorScale);
            Vector2 center = Projectile.Center;
            float maxDistance = 800f;
            float explode = 16f;
            bool homeIn = false;
            Player player = Main.player[Projectile.owner];

            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float extraDistance = (float)(npc.width / 2) + (float)(npc.height / 2);

                    if (Vector2.Distance(npc.Center, Projectile.Center) < (explode + extraDistance))
                    {
                        int numProj = 4;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                                while (speed.X == 0f && speed.Y == 0f)
                                {
                                    speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                                }
                                speed.Normalize();
                                speed *= (float)Main.rand.Next(30, 61) * 0.1f * 2f;
                                int splitRocket = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, speed, ModContent.ProjectileType<CosmicViperSplitRocket1>(), (int)(Projectile.damage * 0.25), Projectile.knockBack, Projectile.owner, 0f, 0f);
                                if (Main.projectile.IndexInRange(splitRocket))
                                    Main.projectile[splitRocket].originalDamage = (int)(Projectile.originalDamage * 0.25f);
                            }
                        }
                        SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                        Projectile.Kill();
                        return;
                    }
                    else if (Vector2.Distance(npc.Center, Projectile.Center) < (maxDistance + extraDistance))
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

                    if (Vector2.Distance(npc.Center, Projectile.Center) < (explode + extraDistance))
                    {
                        int numProj = 4;
                        if (Projectile.owner == Main.myPlayer)
                        {
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                                while (speed.X == 0f && speed.Y == 0f)
                                {
                                    speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
                                }
                                speed.Normalize();
                                speed *= (float)Main.rand.Next(30, 61) * 0.1f * 2f;
                                int rocket = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, speed, ModContent.ProjectileType<CosmicViperSplitRocket1>(), (int)(Projectile.damage * 0.25f), Projectile.knockBack, Projectile.owner, 0f, 0f);
                                if (Main.projectile.IndexInRange(rocket))
                                    Main.projectile[rocket].originalDamage = (int)(Projectile.originalDamage * 0.25f);
                            }
                        }
                        SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                        Projectile.Kill();
                        return;
                    }
                    else if (Vector2.Distance(npc.Center, Projectile.Center) < (maxDistance + extraDistance))
                    {
                        center = npc.Center;
                        homeIn = true;
                    }
                }
            }
            if (!homeIn)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float extraDistance = (float)(npc.width / 2) + (float)(npc.height / 2);

                        if (Vector2.Distance(npc.Center, Projectile.Center) < (explode + extraDistance))
                        {
                            int numProj = 4;
                            if (Projectile.owner == Main.myPlayer)
                            {
                                for (int i = 0; i < numProj; i++)
                                {
                                    Vector2 speed = CalamityUtils.RandomVelocity(50f, 30f, 60f, 0.2f);
                                    int rocket = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, speed, ModContent.ProjectileType<CosmicViperSplitRocket1>(), Projectile.damage / numProj, Projectile.knockBack, Projectile.owner, Main.rand.Next(2), 0f);
                                    if (Main.projectile.IndexInRange(rocket))
                                        Main.projectile[rocket].originalDamage = (int)(Projectile.originalDamage * 0.25f);
                                }
                            }
                            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
                            Projectile.Kill();
                            return;
                        }
                        else if (Vector2.Distance(npc.Center, Projectile.Center) < (maxDistance + extraDistance))
                        {
                            center = npc.Center;
                            homeIn = true;
                        }
                    }
                }
            }
            if (homeIn)
            {
                Vector2 moveDirection = Projectile.SafeDirectionTo(center, Vector2.UnitY);

                float homingInertia = 15f;
                float homingVelocity = 30f;
                Projectile.velocity = (Projectile.velocity * homingInertia + moveDirection * homingVelocity) / (homingInertia + 1f);
            }
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
        }

        public override void OnKill(int timeLeft)
        {
			Projectile.ExpandHitboxBy(50);
            for (int i = 0; i < 3; i++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool(3) ? 56 : 242, 0f, 0f, 100, default, 2f);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[idx].scale = 0.5f;
                }
                if (Main.rand.NextBool())
                {
                    Main.dust[idx].scale *= 0.5f;
                    Main.dust[idx].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
