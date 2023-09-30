using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class RegulusRiotProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/RegulusRiot";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 15;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 20;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

            int blueDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 100);
            Main.dust[blueDust].noGravity = true;
            Main.dust[blueDust].velocity = Vector2.Zero;
            int orangeDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100);
            Main.dust[orangeDust].noGravity = true;
            Main.dust[orangeDust].velocity = Vector2.Zero;

            Projectile.ai[0] += 1f;
            int behaviorInt = 0;
            if (Projectile.velocity.Length() <= 8f) //4
            {
                behaviorInt = 1;
            }
            if (behaviorInt == 0)
            {
                Projectile.rotation -= 0.104719758f;

                if (Projectile.ai[0] >= 30f)
                {
                    Projectile.extraUpdates = 2;
                    Projectile.velocity *= 0.98f;
                    Projectile.rotation -= 0.0174532924f;
                }
                if (Projectile.velocity.Length() < 8.2f) //4.1
                {
                    Projectile.velocity.Normalize();
                    Projectile.velocity *= 4f;
                    Projectile.ai[0] = 0f;
                    Projectile.extraUpdates = 1;
                }
            }
            else if (behaviorInt == 1)
            {
                Projectile.rotation -= 0.104719758f;
                Vector2 targetCenter = Projectile.Center;
                float homingRange = 300f;
                bool homeIn = false;
                int targetIndex = 0;
                if (Projectile.ai[1] == 0f)
                {
                    for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                    {
                        NPC npc = Main.npc[npcIndex];
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            Vector2 npcCenter = npc.Center;
                            if (Projectile.Distance(npcCenter) < homingRange)
                            {
                                homingRange = Projectile.Distance(npcCenter);
                                targetCenter = npcCenter;
                                homeIn = true;
                                targetIndex = npcIndex;
                                break;
                            }
                        }
                    }
                    if (homeIn)
                    {
                        if (Projectile.ai[1] != (float)(targetIndex + 1))
                        {
                            Projectile.netUpdate = true;
                        }
                        Projectile.ai[1] = (float)(targetIndex + 1);
                    }
                    homeIn = false;
                }
                if (Projectile.ai[1] != 0f)
                {
                    int npcIndex2 = (int)(Projectile.ai[1] - 1f);
                    NPC npc2 = Main.npc[npcIndex2];
                    if (npc2.active && npc2.CanBeChasedBy(Projectile, true) && Projectile.Distance(npc2.Center) < 1000f)
                    {
                        homeIn = true;
                        targetCenter = Main.npc[npcIndex2].Center;
                    }
                }
                if (!Projectile.friendly)
                {
                    homeIn = false;
                }
                if (homeIn)
                {
                    float homeSpeed = 24f;
                    float turnMult = 10f;
                    Vector2 projCenter = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float xDist = targetCenter.X - projCenter.X;
                    float yDist = targetCenter.Y - projCenter.Y;
                    float totalDist = (float)Math.Sqrt((double)(xDist * xDist + yDist * yDist));
                    totalDist = homeSpeed / totalDist;
                    xDist *= totalDist;
                    yDist *= totalDist;
                    Projectile.velocity.X = (Projectile.velocity.X * (turnMult - 1f) + xDist) / turnMult;
                    Projectile.velocity.Y = (Projectile.velocity.Y * (turnMult - 1f) + yDist) / turnMult;
                }
            }
            if (Projectile.ai[0] >= 180f)
            {
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int blueDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<AstralBlue>(), 0f, 0f, 100, default, 1.5f);
                Main.dust[blueDust].noGravity = true;
                Main.dust[blueDust].velocity = Vector2.Zero;
            }
            for (int i = 0; i < 10; i++)
            {
                int orangeDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                Main.dust[orangeDust].noGravity = true;
                Main.dust[orangeDust].velocity = Vector2.Zero;
            }
            if (Projectile.Calamity().stealthStrike)
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    float spread = 60f * 0.0174f;
                    double startAngle = Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 6f;
                    double offsetAngle;
                    for (int i = 0; i < 3; i++)
                    {
                        offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 2f), (float)(Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<RegulusEnergy>(), (int)(Projectile.damage * 0.4), Projectile.knockBack, Projectile.owner, 0f, 0f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 2f), (float)(-Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<RegulusEnergy>(), (int)(Projectile.damage * 0.4), Projectile.knockBack, Projectile.owner, 0f, 0f);
                    }
                }
            }
        }
    }
}
