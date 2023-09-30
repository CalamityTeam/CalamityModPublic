using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PlagueBeeSmall : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/Rogue/PlaguenadeBee";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.timeLeft = 420;
            Projectile.ignoreWater = true;
            Projectile.MaxUpdates = 2;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            if (Projectile.owner != Main.myPlayer)
                Projectile.Kill();

            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            Projectile.rotation += Projectile.spriteDirection * MathHelper.ToRadians(45f);

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            Vector2 center = Projectile.Center;
            float maxDistance = 800f;
            bool homeIn = false;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 30f)
            {
                Player player = Main.player[Projectile.owner];
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false) && !npc.wet)
                    {
                        float extraDistance = (npc.width / 2) + (npc.height / 2);

                        bool canHit = true;
                        if (extraDistance < maxDistance)
                            canHit = Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1);

                        if (Vector2.Distance(npc.Center, Projectile.Center) < (maxDistance + extraDistance) && canHit)
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
                        if (npc.CanBeChasedBy(Projectile, false) && !npc.wet)
                        {
                            float extraDistance = (npc.width / 2) + (npc.height / 2);

                            bool canHit = true;
                            if (extraDistance < maxDistance)
                                canHit = Collision.CanHit(Projectile.Center, 1, 1, npc.Center, 1, 1);

                            if (Vector2.Distance(npc.Center, Projectile.Center) < (maxDistance + extraDistance) && canHit)
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
                center.X = Projectile.Center.X + Projectile.velocity.X * 100f;
                center.Y = Projectile.Center.Y + Projectile.velocity.Y * 100f;
            }
            float speed = 15f;
            float velocityTweak = 0.14f;
            Vector2 projPos = Projectile.Center;
            Vector2 velocity = center - projPos;
            float targetDist = velocity.Length();
            targetDist = speed / targetDist;
            velocity.X *= targetDist;
            velocity.Y *= targetDist;
            if (Projectile.velocity.X < velocity.X)
            {
                Projectile.velocity.X += velocityTweak;
                if (Projectile.velocity.X < 0f && velocity.X > 0f)
                {
                    Projectile.velocity.X += velocityTweak * 2f;
                }
            }
            else if (Projectile.velocity.X > velocity.X)
            {
                Projectile.velocity.X -= velocityTweak;
                if (Projectile.velocity.X > 0f && velocity.X < 0f)
                {
                    Projectile.velocity.X -= velocityTweak * 2f;
                }
            }
            if (Projectile.velocity.Y < velocity.Y)
            {
                Projectile.velocity.Y += velocityTweak;
                if (Projectile.velocity.Y < 0f && velocity.Y > 0f)
                {
                    Projectile.velocity.Y += velocityTweak * 2f;
                }
            }
            else if (Projectile.velocity.Y > velocity.Y)
            {
                Projectile.velocity.Y -= velocityTweak;
                if (Projectile.velocity.Y > 0f && velocity.Y < 0f)
                {
                    Projectile.velocity.Y -= velocityTweak * 2f;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int drawStart = frameHeight * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, frameHeight)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture.Width / 2f, (float)frameHeight / 2f), Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 2; i++)
            {
                int plague = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 89, Projectile.velocity.X, Projectile.velocity.Y, 50, default, 1f);
                Main.dust[plague].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Plague>(), 60);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Plague>(), 60);
    }
}
