using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class PinkButterfly : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0.5f;
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
            if (Projectile.localAI[0] == 0f)
            {
                int dustAmt = 36;
                for (int i = 0; i < dustAmt; i++)
                {
                    Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.5f;
                    rotate = rotate.RotatedBy((double)((float)(i - (dustAmt / 2 - 1)) * 6.28318548f / (float)dustAmt), default) + Projectile.Center;
                    Vector2 faceDirection = rotate - Projectile.Center;
                    int dust = Dust.NewDust(rotate + faceDirection, 0, 0, 73, faceDirection.X, faceDirection.Y, 100, default, 1.1f);
                    Main.dust[dust].noGravity = true;
                }
                Projectile.localAI[0] += 1f;
            }
            if (Math.Abs(Projectile.velocity.X) > 0.2f)
            {
                Projectile.spriteDirection = -Projectile.direction;
            }
            Projectile.rotation = Projectile.velocity.X * 0.02f;
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
            {
                Projectile.frame = 0;
            }
            Lighting.AddLight(Projectile.Center, 0.3f, 0.2f, 0.3f);
            float attackRange = 1200f;
            bool isMinion = Projectile.type == ModContent.ProjectileType<PinkButterfly>();
            player.AddBuff(ModContent.BuffType<ResurrectionButterflyBuff>(), 3600);
            if (isMinion)
            {
                if (player.dead)
                {
                    modPlayer.resButterfly = false;
                }
                if (modPlayer.resButterfly)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.MinionAntiClump();
            bool returnbool = false;
            if (returnbool)
            {
                return;
            }
            Vector2 projPos = Projectile.position;
            bool canAttack = false;
            int targetIndex = -1;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if ((npc.CanBeChasedBy(Projectile, false) || npc.type == NPCID.DukeFishron) && npc.active)
                {
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (!canAttack && targetDist < attackRange)
                    {
                        projPos = npc.Center;
                        canAttack = true;
                        targetIndex = npc.whoAmI;
                    }
                }
            }
            if (!canAttack)
            {
                for (int j = 0; j < Main.maxNPCs; j++)
                {
                    NPC nPC2 = Main.npc[j];
                    if ((nPC2.CanBeChasedBy(Projectile, false) || nPC2.type == NPCID.DukeFishron) && nPC2.active)
                    {
                        float targetDist = Vector2.Distance(nPC2.Center, Projectile.Center);
                        if (!canAttack && targetDist < attackRange)
                        {
                            attackRange = targetDist;
                            projPos = nPC2.Center;
                            canAttack = true;
                            targetIndex = j;
                        }
                    }
                }
            }
            float separationAnxietyDist = 1500f;
            if (canAttack)
            {
                separationAnxietyDist = 2400f;
            }
            if (Vector2.Distance(player.Center, Projectile.Center) > separationAnxietyDist)
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
                    float scaleFactor2 = 12f;
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
                    returnSpeed = 24f;
                }
                Vector2 center2 = Projectile.Center;
                Vector2 playerDirection = player.Center - center2 + new Vector2(-40f, -40f);
                float playerDist = playerDirection.Length();
                if (playerDist > 200f && returnSpeed < 12f)
                {
                    returnSpeed = 12f;
                }
                if (playerDist < 150f && isReturning && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                if (playerDist > 2000f)
                {
                    Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
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

            // Projectile fire timer
            if (Projectile.ai[1] > 0f)
                Projectile.ai[1] += (float)Main.rand.Next(1, 3);

            // Reset timer
            if (Projectile.ai[1] > 300f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }

            // Prevent firing immediately
            if (Projectile.localAI[0] < 120f)
                Projectile.localAI[0] += 1f;

            // Fire projectiles
            if (Projectile.ai[0] == 0f)
            {
                float velocity = 8f;
                int projectileType = ModContent.ProjectileType<SakuraBullet>();
                if (canAttack && Projectile.ai[1] == 0f && Projectile.localAI[0] >= 120f)
                {
                    Projectile.ai[1] += 1f;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        SoundEngine.PlaySound(SoundID.Item8, Projectile.position);
                        Vector2 projDirectionAgain = projPos - Projectile.Center;
                        projDirectionAgain.Normalize();
                        projDirectionAgain *= velocity;
                        int numProj = 2;
                        float rotation = MathHelper.ToRadians(4);
                        for (int i = 0; i < numProj + 1; i++)
                        {
                            Vector2 perturbedSpeed = projDirectionAgain.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, perturbedSpeed, projectileType, Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner, targetIndex, 0f);
                        }
                        Projectile.netUpdate = true;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture2D13 = ModContent.Request<Texture2D>(Texture).Value;
            int framing = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int y6 = framing * Projectile.frame;
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, framing)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)framing / 2f), Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override bool? CanDamage() => false;
    }
}
