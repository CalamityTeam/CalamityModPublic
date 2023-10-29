using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class WaterElementalMinion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public int dust = 3;

        public override void SetStaticDefaults()
        {
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
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            bool isMinion = Projectile.type == ModContent.ProjectileType<WaterElementalMinion>();
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.sirenWaifu && !modPlayer.allWaifus && !modPlayer.sirenWaifuVanity && !modPlayer.allWaifusVanity)
            {
                Projectile.active = false;
                return;
            }
            if (isMinion)
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
                for (int i = 0; i < 50; i++)
                {
                    int spawnDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, 33, 0f, 0f, 0, default, 1f);
                    Main.dust[spawnDust].velocity *= 2f;
                    Main.dust[spawnDust].scale *= 1.15f;
                }
                dust--;
            }
            bool passive = modPlayer.sirenWaifuVanity || modPlayer.allWaifusVanity;
            if (!passive)
                Lighting.AddLight(Projectile.Center, 0f, 0.25f, 1.5f);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
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
            if (Projectile.owner == Main.myPlayer && !passive)
            {
                // Prevent firing immediately
                if (Projectile.localAI[0] < 120f)
                    Projectile.localAI[0] += 1f;

                if (Projectile.ai[0] != 0f)
                {
                    Projectile.ai[0] -= 1f;
                    return;
                }
                bool canAttack = false;
                float projX = Projectile.Center.X;
                float projY = Projectile.Center.Y;
                float attackRange = 1000f;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float npcX = npc.position.X + (float)(npc.width / 2);
                        float npcY = npc.position.Y + (float)(npc.height / 2);
                        float npcDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcY);
                        if (npcDist < attackRange && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            projX = npcX;
                            projY = npcY;
                            canAttack = true;
                        }
                    }
                }
                if (!canAttack)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].CanBeChasedBy(Projectile, false))
                        {
                            float targetX = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
                            float targetY = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
                            float targetDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - targetX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - targetY);
                            if (targetDist < attackRange && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height))
                            {
                                attackRange = targetDist;
                                projX = targetX;
                                projY = targetY;
                                canAttack = true;
                            }
                        }
                    }
                }
                if (canAttack && Projectile.localAI[0] >= 120f)
                {
                    float projXStore = projX;
                    float projYStore = projY;
                    projX -= Projectile.Center.X;
                    projY -= Projectile.Center.Y;
                    if (projX < 0f)
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
                    float projVel = Main.rand.Next(12, 20);
                    Vector2 fireDirection = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    float fireXVel = projXStore - fireDirection.X;
                    float fireYVel = projYStore - fireDirection.Y;
                    float fireVelocity = (float)Math.Sqrt((double)(fireXVel * fireXVel + fireYVel * fireYVel));
                    fireVelocity = projVel / fireVelocity;
                    fireXVel *= fireVelocity;
                    fireYVel *= fireVelocity;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X - 4f, Projectile.Center.Y, fireXVel, fireYVel, projectileType, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
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
