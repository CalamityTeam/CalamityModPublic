using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityMod.Projectiles.Summon
{
    public class ApexShark : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/Ranged/SandyWaifuShark";

        private int HitCooldown = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
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
            if (HitCooldown > 0)
                HitCooldown--;
            Player player = Main.player[Projectile.owner];
            if (Projectile.localAI[0] == 0f)
            {
                int constant = 36;
                for (int i = 0; i < constant; i++)
                {
                    Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    rotate = rotate.RotatedBy((double)((float)(i - (constant / 2 - 1)) * 6.28318548f / (float)constant), default) + Projectile.Center;
                    Vector2 faceDirection = rotate - Projectile.Center;
                    int ancientDust = Dust.NewDust(rotate + faceDirection, 0, 0, 85, faceDirection.X * 1.75f, faceDirection.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[ancientDust].noGravity = true;
                    Main.dust[ancientDust].velocity = faceDirection;
                }
                Projectile.localAI[0]++;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 7)
            {
                Projectile.frame = 0;
            }
            float maxTargetDist = 1200f;
            bool isProjectile = Projectile.type == ModContent.ProjectileType<ApexShark>();

            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<AncientMineralSharkBuff>(), 3600);
            if (isProjectile)
            {
                if (player.dead)
                {
                    modPlayer.apexShark = false;
                }
                if (modPlayer.apexShark)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.MinionAntiClump();
            bool decelerate = false;
            if (Projectile.ai[0] == 2f)
            {
                Projectile.ai[1] += 1f;
                Projectile.extraUpdates = 1;
                if (Projectile.ai[1] > 60f)
                {
                    Projectile.ai[1] = 1f;
                    Projectile.ai[0] = 0f;
                    Projectile.extraUpdates = 0;
                    Projectile.numUpdates = 0;
                    Projectile.netUpdate = true;
                }
                else
                {
                    decelerate = true;
                }
            }
            if (decelerate)
            {
                return;
            }
            Vector2 projPos = Projectile.position;
            bool canAttack = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (!canAttack && targetDist < maxTargetDist)
                    {
                        maxTargetDist = targetDist;
                        projPos = npc.Center;
                        canAttack = true;
                    }
                }
            }
            if (!canAttack)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC nPC2 = Main.npc[i];
                    if (nPC2.CanBeChasedBy(Projectile, false))
                    {
                        float targetDist = Vector2.Distance(nPC2.Center, Projectile.Center);
                        if (!canAttack && targetDist < maxTargetDist)
                        {
                            maxTargetDist = targetDist;
                            projPos = nPC2.Center;
                            canAttack = true;
                        }
                    }
                }
            }
            float separationAnxietyRange = 1500f;
            if (canAttack)
            {
                separationAnxietyRange = 2200f;
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > separationAnxietyRange)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            if (canAttack && Projectile.ai[0] == 0f)
            {
                Vector2 projDirection = projPos - Projectile.Center;
                float projDistance = projDirection.Length();
                projDirection.Normalize();
                if (projDistance > 200f)
                {
                    float scaleFactor2 = 13f; //8
                    projDirection *= scaleFactor2;
                    Projectile.velocity = (Projectile.velocity * 40f + projDirection) / 41f;
                }
                else
                {
                    projDirection *= -6f;
                    Projectile.velocity = (Projectile.velocity * 40f + projDirection) / 41f;
                }
            }
            else
            {
                bool isReturning = false;
                if (!isReturning)
                {
                    isReturning = Projectile.ai[0] == 1f;
                }
                float returnSpeed = 10f;
                if (isReturning)
                {
                    returnSpeed = 21f;
                }
                Vector2 center2 = Projectile.Center;
                Vector2 playerDirection = player.Center - center2 + new Vector2(0f, -60f);
                float playerDistance = playerDirection.Length();
                if (playerDistance > 200f && returnSpeed < 8f)
                {
                    returnSpeed = 1f;
                }
                if (playerDistance < 150f && isReturning && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
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
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (Projectile.ai[1] > 60f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == 0f)
            {
                if (Projectile.ai[1] == 0f && canAttack && maxTargetDist < 500f)
                {
                    Projectile.ai[1] += 1f;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.ai[0] = 2f;
                        Vector2 npcCenter = projPos - Projectile.Center;
                        npcCenter.Normalize();
                        Projectile.velocity = npcCenter * 13f; //8
                        Projectile.netUpdate = true;
                    }
                }
            }

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 90);

            SoundEngine.PlaySound(SoundID.Item70 with { Volume = SoundID.Item70.Volume * 0.5f}, Projectile.Center);
            Projectile.velocity = new Vector2(0f, 5f).RotatedBy(Projectile.velocity.ToRotation() + 1.5f);
            int sand = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SandExplosion>(), (int)(Projectile.damage * 0.7f), (int)(Projectile.knockBack * 0.7f), Projectile.owner, 0f, 0f);
            Main.projectile[sand].Center = Projectile.Center;
            Main.projectile[sand].DamageType = DamageClass.Summon;
            Projectile.netUpdate = true;
            HitCooldown = 20;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (HitCooldown == 0)
            {
                return null;
            }
            return false;
        }
    }
}
