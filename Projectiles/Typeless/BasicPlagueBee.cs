using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class BasicPlagueBee : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/Rogue/PlaguenadeBee";

        public int time = 0;
        public float projRotValue;
        public NPC npc;
        public int tileCollisions = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override void AI()
        {
            Color trailColor = Color.Lime;
            if (time == 0)
                trailColor = Main.rand.NextBool() ? Color.LawnGreen : Color.Lime;
            time++;

            // If you would like the bees to home in sooner or later than normal, change Projectile.ai[0]! You can set it to anything above 30 to make them home in and damage immediately.
            // If you set Projectile.ai[0] to a negative number, it will take longer before they begin homing.

            // You can set Projectile.ai[1] when spawning these bees to determine how long they inflict the plague debuff, default is 90 frames.
            if (Projectile.ai[1] <= 0)
                Projectile.ai[1] = 90;

            // You can set Projectile.ai[2] to change how detailed the bee's particle trail is. Higher is less detailed, default is 2.
            // If you are spawning a lot of bees, consider raising the value to 3 or 4.
            if (Projectile.ai[2] <= 0)
                Projectile.ai[2] = 2;

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

            // If a bee bounces around a fuckload (probably got stuck in a tile) stop it from doing visuals and kill it, that bee is doomed anyways.
            if (tileCollisions < 7)
            {
                if (time % Projectile.ai[2] == 0)
                {
                    PointParticle orb = new PointParticle(Projectile.Center, -Projectile.velocity * 0.05f, false, 7, 0.35f, trailColor * 0.45f);
                    GeneralParticleHandler.SpawnParticle(orb);

                    if (Main.rand.NextBool())
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, 89, -Projectile.velocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.2f, 0.6f), 0, default, Main.rand.NextFloat(0.5f, 0.8f));
                        dust.noGravity = true;
                        dust.alpha = Main.rand.Next(90, 220 + 1);
                    }
                }
            }
            else
                Projectile.Kill();

            Vector2 center = Projectile.Center;
            float maxDistance = 800f;
            bool homeIn = false;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 30f)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    if (time % 10 == 0 && npc == null)
                        npc = Projectile.Center.ClosestNPCAt(1000, false);
                    if (npc == null || !npc.CanBeChasedBy(Projectile, false))
                        return;
                    if (npc.CanBeChasedBy(Projectile, false))
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
            if (!homeIn)
            {
                center.X = Projectile.Center.X + Projectile.velocity.X * 100f;
                center.Y = Projectile.Center.Y + Projectile.velocity.Y * 100f;
            }
            float speed = 8f;
            float velocityTweak = 0.30f;
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
            tileCollisions++;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int drawStart = frameHeight * Projectile.frame;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, frameHeight)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2((float)texture.Width / 2f, (float)frameHeight / 2f), Projectile.scale, spriteEffects, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 89, Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(0.7f, 0.85f));
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), (int)(Projectile.ai[1]));
            Projectile.ai[0] = 15;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<Plague>(), (int)(Projectile.ai[1]));

        public override bool? CanDamage() => Projectile.ai[0] <= 30f ? false : null;
    }
}
