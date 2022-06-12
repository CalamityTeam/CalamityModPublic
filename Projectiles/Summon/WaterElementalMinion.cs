using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class WaterElementalMinion : ModProjectile
    {
        public int dust = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anahita");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 190;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
        }

        public override void AI()
        {
            bool flag64 = Projectile.type == ModContent.ProjectileType<WaterElementalMinion>();
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.sirenWaifu && !modPlayer.allWaifus)
            {
                Projectile.active = false;
                return;
            }
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.slWaifu = false;
                }
                if (modPlayer.slWaifu)
                {
                    Projectile.timeLeft = 2;
                }
            }
            if (dust > 0)
            {
                int num501 = 50;
                for (int num502 = 0; num502 < num501; num502++)
                {
                    int num503 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, 33, 0f, 0f, 0, default, 1f);
                    Main.dust[num503].velocity *= 2f;
                    Main.dust[num503].scale *= 1.15f;
                }
                dust--;
            }
            Lighting.AddLight(Projectile.Center, 0f, 0.25f, 1.5f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 6)
            {
                Projectile.frame = 0;
            }
            Projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY - 180f);
            if (player.gravDir == -1f)
            {
                Projectile.position.Y += 360f;
                Projectile.rotation = MathHelper.Pi;
            }
            else
            {
                Projectile.rotation = 0f;
            }
            Projectile.position.X = (int)Projectile.position.X;
            Projectile.position.Y = (int)Projectile.position.Y;
            if (Projectile.owner == Main.myPlayer)
            {
                // Prevent firing immediately
                if (Projectile.localAI[0] < 120f)
                    Projectile.localAI[0] += 1f;

                if (Projectile.ai[0] != 0f)
                {
                    Projectile.ai[0] -= 1f;
                    return;
                }
                bool flag18 = false;
                float num506 = Projectile.Center.X;
                float num507 = Projectile.Center.Y;
                float num508 = 1000f;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float num539 = npc.position.X + (float)(npc.width / 2);
                        float num540 = npc.position.Y + (float)(npc.height / 2);
                        float num541 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num539) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num540);
                        if (num541 < num508 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            num506 = num539;
                            num507 = num540;
                            flag18 = true;
                        }
                    }
                }
                if (!flag18)
                {
                    for (int num512 = 0; num512 < Main.maxNPCs; num512++)
                    {
                        if (Main.npc[num512].CanBeChasedBy(Projectile, false))
                        {
                            float num513 = Main.npc[num512].position.X + (float)(Main.npc[num512].width / 2);
                            float num514 = Main.npc[num512].position.Y + (float)(Main.npc[num512].height / 2);
                            float num515 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num513) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num514);
                            if (num515 < num508 && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[num512].position, Main.npc[num512].width, Main.npc[num512].height))
                            {
                                num508 = num515;
                                num506 = num513;
                                num507 = num514;
                                flag18 = true;
                            }
                        }
                    }
                }
                if (flag18 && Projectile.localAI[0] >= 120f)
                {
                    float num516 = num506;
                    float num517 = num507;
                    num506 -= Projectile.Center.X;
                    num507 -= Projectile.Center.Y;
                    if (num506 < 0f)
                    {
                        Projectile.spriteDirection = 1;
                    }
                    else
                    {
                        Projectile.spriteDirection = -1;
                    }
                    int projectileType = ModContent.ProjectileType<WaterSpearFriendly>();
                    if (Main.rand.NextBool(9))
                    {
                        projectileType = ModContent.ProjectileType<FrostMistFriendly>();
                    }
                    else if (Main.rand.NextBool(9))
                    {
                        projectileType = ModContent.ProjectileType<WaterElementalSong>();
                    }
                    float num403 = Main.rand.Next(12, 20);
                    Vector2 vector29 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float num404 = num516 - vector29.X;
                    float num405 = num517 - vector29.Y;
                    float num406 = (float)Math.Sqrt((double)(num404 * num404 + num405 * num405));
                    num406 = num403 / num406;
                    num404 *= num406;
                    num405 *= num406;
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X - 4f, Projectile.Center.Y, num404, num405, projectileType, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
                    Main.projectile[proj].originalDamage = Projectile.originalDamage;
                    Projectile.ai[0] = 12f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 200);
        }

        public override bool? CanDamage() => false;
    }
}
