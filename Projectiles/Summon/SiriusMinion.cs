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
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 48;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            bool correctMinion = Projectile.type == ModContent.ProjectileType<SiriusMinion>();
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.sirius = false;
                }
                if (modPlayer.sirius)
                {
                    Projectile.timeLeft = 2;
                }
            }
            player.AddBuff(ModContent.BuffType<SiriusBuff>(), 3600);

            Projectile.minionSlots = Projectile.ai[0];
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 1f);

            Projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY - 60f);
            if (player.gravDir == -1f)
            {
                Projectile.position.Y += 120f;
                Projectile.rotation = MathHelper.Pi;
            }
            else
            {
                Projectile.rotation = 0f;
            }
            Projectile.position.X = (int)Projectile.position.X;
            Projectile.position.Y = (int)Projectile.position.Y;

            float scalar = (float)Main.mouseTextColor / 200f - 0.35f;
            scalar *= 0.2f;
            Projectile.scale = scalar + 0.95f;

            if (Projectile.localAI[0] == 0f)
            {
                int dustAmt = 50;
                for (int d = 0; d < dustAmt; d++)
                {
                    int sirius = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, 20, 0f, 0f, 0, default, 1f);
                    Main.dust[sirius].velocity *= 2f;
                    Main.dust[sirius].scale *= 1.15f;
                }
                Projectile.localAI[0] += 1f;
            }

            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[1] != 0f)
                {
                    Projectile.ai[1] -= 1f;
                    return;
                }
                Vector2 targetVec = Projectile.position;
                float maxDistance = 7000f;
                bool hasTarget = false;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float extraDistance = (npc.width / 2) + (npc.height / 2);

                        if (Vector2.Distance(npc.Center, Projectile.Center) < (maxDistance + extraDistance))
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
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            float extraDistance = (npc.width / 2) + (npc.height / 2);

                            if (Vector2.Distance(npc.Center, Projectile.Center) < (maxDistance + extraDistance))
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
                    Vector2 source = new Vector2(Projectile.Center.X - 4f, Projectile.Center.Y);
                    Vector2 velocity = targetVec - Projectile.Center;
                    float targetDist = velocity.Length();
                    targetDist = projSpeed / targetDist;
                    velocity.X *= targetDist;
                    velocity.Y *= targetDist;
                    float damageMult = ((float)Math.Log(Projectile.ai[0], MathHelper.E)) + 1f;
                    int beam = Projectile.NewProjectile(Projectile.GetSource_FromThis(), source, velocity, ModContent.ProjectileType<SiriusBeam>(), (int)(Projectile.damage * damageMult), Projectile.knockBack, Projectile.owner);
                    if (Main.projectile.IndexInRange(beam))
                        Main.projectile[beam].originalDamage = (int)(Projectile.originalDamage * damageMult);
                    Main.projectile[beam].penetrate = (int)Projectile.ai[0];
                    Projectile.ai[1] = 30f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        public override bool? CanDamage() => false;
    }
}
