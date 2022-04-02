using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class PinkButterfly : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pink Butterfly");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0.5f;
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
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.5f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 73, vector7.X, vector7.Y, 100, default, 1.1f);
                    Main.dust[num228].noGravity = true;
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
            if (Math.Abs(projectile.velocity.X) > 0.2f)
            {
                projectile.spriteDirection = -projectile.direction;
            }
            projectile.rotation = projectile.velocity.X * 0.02f;
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            Lighting.AddLight(projectile.Center, 0.3f, 0.2f, 0.3f);
            float num633 = 1200f;
            float num634 = 1500f;
            float num635 = 2400f;
            float num636 = 150f;
            bool flag64 = projectile.type == ModContent.ProjectileType<PinkButterfly>();
            player.AddBuff(ModContent.BuffType<ResurrectionButterflyBuff>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.resButterfly = false;
                }
                if (modPlayer.resButterfly)
                {
                    projectile.timeLeft = 2;
                }
            }
            projectile.MinionAntiClump();
            bool flag24 = false;
            if (flag24)
            {
                return;
            }
            Vector2 vector46 = projectile.position;
            bool flag25 = false;
            int targetIndex = -1;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if ((npc.CanBeChasedBy(projectile, false) || npc.type == NPCID.DukeFishron) && npc.active)
                {
                    float num646 = Vector2.Distance(npc.Center, projectile.Center);
                    if (!flag25 && num646 < num633)
                    {
                        vector46 = npc.Center;
                        flag25 = true;
                        targetIndex = npc.whoAmI;
                    }
                }
            }
            if (!flag25)
            {
                for (int num645 = 0; num645 < Main.maxNPCs; num645++)
                {
                    NPC nPC2 = Main.npc[num645];
                    if ((nPC2.CanBeChasedBy(projectile, false) || nPC2.type == NPCID.DukeFishron) && nPC2.active)
                    {
                        float num646 = Vector2.Distance(nPC2.Center, projectile.Center);
                        if (!flag25 && num646 < num633)
                        {
                            num633 = num646;
                            vector46 = nPC2.Center;
                            flag25 = true;
                            targetIndex = num645;
                        }
                    }
                }
            }
            float num647 = num634;
            if (flag25)
            {
                num647 = num635;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > num647)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }
            if (flag25 && projectile.ai[0] == 0f)
            {
                Vector2 vector47 = vector46 - projectile.Center;
                float num648 = vector47.Length();
                vector47.Normalize();
                if (num648 > 200f)
                {
                    float scaleFactor2 = 12f;
                    vector47 *= scaleFactor2;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
                else
                {
                    float num649 = 6f;
                    vector47 *= -num649;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
            }
            else
            {
                bool flag26 = false;
                if (!flag26)
                {
                    flag26 = projectile.ai[0] == 1f;
                }
                float num650 = 10f;
                if (flag26)
                {
                    num650 = 24f;
                }
                Vector2 center2 = projectile.Center;
                Vector2 vector48 = player.Center - center2 + new Vector2(-40f, -40f);
                float num651 = vector48.Length();
                if (num651 > 200f && num650 < 12f)
                {
                    num650 = 12f;
                }
                if (num651 < num636 && flag26 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
                if (num651 > 2000f)
                {
                    projectile.position.X = player.Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
                    projectile.netUpdate = true;
                }
                if (num651 > 70f)
                {
                    vector48.Normalize();
                    vector48 *= num650;
                    projectile.velocity = (projectile.velocity * 40f + vector48) / 41f;
                }
                else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                {
                    projectile.velocity.X = -0.15f;
                    projectile.velocity.Y = -0.05f;
                }
            }

            // Projectile fire timer
            if (projectile.ai[1] > 0f)
                projectile.ai[1] += (float)Main.rand.Next(1, 3);

            // Reset timer
            if (projectile.ai[1] > 300f)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }

            // Prevent firing immediately
            if (projectile.localAI[0] < 120f)
                projectile.localAI[0] += 1f;

            // Fire projectiles
            if (projectile.ai[0] == 0f)
            {
                float velocity = 8f;
                int projectileType = ModContent.ProjectileType<SakuraBullet>();
                if (flag25 && projectile.ai[1] == 0f && projectile.localAI[0] >= 120f)
                {
                    projectile.ai[1] += 1f;
                    if (Main.myPlayer == projectile.owner)
                    {
                        Main.PlaySound(SoundID.Item8, projectile.position);
                        Vector2 value19 = vector46 - projectile.Center;
                        value19.Normalize();
                        value19 *= velocity;
                        int numProj = 2;
                        float rotation = MathHelper.ToRadians(4);
                        for (int i = 0; i < numProj + 1; i++)
                        {
                            Vector2 perturbedSpeed = value19.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
                            Projectile.NewProjectile(projectile.Center, perturbedSpeed, projectileType, projectile.damage / 2, projectile.knockBack * 0.5f, projectile.owner, targetIndex, 0f);
                        }
                        projectile.netUpdate = true;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int y6 = num214 * projectile.frame;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
