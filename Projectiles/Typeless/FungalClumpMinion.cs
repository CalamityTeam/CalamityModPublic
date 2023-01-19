using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Healing;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class FungalClumpMinion : ModProjectile
    {
        private bool returnToPlayer = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fungal Clump");
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            bool correctMinion = Projectile.type == ModContent.ProjectileType<FungalClumpMinion>();
            if (!modPlayer.fungalClump && !modPlayer.fungalClumpVanity)
            {
                Projectile.active = false;
                return;
            }
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.fClump = false;
                }
                if (modPlayer.fClump)
                {
                    Projectile.timeLeft = 2;
                }
            }

			Projectile.damage = (int)player.GetBestClassDamage().ApplyTo(Projectile.originalDamage);

            //Animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            //Initializing dust and damage
            if (Projectile.localAI[0] == 0f)
            {
                int dustAmt = 36;
                for (int i = 0; i < dustAmt; i++)
                {
                    Vector2 spawnPos = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    spawnPos = spawnPos.RotatedBy((double)((float)(i - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 velocity = spawnPos - Projectile.Center;
                    int idx = Dust.NewDust(spawnPos + velocity, 0, 0, 56, velocity.X * 1.5f, velocity.Y * 1.5f, 100, default, 1.4f);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].noLight = true;
                    Main.dust[idx].velocity = velocity;
                }
                Projectile.localAI[0] += 1f;
            }

            //Periodically create dust
            if (Main.rand.NextBool(16))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 56, Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f);
            }

            //Anti-sticky movement failsafe
            Projectile.MinionAntiClump();

            //If too far from player, increase speed to chase after player
            float playerRange = 500f;
            //Range is boosted if chasing after an enemy
            if (Projectile.ai[1] != 0f || Projectile.friendly)
                playerRange = 1400f;
            if (Math.Abs(Projectile.Center.X - player.Center.X) + Math.Abs(Projectile.Center.Y - player.Center.Y) > playerRange)
                returnToPlayer = true;

            //Find an npc to target, or if minion targetting is used, choose that npc
            Vector2 targetVec = Projectile.Center;
            float range = 900f;
            bool npcFound = false;
            Vector2 half = new Vector2(0.5f);
            if (!returnToPlayer && !modPlayer.fungalClumpVanity)
            {
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        //Check the size of the target to make it easier to hit fat targets like Levi
                        Vector2 sizeCheck = npc.position + npc.Size * half;
                        float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                        //Some minions will ignore tiles when choosing a target like Ice Claspers, others will not
                        bool canHit = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        if (!npcFound && targetDist < range && canHit)
                        {
                            range = targetDist;
                            targetVec = sizeCheck;
                            npcFound = true;
                        }
                    }
                }
                //If no npc is specifically targetted, check through the entire array
                if (!npcFound)
                {
                    for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                    {
                        NPC npc = Main.npc[npcIndex];
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            Vector2 sizeCheck = npc.position + npc.Size * half;
                            float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                            bool canHit = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                            if (!npcFound && targetDist < range && canHit)
                            {
                                range = targetDist;
                                targetVec = sizeCheck;
                                npcFound = true;
                            }
                        }
                    }
                }
            }

            //Tile collision depends on if returning to the player or not
            Projectile.tileCollide = !returnToPlayer;

            if (!npcFound)
            {
                Projectile.friendly = true;
                float homingSpeed = 8f;
                float turnSpeed = 20f;
                if (returnToPlayer) //move faster if returning to the player
                    homingSpeed = 12f;
                Vector2 playerVector = player.Center - Projectile.Center;
                playerVector.Y -= 60f;
                float playerDist = playerVector.Length();
                if (playerDist < 100f && returnToPlayer && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    returnToPlayer = false;
                if (playerDist > 2000f)
                {
                    Projectile.position.X = player.Center.X - Projectile.width / 2;
                    Projectile.position.Y = player.Center.Y - Projectile.width / 2;
                }
                //If more than 70 pixels away, move toward the player
                if (playerDist > 70f)
                {
                    playerVector.Normalize();
                    playerVector *= homingSpeed;
                    Projectile.velocity = (Projectile.velocity * turnSpeed + playerVector) / (turnSpeed + 1f);
                }
                //Minions never stay still
                else
                {
                    if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                    {
                        Projectile.velocity.X = -0.15f;
                        Projectile.velocity.Y = -0.05f;
                    }
                    Projectile.velocity *= 1.01f;
                }
                Projectile.friendly = false;
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                if (Math.Abs(Projectile.velocity.X) <= 0f)
                    return;
                Projectile.spriteDirection = -Projectile.direction;
            }
            else
            {
                if (Projectile.ai[1] == -1f)
                    Projectile.ai[1] = 17f;
                if (Projectile.ai[1] > 0f)
                {
                    Projectile.ai[1] -= 1f;
                }
                if (Projectile.ai[1] == 0f)
                {
                    Projectile.friendly = true;
                    float minionSpeed = 8f;
                    float turnSpeed = 14f;
                    float targetDist = Projectile.Distance(targetVec);
                    if (targetDist < 100f)
                        minionSpeed = 10f;

                    Vector2 homingVelocity = Projectile.SafeDirectionTo(targetVec) * minionSpeed;
                    Projectile.velocity = (Projectile.velocity * turnSpeed + homingVelocity) / (turnSpeed + 1f);
                }
                else
                {
                    Projectile.friendly = false;
                    if (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y) < 10f)
                        Projectile.velocity *= 1.05f;
                }
                Projectile.rotation = Projectile.velocity.X * 0.05f;
                if (Math.Abs(Projectile.velocity.X) <= 0.2f)
                    return;
                Projectile.spriteDirection = -Projectile.direction;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!target.canGhostHeal)
                return;

            float healAmt = damage * 0.25f;
            if ((int)healAmt == 0)
                return;

            if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                return;

            if (healAmt > CalamityMod.lifeStealCap)
                healAmt = CalamityMod.lifeStealCap;

            CalamityGlobalProjectile.SpawnLifeStealProjectile(Projectile, Main.player[Projectile.owner], healAmt, ModContent.ProjectileType<FungalHeal>(), 1200f, 3f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public override bool? CanDamage()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.fungalClumpVanity)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
