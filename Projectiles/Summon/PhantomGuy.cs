using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Summon
{
    public class PhantomGuy : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/NPCs/Polterghast/PhantomFuckYou";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.extraUpdates = 1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public int shootTimeCounter = 0;
        bool canShoot = false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (Projectile.localAI[0] == 0f)
            {
                int dustAmt = 36;
                for (int i = 0; i < dustAmt; i++)
                {
                    Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.75f;
                    rotate = rotate.RotatedBy((i - (dustAmt / 2 - 1)) * MathHelper.TwoPi / dustAmt) + Projectile.Center;
                    Vector2 faceDirection = rotate - Projectile.Center;
                    int dust = Dust.NewDust(rotate + faceDirection, 0, 0, 180, faceDirection.X * 1.75f, faceDirection.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity = faceDirection;
                }
                Projectile.localAI[0] += 1f;
            }
            bool isMinion = Projectile.type == ModContent.ProjectileType<PhantomGuy>();
            player.AddBuff(ModContent.BuffType<Phantom>(), 3600);
            if (isMinion)
            {
                if (player.dead)
                {
                    modPlayer.pGuy = false;
                }
                if (modPlayer.pGuy)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.MinionAntiClump();
            float attackDistance = 3000f;
            Vector2 targetCenter = Projectile.position;
            bool canAttack = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (targetDist < attackDistance)
                    {
                        attackDistance = targetDist;
                        targetCenter = npc.Center;
                        canAttack = true;
                    }
                }
            }
            if (!canAttack)
            {
                for (int j = 0; j < Main.maxNPCs; j++)
                {
                    NPC nPC2 = Main.npc[j];
                    if (nPC2.CanBeChasedBy(Projectile, false))
                    {
                        float targetDist = Vector2.Distance(nPC2.Center, Projectile.Center);
                        if (!canAttack && targetDist < attackDistance)
                        {
                            attackDistance = targetDist;
                            targetCenter = nPC2.Center;
                            canAttack = true;
                        }
                    }
                }
            }
            float separationAnxietyDist = 3500f;
            if (canAttack)
            {
                separationAnxietyDist = 4000f;
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > separationAnxietyDist)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            if (canAttack && Projectile.ai[0] == 0f)
            {
                Vector2 targetDirection = targetCenter - Projectile.Center;
                float targetDistance = targetDirection.Length();
                targetDirection.Normalize();
                if (targetDistance > 200f)
                {
                    float scaleFactor2 = 18f;
                    targetDirection *= scaleFactor2;
                    Projectile.velocity = (Projectile.velocity * 40f + targetDirection) / 41f;
                }
                else
                {
                    targetDirection *= -12f;
                    Projectile.velocity = (Projectile.velocity * 40f + targetDirection) / 41f;
                }
            }
            else
            {
                bool isReturning = false;
                if (!isReturning)
                {
                    isReturning = Projectile.ai[0] == 1f;
                }
                float returnSpeed = 12f;
                if (isReturning)
                {
                    returnSpeed = 30f;
                }
                Vector2 center2 = Projectile.Center;
                Vector2 playerDirection = player.Center - center2 + new Vector2(0f, -120f);
                float playerDist = playerDirection.Length();
                if (playerDist > 200f && returnSpeed < 16f)
                {
                    returnSpeed = 16f;
                }
                if (playerDist < 600f && isReturning)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                if (playerDist > 3500f)
                {
                    Projectile.position.X = player.Center.X - (Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (Projectile.height / 2);
                    Projectile.netUpdate = true;
                }
                if (playerDist > 70f)
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
            if (canAttack)
            {
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(targetCenter), 0.1f);
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += Main.rand.Next(1, 3);
            }
            if (Projectile.ai[1] > 75f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == 0f)
            {
                if (canAttack && Projectile.ai[1] == 0f)
                {
                    Projectile.ai[1] += 1f;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        canShoot = true;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (canShoot)
            {
                shootTimeCounter++;
                
                if (shootTimeCounter % 20 == 0 && shootTimeCounter <= 60)
                {
                    SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
                    float randomRadius = Main.rand.Next(10, 14);
                    Vector2 randomVelocity = Main.rand.NextVector2CircularEdge(randomRadius, randomRadius);
                    Projectile.velocity -= randomVelocity*0.22f; //funny recoil
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, randomVelocity.X, randomVelocity.Y, ModContent.ProjectileType<GhostFire>(), Projectile.damage, 0f, Main.myPlayer, 0f, 0f);
                    Projectile.netUpdate = true;
                } 
                if (shootTimeCounter > 200)
                {
                    canShoot = false;
                    shootTimeCounter = 0;
                    Projectile.netUpdate = true;
                }
                
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(100, 250, 250, Projectile.alpha);
        }

        public override bool? CanDamage() => false;
    }
}
