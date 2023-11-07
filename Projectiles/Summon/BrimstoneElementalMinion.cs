using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class BrimstoneElementalMinion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public int dust = 3;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 126;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            bool isActive = Projectile.type == ModContent.ProjectileType<BrimstoneElementalMinion>();
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.brimstoneWaifu && !modPlayer.allWaifus && !modPlayer.brimstoneWaifuVanity && !modPlayer.allWaifusVanity)
            {
                Projectile.active = false;
                return;
            }
            if (isActive)
            {
                if (player.dead)
                {
                    modPlayer.bWaifu = false;
                }
                if (modPlayer.bWaifu)
                {
                    Projectile.timeLeft = 2;
                }
            }
            dust--;
            if (dust >= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    int brimDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, (int)CalamityDusts.Brimstone, 0f, 0f, 0, default, 1f);
                    Main.dust[brimDust].velocity *= 2f;
                    Main.dust[brimDust].scale *= 1.15f;
                }
            }
            bool passive = modPlayer.brimstoneWaifuVanity || modPlayer.allWaifusVanity;
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 9)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
            if (!passive)
            {
                float lights = (float)Main.rand.Next(90, 111) * 0.01f;
                lights *= Main.essScale;
                Lighting.AddLight(Projectile.Center, 1.25f * lights, 0f * lights, 0.5f * lights);
            }
            if (Math.Abs(Projectile.velocity.X) > 0.2f)
            {
                Projectile.spriteDirection = -Projectile.direction;
            }
            float maxTargetDist = 700f;
            Projectile.MinionAntiClump();
            Vector2 targetCenter = Projectile.position;
            bool canAttack = false;
            if (Projectile.ai[0] != 1f)
            {
                Projectile.tileCollide = false;
            }
            if (Projectile.tileCollide && WorldGen.SolidTile(Framing.GetTileSafely((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16)))
            {
                Projectile.tileCollide = false;
            }
            if (player.HasMinionAttackTargetNPC && !passive)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if ((!canAttack && targetDist < maxTargetDist) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                    {
                        targetCenter = npc.Center;
                        canAttack = true;
                    }
                }
            }
            if (!canAttack && !passive)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC nPC2 = Main.npc[i];
                    if (nPC2.CanBeChasedBy(Projectile, false))
                    {
                        float targetDist = Vector2.Distance(nPC2.Center, Projectile.Center);
                        if ((!canAttack && targetDist < maxTargetDist) && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, nPC2.position, nPC2.width, nPC2.height))
                        {
                            maxTargetDist = targetDist;
                            targetCenter = nPC2.Center;
                            canAttack = true;
                        }
                    }
                }
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > 1200f)
            {
                Projectile.ai[0] = 1f;
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
            }
            bool isReturning = false;
            if (!isReturning)
            {
                isReturning = Projectile.ai[0] == 1f;
            }
            float returnSpeed = 5f; //6
            if (isReturning)
            {
                returnSpeed = 12f; //15
            }
            Vector2 center2 = Projectile.Center;
            Vector2 playerDirection = player.Center - center2 + new Vector2(-500f, -60f); //-60
            float playerDistance = playerDirection.Length();
            if (playerDistance > 200f && returnSpeed < 6.5f) //200 and 8
            {
                returnSpeed = 6.5f; //8
            }
            if (playerDistance < 400f && isReturning && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
            if (playerDistance > 2000f)
            {
                Projectile.position.X = Main.player[Projectile.owner].Center.X - (float)(Projectile.width / 2);
                Projectile.position.Y = Main.player[Projectile.owner].Center.Y - (float)(Projectile.height / 2);
                Projectile.netUpdate = true;
            }
            if (playerDistance > 70f)
            {
                playerDirection.Normalize();
                playerDirection *= returnSpeed;
                Projectile.velocity = (Projectile.velocity * 40f + playerDirection) / 41f;
            }
            else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
            {
                Projectile.velocity.X = -0.18f;
                Projectile.velocity.Y = -0.08f;
            }
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (Projectile.ai[1] > 160f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }

            // Prevent firing immediately
            if (Projectile.localAI[0] < 120f)
                Projectile.localAI[0] += 1f;

            if (Projectile.ai[0] == 0f)
            {
                float fireballShootSpeed = 14f;
                int projID = ModContent.ProjectileType<BrimstoneFireballMinion>();
                if (canAttack && Projectile.ai[1] == 0f && Projectile.localAI[0] >= 120f)
                {
                    Projectile.ai[1] += 1f;
                    if (Main.myPlayer == Projectile.owner && Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, targetCenter, 0, 0))
                    {
                        Vector2 fireballshootVelocity = Projectile.SafeDirectionTo(targetCenter) * fireballShootSpeed;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, fireballshootVelocity, projID, Projectile.damage, 0f, Projectile.owner);
                        Projectile.netUpdate = true;
                    }
                }
            }
        }
        public override bool? CanDamage()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.brimstoneWaifuVanity || modPlayer.allWaifusVanity)
            {
                return false;
            }
            else
            {
                return null;
            }
        }
    }
}
