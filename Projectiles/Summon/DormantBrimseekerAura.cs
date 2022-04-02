using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class DormantBrimseekerAura : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Summon/DormantBrimseeker";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dormant Brimseeker Aura Projecter");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.width = 30;
            projectile.height = 30;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.rotation.AngleLerp(-MathHelper.PiOver4, 0.045f);
            projectile.velocity *= 0.975f;
            if (Math.Abs(projectile.rotation + MathHelper.PiOver4) < 0.04f)
            {
                if (projectile.localAI[0] == 0f)
                {
                    Main.PlaySound(SoundID.DD2_EtherianPortalOpen, projectile.Center);
                    for (int i = 0; i < 30; i++)
                    {
                        float angle = 1.4f * (i / 30f) - 0.7f;
                        Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.Brimstone);
                        dust.velocity = new Vector2(0f, -14f).RotatedBy(angle);
                        dust.noGravity = true;
                    }
                    for (int i = 0; i < 50; i++)
                    {
                        float angle = 2.4f * (i / 50f) - 1.2f;
                        Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.Brimstone);
                        dust.velocity = new Vector2(0f, 8f).RotatedBy(angle);
                    }
                    projectile.timeLeft = 820;
                    projectile.localAI[0] = 1f;
                }
                else
                {
                    if (projectile.localAI[1] < 400f)
                    {
                        projectile.localAI[1] += 6f;
                    }
                    projectile.ai[1] += MathHelper.ToRadians(1.2f);
                    for (int i = 0; i < 85; i++)
                    {
                        float angle = MathHelper.TwoPi / 85f * i + projectile.ai[1];
                        Dust dust = Dust.NewDustPerfect(projectile.Center + angle.ToRotationVector2() * projectile.localAI[1], (int)CalamityDusts.Brimstone);
                        dust.noGravity = true;
                        dust.velocity = Vector2.Zero;
                        if (Main.rand.NextBool(360))
                            dust.scale = 1.5f;
                    }
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<DormantBrimseekerBab>() &&
                            Main.projectile[i].owner == projectile.owner && Main.projectile[i].localAI[1] == 0f)
                        {
                            if (Main.projectile[i].Distance(projectile.Center) < projectile.localAI[1])
                            {
                                for (int j = 0; j < 30; j++)
                                {
                                    Dust dust = Dust.NewDustPerfect(Main.projectile[i].Center, (int)CalamityDusts.Brimstone);
                                    dust.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 7f;
                                }
                                Main.projectile[i].localAI[1] = 1f;
                                Main.PlaySound(SoundID.Item45, Main.projectile[i].Center);
                            }
                        }
                    }

                    if (Main.rand.NextBool(50))
                    {
                        int idx = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<BrimseekerAuraBall>(), projectile.damage, 3f, projectile.owner, projectile.identity);
                        Main.projectile[idx].timeLeft = projectile.timeLeft;
                    }
                }
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<DormantBrimseekerBab>() &&
                    Main.projectile[i].owner == projectile.owner && Main.projectile[i].localAI[1] == 1f)
                {
                    for (int j = 0; j < 30; j++)
                    {
                        Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.Brimstone);
                        dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 7f;
                    }
                    Main.PlaySound(SoundID.Item29, Main.projectile[i].Center);
                    Main.projectile[i].localAI[1] = 0f;
                }
            }
        }
        public override bool CanDamage() => false;
    }
}
