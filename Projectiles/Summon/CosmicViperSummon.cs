using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class CosmicViperSummon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Viper");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            Main.projFrames[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 48;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.aiStyle = 66;
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
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    int dustType = Main.rand.NextBool(3) ? 56 : 242;
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int dusty = Dust.NewDust(vector6 + vector7, 0, 0, dustType, vector7.X * 1.75f, vector7.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].velocity = vector7;
                }
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter >= 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 3)
            {
                projectile.frame = 0;
            }

            bool correctMinion = projectile.type == ModContent.ProjectileType<CosmicViperSummon>();
            player.AddBuff(ModContent.BuffType<CosmicViperEngineBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.cosmicViper = false;
                }
                if (modPlayer.cosmicViper)
                {
                    projectile.timeLeft = 2;
                }
            }

            float colorScale = (float)projectile.alpha / 255f;
            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 1f * colorScale, 0.1f * colorScale, 1f * colorScale);

            projectile.MinionAntiClump();

            float detectRange = 1100f;
            Vector2 targetVec = projectile.position;
            bool foundTarget = false;
            int targetIndex = -1;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float extraDist = (npc.width / 2) + (npc.height / 2);
                    //Calculate distance between target and the projectile to know if it's too far or not
                    float targetDist = Vector2.Distance(npc.Center, projectile.Center);
                    if (!foundTarget && targetDist < (detectRange + extraDist))
                    {
                        detectRange = targetDist;
                        targetVec = npc.Center;
                        foundTarget = true;
                        targetIndex = npc.whoAmI;
                    }
                }
            }
            if (!foundTarget)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float extraDist = (npc.width / 2) + (npc.height / 2);
                        //Calculate distance between target and the projectile to know if it's too far or not
                        float targetDist = Vector2.Distance(npc.Center, projectile.Center);
                        if (!foundTarget && targetDist < (detectRange + extraDist))
                        {
                            detectRange = targetDist;
                            targetVec = npc.Center;
                            foundTarget = true;
                            targetIndex = npcIndex;
                        }
                    }
                }
            }
            float returnDist = 1300f;
            if (foundTarget)
            {
                returnDist = 2600f;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > returnDist)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }
            if (foundTarget && projectile.ai[0] == 0f)
            {
                Vector2 targetVector = targetVec - projectile.Center;
                float targetDist = targetVector.Length();
                targetVector.Normalize();
                float speedMult = 18f; //12
                if (targetDist > 200f)
                {
                    targetVector *= speedMult;
                    projectile.velocity = (projectile.velocity * 40f + targetVector) / 41f;
                }
                else
                {
                    targetVector *= -(speedMult / 2);
                    projectile.velocity = (projectile.velocity * 40f + targetVector) / 41f;
                }
            }
            else
            {
                float safeDist = 600f;
                bool returnToPlayer = false;
                if (!returnToPlayer)
                {
                    returnToPlayer = projectile.ai[0] == 1f;
                }
                float velocityMult = 12f;
                if (returnToPlayer)
                {
                    velocityMult = 30f;
                }
                Vector2 playerVec = player.Center - projectile.Center + new Vector2(0f, -120f);
                float playerDist = playerVec.Length();
                if (playerDist > 200f && velocityMult < 16f)
                {
                    velocityMult = 16f;
                }
                if (playerDist < safeDist && returnToPlayer)
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                if (playerDist > 2000f)
                {
                    projectile.Center = player.Center;
                    projectile.netUpdate = true;
                }
                if (playerDist > 70f)
                {
                    playerVec.Normalize();
                    playerVec *= velocityMult;
                    projectile.velocity = (projectile.velocity * 40f + playerVec) / 41f;
                }
                else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                {
                    projectile.velocity.X = -0.15f;
                    projectile.velocity.Y = -0.05f;
                }
            }
            if (foundTarget)
            {
                projectile.rotation = projectile.rotation.AngleTowards(projectile.AngleTo(targetVec) + MathHelper.Pi, 0.1f);
            }
            else
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi;
                int dustType = Main.rand.NextBool(3) ? 56 : 242;
                float xVelOffset = projectile.velocity.X / 3f;
                float yVelOffset = projectile.velocity.Y / 3f;
                int trail = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[trail];
                dust.position.X = projectile.Center.X - xVelOffset;
                dust.position.Y = projectile.Center.Y - yVelOffset;
                dust.velocity *= 0f;
                dust.scale = 0.5f;
            }
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += (float)Main.rand.Next(1, 5);
            }
            if (projectile.ai[1] > 90f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }
            if (projectile.ai[0] == 0f)
            {
                float speedMult = 6f;
                if (foundTarget && projectile.ai[1] == 0f)
                {
                    //play cool sound
                    Main.PlaySound(SoundID.Item20, projectile.Center);
                    projectile.ai[1] += 2f;
                    if (Main.myPlayer == projectile.owner)
                    {
                        int projType;
                        float dmgMult;
                        if (Main.rand.NextBool(5))
                        {
                            projType = ModContent.ProjectileType<CosmicViperSplittingRocket>();
                            dmgMult = 0.75f;
                        }
                        else if (Main.rand.NextBool(3))
                        {
                            projType = ModContent.ProjectileType<CosmicViperHomingRocket>();
                            dmgMult = 1f;
                        }
                        else
                        {
                            projType = ModContent.ProjectileType<CosmicViperConcussionMissile>();
                            dmgMult = 1.5f;
                        }

                        Vector2 velocity = targetVec - projectile.Center;
                        velocity.Normalize();
                        velocity *= speedMult;

                        //add some inaccuracy
                        velocity.Y += Main.rand.NextFloat(-30f, 30f) * 0.05f;
                        velocity.X += Main.rand.NextFloat(-30f, 30f) * 0.05f;

                        Projectile.NewProjectile(projectile.Center, velocity, projType, (int)(projectile.damage * dmgMult), projectile.knockBack, projectile.owner, targetIndex, 0f);
                        projectile.netUpdate = true;
                    }
                }
            }
        }

        public override bool CanDamage() => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int y6 = frameHeight * projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, frameHeight)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Summon/CosmicViperGlow");
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int y6 = frameHeight * projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, frameHeight)), Color.White, projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), projectile.scale, spriteEffects, 0f);
        }
    }
}
