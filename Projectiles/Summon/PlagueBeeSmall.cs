using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PlagueBeeSmall : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Rogue/PlaguenadeBee";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plague Bee");
            Main.projFrames[projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 420;
            projectile.ignoreWater = true;
            projectile.minion = true;
        }

        public override void AI()
        {
            if (projectile.owner != Main.myPlayer)
                projectile.Kill();

            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            projectile.rotation += projectile.spriteDirection * MathHelper.ToRadians(45f);

            projectile.frameCounter++;
            if (projectile.frameCounter >= 3)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            Vector2 center = projectile.Center;
            float maxDistance = 800f;
            bool homeIn = false;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 30f)
            {
                Player player = Main.player[projectile.owner];
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(projectile, false) && !npc.wet)
                    {
                        float extraDistance = (npc.width / 2) + (npc.height / 2);

                        bool canHit = true;
                        if (extraDistance < maxDistance)
                            canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);

                        if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance) && canHit)
                        {
                            center = npc.Center;
                            homeIn = true;
                        }
                    }
                }
                else
                {
                    for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                    {
                        NPC npc = Main.npc[npcIndex];
                        if (npc.CanBeChasedBy(projectile, false) && !npc.wet)
                        {
                            float extraDistance = (npc.width / 2) + (npc.height / 2);

                            bool canHit = true;
                            if (extraDistance < maxDistance)
                                canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);

                            if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance) && canHit)
                            {
                                center = npc.Center;
                                homeIn = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!homeIn)
            {
                center.X = projectile.Center.X + projectile.velocity.X * 100f;
                center.Y = projectile.Center.Y + projectile.velocity.Y * 100f;
            }
            float speed = 10f;
            float velocityTweak = 0.14f;
            Vector2 projPos = projectile.Center;
            Vector2 velocity = center - projPos;
            float targetDist = velocity.Length();
            targetDist = speed / targetDist;
            velocity.X *= targetDist;
            velocity.Y *= targetDist;
            if (projectile.velocity.X < velocity.X)
            {
                projectile.velocity.X += velocityTweak;
                if (projectile.velocity.X < 0f && velocity.X > 0f)
                {
                    projectile.velocity.X += velocityTweak * 2f;
                }
            }
            else if (projectile.velocity.X > velocity.X)
            {
                projectile.velocity.X -= velocityTweak;
                if (projectile.velocity.X > 0f && velocity.X < 0f)
                {
                    projectile.velocity.X -= velocityTweak * 2f;
                }
            }
            if (projectile.velocity.Y < velocity.Y)
            {
                projectile.velocity.Y += velocityTweak;
                if (projectile.velocity.Y < 0f && velocity.Y > 0f)
                {
                    projectile.velocity.Y += velocityTweak * 2f;
                }
            }
            else if (projectile.velocity.Y > velocity.Y)
            {
                projectile.velocity.Y -= velocityTweak;
                if (projectile.velocity.Y > 0f && velocity.Y < 0f)
                {
                    projectile.velocity.Y -= velocityTweak * 2f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int drawStart = frameHeight * projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, frameHeight)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)frameHeight / 2f), projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 2; i++)
            {
                int plague = Dust.NewDust(projectile.position, projectile.width, projectile.height, 89, projectile.velocity.X, projectile.velocity.Y, 50, default, 1f);
                Main.dust[plague].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 60);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 60);
        }
    }
}
