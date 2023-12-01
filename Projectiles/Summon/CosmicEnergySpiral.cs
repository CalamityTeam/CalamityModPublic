using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class CosmicEnergySpiral : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        private bool justSpawned = true;

        public override void SetStaticDefaults()
        {
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 78;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 10f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, (float)Main.DiscoR / 255f, (float)Main.DiscoG / 255f, (float)Main.DiscoB / 255f);
            bool isMinion = Projectile.type == ModContent.ProjectileType<CosmicEnergySpiral>();
            player.AddBuff(ModContent.BuffType<CosmicEnergy>(), 3600);
            if (isMinion)
            {
                if (player.dead)
                {
                    modPlayer.cEnergy = false;
                }
                if (modPlayer.cEnergy)
                {
                    Projectile.timeLeft = 2;
                }
            }
            float targetDist = 1400f; //700
            Projectile.rotation += Projectile.velocity.X * 0.1f;
            Vector2 projPos = Projectile.position;
            bool canAttack = false;
            int target = 0;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float maxTargetDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (!canAttack && maxTargetDist < targetDist)
                    {
                        projPos = npc.Center;
                        canAttack = true;
                        target = npc.whoAmI;
                    }
                }
            }
            else
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC nPC2 = Main.npc[i];
                    if (nPC2.CanBeChasedBy(Projectile, false))
                    {
                        float maxTargetDist = Vector2.Distance(nPC2.Center, Projectile.Center);
                        if (!canAttack && maxTargetDist < targetDist)
                        {
                            targetDist = maxTargetDist;
                            projPos = nPC2.Center;
                            canAttack = true;
                            target = i;
                        }
                    }
                }
            }
            float separationAnxietyDist = 1600f;
            if (canAttack)
            {
                separationAnxietyDist = 2400f;
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > separationAnxietyDist)
            {
                Projectile.ai[1] = 1f;
                Projectile.netUpdate = true;
            }
            if (canAttack && Projectile.ai[1] == 0f)
            {
                Vector2 projDirection = projPos - Projectile.Center;
                float projDistance = projDirection.Length();
                projDirection.Normalize();
                if (projDistance > 200f)
                {
                    float scaleFactor2 = 6f; //6
                    projDirection *= scaleFactor2;
                    Projectile.velocity = (Projectile.velocity * 40f + projDirection) / 41f;
                }
                else
                {
                    projDirection *= -4f;
                    Projectile.velocity = (Projectile.velocity * 40f + projDirection) / 41f;
                }
            }
            else
            {
                bool isReturning = false;
                if (!isReturning)
                {
                    isReturning = Projectile.ai[1] == 1f;
                }
                float returnSpeed = 6f;
                if (isReturning)
                {
                    returnSpeed = 15f;
                }
                Vector2 center2 = Projectile.Center;
                Vector2 playerDirection = player.Center - center2 + new Vector2(0f, -60f);
                float playerDistance = playerDirection.Length();
                if (playerDistance > 200f && returnSpeed < 8f)
                {
                    returnSpeed = 8f;
                }
                if (playerDistance < 800f && isReturning && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[1] = 0f;
                    Projectile.netUpdate = true;
                }
                if (playerDistance > 2000f) //2000
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
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
            float projScale = (float)Main.mouseTextColor / 200f - 0.35f;
            projScale *= 0.2f;
            Projectile.scale = projScale + 0.95f;
            if (justSpawned)
            {
                justSpawned = false;
                Projectile.ai[0] = 100f;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[0] != 0f)
                {
                    Projectile.ai[0] -= 1f;
                    return;
                }
                float projX = Projectile.position.X;
                float projY = Projectile.position.Y;
                float homeDistance = 1200f;
                bool isInRange = false;
                for (int j = 0; j < Main.maxNPCs; j++)
                {
                    if (Main.npc[j].CanBeChasedBy(Projectile, false))
                    {
                        float npcX = Main.npc[j].position.X + (float)(Main.npc[j].width / 2);
                        float npcY = Main.npc[j].position.Y + (float)(Main.npc[j].height / 2);
                        float npcDistance = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcY);
                        if (npcDistance < homeDistance)
                        {
                            homeDistance = npcDistance;
                            projX = npcX;
                            projY = npcY;
                            isInRange = true;
                        }
                    }
                }
                if (isInRange)
                {
                    SoundEngine.PlaySound(SoundID.Item105, Projectile.position);
                    int blastAmt = Main.rand.Next(5, 8);
                    for (int b = 0; b < blastAmt; b++)
                    {
                        Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<CosmicBlast>(), (int)(Projectile.damage * 0.5), 2f, Projectile.owner, (float)target, 0f);
                    }
                    float speed = 15f;
                    float projXSpeed = projX - Projectile.Center.X;
                    float projYSpeed = projY - Projectile.Center.Y;
                    float velocityMult = (float)Math.Sqrt((double)(projXSpeed * projXSpeed + projYSpeed * projYSpeed));
                    velocityMult = speed / velocityMult;
                    projXSpeed *= velocityMult;
                    projYSpeed *= velocityMult;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, projXSpeed, projYSpeed, ModContent.ProjectileType<CosmicBlastBig>(), Projectile.damage, 3f, Projectile.owner, (float)target, 0f);
                    Projectile.ai[0] = 100f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 255);
        }

        public override bool? CanDamage() => false;
    }
}
