using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Dusts;
using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class AstralProbeSummon : ModProjectile
    {
        private double rotation = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Probe");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.timeLeft *= 5;
            projectile.penetrate = -1;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.minionDamage;
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, (Main.rand.NextBool(2) ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>()), vector7.X * 1.75f, vector7.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = vector7;
                }
                projectile.localAI[0] += 1f;
            }
            if (player.minionDamage != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.minionDamage);
                projectile.damage = damage2;
            }
            bool flag64 = projectile.type == ModContent.ProjectileType<AstralProbeSummon>();
            player.AddBuff(ModContent.BuffType<AstralProbeBuff>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.aProbe = false;
                }
                if (modPlayer.aProbe)
                {
                    projectile.timeLeft = 2;
                }
            }
            float num633 = 700f;
            float num634 = 1300f;
            float num635 = 2600f;
            float num636 = 600f;
            Vector2 vector46 = projectile.position;
            bool flag25 = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float num646 = Vector2.Distance(npc.Center, projectile.Center);
                    if ((Vector2.Distance(projectile.Center, vector46) > num646 && num646 < num633) || !flag25)
                    {
                        vector46 = npc.Center;
                        flag25 = true;
                    }
                }
            }
            else
            {
                for (int num645 = 0; num645 < 200; num645++)
                {
                    NPC nPC2 = Main.npc[num645];
                    if (nPC2.CanBeChasedBy(projectile, false))
                    {
                        float num646 = Vector2.Distance(nPC2.Center, projectile.Center);
                        if ((Vector2.Distance(projectile.Center, vector46) > num646 && num646 < num633) || !flag25)
                        {
                            num633 = num646;
                            vector46 = nPC2.Center;
                            flag25 = true;
                        }
                    }
                }
            }
            Vector2 vector = player.Center - projectile.Center;
            if (flag25)
            {
                projectile.rotation = (vector46 - projectile.Center).ToRotation() + 3.14159274f;
            }
			else
			{
				projectile.rotation = vector.ToRotation() - 1.57f - (projectile.spriteDirection == 1 ? MathHelper.ToRadians(270) : MathHelper.ToRadians(90) * projectile.direction);
			}
            projectile.Center = player.Center + new Vector2(80, 0).RotatedBy(rotation);
            rotation += 0.03;
            if (rotation >= 360)
            {
                rotation = 0;
            }
            projectile.velocity.X = (vector.X > 0f) ? -0.000001f : 0f;

            float num647 = num634;
            if (flag25)
            {
                num647 = num635;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > num647)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }
            if (!flag25 && projectile.ai[0] != 0f)
            {
                bool flag26 = false;
                if (!flag26)
                {
                    flag26 = projectile.ai[0] == 1f;
                }
                Vector2 center2 = projectile.Center;
                Vector2 vector48 = player.Center - center2 + new Vector2(0f, -120f);
                float num651 = vector48.Length();
                if (num651 < num636 && flag26)
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
            }
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (projectile.ai[1] > 90f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            if (projectile.ai[0] == 0f)
            {
                float scaleFactor3 = 6f;
                int num658 = ModContent.ProjectileType<AstralProbeRound>();
                if (flag25 && projectile.ai[1] == 0f)
                {
                    Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 12);
                    projectile.ai[1] += 1f;
                    if (Main.myPlayer == projectile.owner)
                    {
                        Vector2 value19 = vector46 - projectile.Center;
                        value19.Normalize();
                        value19 *= scaleFactor3;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value19.X, value19.Y, num658, projectile.damage, 0f, Main.myPlayer, 0f, 0f);
                        projectile.netUpdate = true;
                    }
                }
            }
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
