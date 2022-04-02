using CalamityMod.Buffs.Summon;
using CalamityMod.Dusts;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class Calamitamini : ModProjectile
    {
        public const float Range = 1300f;
        public const float SeparationAnxietyMin = 1500f;
        public const float SeparationAnxietyMax = 3200f;
        public const float SafeDist = 200f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamitamini");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 36;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 36;
                for (int d = 0; d < dustAmt; d++)
                {
                    Vector2 source = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                    Vector2 dustVel = source - projectile.Center;
                    int brim = Dust.NewDust(source + dustVel, 0, 0, (int)CalamityDusts.Brimstone, dustVel.X * 1.75f, dustVel.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[brim].noGravity = true;
                    Main.dust[brim].velocity = dustVel;
                }
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue * player.MinionDamage());
                projectile.damage = damage2;
            }

            bool correctMinion = projectile.type == ModContent.ProjectileType<Calamitamini>();
            player.AddBuff(ModContent.BuffType<CalamitasEyes>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.cEyes = false;
                }
                if (modPlayer.cEyes)
                {
                    projectile.timeLeft = 2;
                }
            }

            projectile.MinionAntiClump();

            projectile.frameCounter++;
            if (projectile.frameCounter > 3)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 2)
            {
                projectile.frame = 0;
            }

            Vector2 targetVec = projectile.position;
            float maxDistance = Range;
            bool foundTarget = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float extraDist = (npc.width / 2) + (npc.height / 2);
                    float targetDist = Vector2.Distance(npc.Center, projectile.Center);
                    bool canHit = true;
                    if (extraDist < maxDistance)
                        canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);
                    if (!foundTarget && targetDist < (maxDistance + extraDist) && canHit)
                    {
                        targetVec = npc.Center;
                        foundTarget = true;
                    }
                }
            }
            if (!foundTarget)
            {
                for (int index = 0; index < Main.maxNPCs; index++)
                {
                    NPC npc = Main.npc[index];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float extraDist = (npc.width / 2) + (npc.height / 2);
                        float targetDist = Vector2.Distance(npc.Center, projectile.Center);
                        bool canHit = true;
                        if (extraDist < maxDistance)
                            canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);
                        if (!foundTarget && targetDist < (maxDistance + extraDist) && canHit)
                        {
                            targetVec = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }
            float sepAnxietyDist = SeparationAnxietyMin;
            if (foundTarget)
            {
                sepAnxietyDist = SeparationAnxietyMax;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > sepAnxietyDist)
            {
                projectile.ai[0] = 1f;
                projectile.tileCollide = false;
                projectile.netUpdate = true;
            }
            if (foundTarget && projectile.ai[0] == 0f)
            {
                Vector2 vecToTarget = targetVec - projectile.Center;
                float targetDist = vecToTarget.Length();
                vecToTarget.Normalize();
                if (targetDist > 200f)
                {
                    float speedMult = 8f; //6
                    vecToTarget *= speedMult;
                    projectile.velocity = (projectile.velocity * 40f + vecToTarget) / 41f;
                }
                else
                {
                    float speedMult = -4f;
                    vecToTarget *= speedMult;
                    projectile.velocity = (projectile.velocity * 40f + vecToTarget) / 41f;
                }
            }
            else
            {
                bool returningToPlayer = false;
                if (!returningToPlayer)
                {
                    returningToPlayer = projectile.ai[0] == 1f;
                }
                float returnSpeed = 6f;
                if (returningToPlayer)
                {
                    returnSpeed = 18f;
                }
                Vector2 returnSpot = player.Center - projectile.Center + new Vector2(0f, -60f);
                float playerDist = returnSpot.Length();
                if (playerDist > 200f && returnSpeed < 10f)
                {
                    returnSpeed = 10f;
                }
                if (playerDist < SafeDist && returningToPlayer && !Collision.SolidCollision(projectile.Center, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                if (playerDist > 2000f)
                {
                    projectile.position.X = player.Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
                    projectile.netUpdate = true;
                }
                if (playerDist > 70f)
                {
                    returnSpot.Normalize();
                    returnSpot *= returnSpeed;
                    projectile.velocity = (projectile.velocity * 40f + returnSpot) / 41f;
                }
                else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                {
                    projectile.velocity.X = -0.15f;
                    projectile.velocity.Y = -0.05f;
                }
            }

            //Update rotation
            if (foundTarget)
            {
                projectile.rotation = projectile.rotation.AngleTowards(projectile.AngleTo(targetVec) + MathHelper.Pi, 0.1f);
            }
            else
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi;
            }

            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
            if (projectile.ai[1] > 110f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            if (projectile.ai[0] == 0f)
            {
                float projSpeed = 8f;
                int projType = ModContent.ProjectileType<BrimstoneLaserSummon>();
                if (foundTarget && projectile.ai[1] == 0f)
                {
                    projectile.ai[1] += 1f;
                    if (Main.myPlayer == projectile.owner && Collision.CanHitLine(projectile.Center, projectile.width, projectile.height, targetVec, 0, 0))
                    {
                        Vector2 velocity = targetVec - projectile.Center;
                        velocity.Normalize();
                        velocity *= projSpeed;
                        Projectile.NewProjectile(projectile.Center, velocity, projType, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                        projectile.netUpdate = true;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int yStart = frameHeight * projectile.frame;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, yStart, texture.Width, frameHeight)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override bool CanDamage() => false;
    }
}
