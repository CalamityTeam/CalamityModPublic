using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SiriusMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sirius");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 48;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            bool correctMinion = projectile.type == ModContent.ProjectileType<SiriusMinion>();
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.sirius = false;
                }
                if (modPlayer.sirius)
                {
                    projectile.timeLeft = 2;
                }
            }
            player.AddBuff(ModContent.BuffType<SiriusBuff>(), 3600);

            projectile.minionSlots = projectile.ai[0];
            Lighting.AddLight(projectile.Center, 1f, 0.5f, 5f);

            projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY - 60f);
            if (player.gravDir == -1f)
            {
                projectile.position.Y += 120f;
                projectile.rotation = MathHelper.Pi;
            }
            else
            {
                projectile.rotation = 0f;
            }
            projectile.position.X = (int)projectile.position.X;
            projectile.position.Y = (int)projectile.position.Y;

            float scalar = (float)Main.mouseTextColor / 200f - 0.35f;
            scalar *= 0.2f;
            projectile.scale = scalar + 0.95f;

            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 50;
                for (int d = 0; d < dustAmt; d++)
                {
                    int sirius = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 20, 0f, 0f, 0, default, 1f);
                    Main.dust[sirius].velocity *= 2f;
                    Main.dust[sirius].scale *= 1.15f;
                }
                projectile.localAI[0] += 1f;
            }

            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue * player.MinionDamage());
                projectile.damage = damage2;
            }
            if (projectile.owner == Main.myPlayer)
            {
                if (projectile.ai[1] != 0f)
                {
                    projectile.ai[1] -= 1f;
                    return;
                }
                Vector2 targetVec = projectile.position;
                float maxDistance = 7000f;
                bool hasTarget = false;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float extraDistance = (npc.width / 2) + (npc.height / 2);

                        if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance))
                        {
                            targetVec = npc.Center;
                            hasTarget = true;
                        }
                    }
                }
                if (!hasTarget)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(projectile, false))
                        {
                            float extraDistance = (npc.width / 2) + (npc.height / 2);

                            if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance))
                            {
                                targetVec = npc.Center;
                                hasTarget = true;
                            }
                        }
                    }
                }
                if (hasTarget)
                {
                    float projSpeed = 15f;
                    Vector2 source = new Vector2(projectile.Center.X - 4f, projectile.Center.Y);
                    Vector2 velocity = targetVec - projectile.Center;
                    float targetDist = velocity.Length();
                    targetDist = projSpeed / targetDist;
                    velocity.X *= targetDist;
                    velocity.Y *= targetDist;
                    float damageMult = ((float)Math.Log(projectile.ai[0], MathHelper.E)) + 1f;
                    int beam = Projectile.NewProjectile(source, velocity, ModContent.ProjectileType<SiriusBeam>(), (int)(projectile.damage * damageMult), projectile.knockBack, projectile.owner);
                    Main.projectile[beam].penetrate = (int)projectile.ai[0];
                    projectile.ai[1] = 30f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        public override bool CanDamage() => false;
    }
}
