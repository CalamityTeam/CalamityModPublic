using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class DormantBrimseekerAura : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Items/Weapons/Summon/DormantBrimseeker";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.rotation.AngleLerp(-MathHelper.PiOver4, 0.045f);
            Projectile.velocity *= 0.975f;
            if (Math.Abs(Projectile.rotation + MathHelper.PiOver4) < 0.04f)
            {
                if (Projectile.localAI[0] == 0f)
                {
                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Projectile.Center);
                    for (int i = 0; i < 30; i++)
                    {
                        float angle = 1.4f * (i / 30f) - 0.7f;
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.Brimstone);
                        dust.velocity = new Vector2(0f, -14f).RotatedBy(angle);
                        dust.noGravity = true;
                    }
                    for (int i = 0; i < 50; i++)
                    {
                        float angle = 2.4f * (i / 50f) - 1.2f;
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.Brimstone);
                        dust.velocity = new Vector2(0f, 8f).RotatedBy(angle);
                    }
                    Projectile.timeLeft = 820;
                    Projectile.localAI[0] = 1f;
                }
                else
                {
                    if (Projectile.localAI[1] < 400f)
                    {
                        Projectile.localAI[1] += 6f;
                    }
                    Projectile.ai[1] += MathHelper.ToRadians(1.2f);
                    for (int i = 0; i < 85; i++)
                    {
                        float angle = MathHelper.TwoPi / 85f * i + Projectile.ai[1];
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + angle.ToRotationVector2() * Projectile.localAI[1], (int)CalamityDusts.Brimstone);
                        dust.noGravity = true;
                        dust.velocity = Vector2.Zero;
                        if (Main.rand.NextBool(360))
                            dust.scale = 1.5f;
                    }
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<DormantBrimseekerBab>() &&
                            Main.projectile[i].owner == Projectile.owner && Main.projectile[i].localAI[1] == 0f)
                        {
                            if (Main.projectile[i].Distance(Projectile.Center) < Projectile.localAI[1])
                            {
                                for (int j = 0; j < 30; j++)
                                {
                                    Dust dust = Dust.NewDustPerfect(Main.projectile[i].Center, (int)CalamityDusts.Brimstone);
                                    dust.velocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * 7f;
                                }
                                Main.projectile[i].localAI[1] = 1f;
                                SoundEngine.PlaySound(SoundID.Item45, Main.projectile[i].Center);
                            }
                        }
                    }

                    if (Main.rand.NextBool(50))
                    {
                        int idx = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BrimseekerAuraBall>(), Projectile.damage, 3f, Projectile.owner, Projectile.identity);
                        Main.projectile[idx].timeLeft = Projectile.timeLeft;
                    }
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<DormantBrimseekerBab>() &&
                    Main.projectile[i].owner == Projectile.owner && Main.projectile[i].localAI[1] == 1f)
                {
                    for (int j = 0; j < 30; j++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.Brimstone);
                        dust.velocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * 7f;
                    }
                    SoundEngine.PlaySound(SoundID.Item29, Main.projectile[i].Center);
                    Main.projectile[i].localAI[1] = 0f;
                }
            }
        }

        public override bool? CanDamage() => false;
    }
}
