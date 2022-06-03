using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Summon
{
    public class CosmicViperSummon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Viper");
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = 66;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (Projectile.localAI[0] == 0f)
            {
                int dustAmt = 36;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    int dustType = Main.rand.NextBool(3) ? 56 : 242;
                    Vector2 vector6 = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                    Vector2 vector7 = vector6 - Projectile.Center;
                    int dusty = Dust.NewDust(vector6 + vector7, 0, 0, dustType, vector7.X * 1.75f, vector7.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].velocity = vector7;
                }
                Projectile.localAI[0] += 1f;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 3)
            {
                Projectile.frame = 0;
            }

            bool correctMinion = Projectile.type == ModContent.ProjectileType<CosmicViperSummon>();
            player.AddBuff(ModContent.BuffType<CosmicViperEngineBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.cosmicViper = false;
                }
                if (modPlayer.cosmicViper)
                {
                    Projectile.timeLeft = 2;
                }
            }

            float colorScale = (float)Projectile.alpha / 255f;
            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 1f * colorScale, 0.1f * colorScale, 1f * colorScale);

            Projectile.MinionAntiClump();

            float detectRange = 1100f;
            Vector2 targetVec = Projectile.position;
            bool foundTarget = false;
            int targetIndex = -1;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float extraDist = (npc.width / 2) + (npc.height / 2);
                    //Calculate distance between target and the projectile to know if it's too far or not
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
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
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float extraDist = (npc.width / 2) + (npc.height / 2);
                        //Calculate distance between target and the projectile to know if it's too far or not
                        float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
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
            if (Vector2.Distance(player.Center, Projectile.Center) > returnDist)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
            }
            if (foundTarget && Projectile.ai[0] == 0f)
            {
                Vector2 targetVector = targetVec - Projectile.Center;
                float targetDist = targetVector.Length();
                targetVector.Normalize();
                float speedMult = 18f; //12
                if (targetDist > 200f)
                {
                    targetVector *= speedMult;
                    Projectile.velocity = (Projectile.velocity * 40f + targetVector) / 41f;
                }
                else
                {
                    targetVector *= -(speedMult / 2);
                    Projectile.velocity = (Projectile.velocity * 40f + targetVector) / 41f;
                }
            }
            else
            {
                float safeDist = 600f;
                bool returnToPlayer = false;
                if (!returnToPlayer)
                {
                    returnToPlayer = Projectile.ai[0] == 1f;
                }
                float velocityMult = 12f;
                if (returnToPlayer)
                {
                    velocityMult = 30f;
                }
                Vector2 playerVec = player.Center - Projectile.Center + new Vector2(0f, -120f);
                float playerDist = playerVec.Length();
                if (playerDist > 200f && velocityMult < 16f)
                {
                    velocityMult = 16f;
                }
                if (playerDist < safeDist && returnToPlayer)
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                if (playerDist > 2000f)
                {
                    Projectile.Center = player.Center;
                    Projectile.netUpdate = true;
                }
                if (playerDist > 70f)
                {
                    playerVec.Normalize();
                    playerVec *= velocityMult;
                    Projectile.velocity = (Projectile.velocity * 40f + playerVec) / 41f;
                }
                else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
            if (foundTarget)
            {
                Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(targetVec) + MathHelper.Pi, 0.1f);
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
                int dustType = Main.rand.NextBool(3) ? 56 : 242;
                float xVelOffset = Projectile.velocity.X / 3f;
                float yVelOffset = Projectile.velocity.Y / 3f;
                int trail = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 1f);
                Dust dust = Main.dust[trail];
                dust.position.X = Projectile.Center.X - xVelOffset;
                dust.position.Y = Projectile.Center.Y - yVelOffset;
                dust.velocity *= 0f;
                dust.scale = 0.5f;
            }
            if (Projectile.ai[1] > 0f)
            {
                Projectile.ai[1] += (float)Main.rand.Next(1, 5);
            }
            if (Projectile.ai[1] > 90f)
            {
                Projectile.ai[1] = 0f;
                Projectile.netUpdate = true;
            }
            if (Projectile.ai[0] == 0f)
            {
                float speedMult = 6f;
                if (foundTarget && Projectile.ai[1] == 0f)
                {
                    //play cool sound
                    SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                    Projectile.ai[1] += 2f;
                    if (Main.myPlayer == Projectile.owner)
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

                        Vector2 velocity = targetVec - Projectile.Center;
                        velocity.Normalize();
                        velocity *= speedMult;

                        //add some inaccuracy
                        velocity.Y += Main.rand.NextFloat(-30f, 30f) * 0.05f;
                        velocity.X += Main.rand.NextFloat(-30f, 30f) * 0.05f;

                        int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, projType, (int)(Projectile.damage * dmgMult), Projectile.knockBack, Projectile.owner, targetIndex, 0f);
                        if (Main.projectile.IndexInRange(p))
                            Main.projectile[p].originalDamage = (int)(Projectile.originalDamage * dmgMult);
                        Projectile.netUpdate = true;
                    }
                }
            }
        }

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int y6 = frameHeight * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, frameHeight)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), Projectile.scale, spriteEffects, 0);
            return false;
        }


        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/CosmicViperGlow").Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int y6 = frameHeight * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, frameHeight)), Color.White, Projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), Projectile.scale, spriteEffects, 0);
        }
    }
}
