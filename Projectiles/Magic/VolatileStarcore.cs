using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class VolatileStarcore : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        private static int Lifetime = 240;
        private static int NumAnimationFrames = 6;
        private static int AnimationFrameTime = 1;
        public bool explode = true;
        public int time = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = NumAnimationFrames;
        }
        public override void SetDefaults()
        {
            // Actual hitbox is different
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 6;
            Projectile.timeLeft = Lifetime;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center);

            if (Projectile.timeLeft == 1) // if it doesnt hit a tile or enemy, then don't explode
                explode = false;

            // Set rotation on frame 1
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            }

            // Dust only shows up after the first few frames
            if (Projectile.localAI[0] >= 5f)
                SpawnDust();

            // Lighting and spin
            Lighting.AddLight(Projectile.Center, 1.8f, 1.6f, 0.5f);

            // Increment frame counter
            Projectile.localAI[0] += 1f;

            // Update animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > AnimationFrameTime)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= NumAnimationFrames)
                Projectile.frame = 0;
            if (targetDist < 1400)
            {
                if (time > 8 && time % 2 == 0)
                {
                    SparkParticle spark = new SparkParticle(Projectile.Center + Main.rand.NextVector2Circular(15, 15) - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.8f), false, 35, 0.9f, Color.Lerp(Color.Orange, Color.OrangeRed, Main.rand.NextFloat(0, 1)));
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                if (time > 6 && time % 2 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Particle spark = new GlowSparkParticle(Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, -Projectile.velocity, false, 8, 0.13f * (i == 0 ? 0.5f : 1), Color.Lerp(Color.Red, Color.OrangeRed, 0.25f) * 0.5f, new Vector2(0.3f, 1f), false, false, 0.8f);
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
            }
            time++;
        }
        private void SpawnDust()
        {
            int trailDustCount = 3;
            int trailDustType = 303;
            for (int i = 0; i < trailDustCount; i++)
            {
                float scale = Main.rand.NextFloat(0.7f, 1.1f);
                Dust b = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(25, 25), trailDustType, Vector2.Zero, 120);
                b.velocity = -Projectile.velocity * Main.rand.NextFloat(0.4f, 0.8f);
                b.scale = scale;
                b.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Daybreak, 180);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ApplyScalingForcedCrit(Projectile);
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            // Spawn a Helium Flash on impact
            int type = ModContent.ProjectileType<HeliumFlashBlast>();
            int damage = (int)(HeliumFlash.ExplosionDamageMultiplier * Projectile.damage);
            float kb = 9.5f;
            if (explode)
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/HeliumFlashCoreImpact");
                SoundEngine.PlaySound(fire with { Volume = 1f, Pitch = 0f }, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, type, damage, kb, Projectile.owner, 0f, 0f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D orbTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/VolatileStarcore").Value;
            Rectangle frame = orbTexture.Frame(1, NumAnimationFrames, 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            Texture2D rechargeTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            // Glow Orb
            float randSize = Main.rand.NextFloat(0.8f, 1.2f);
            Main.EntitySpriteDraw(rechargeTexture, Projectile.Center - Main.screenPosition, null, Color.OrangeRed with { A = 0 }, Projectile.rotation, rechargeTexture.Size() * 0.5f, 0.6f * randSize, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(rechargeTexture, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 0.75f, Projectile.rotation, rechargeTexture.Size() * 0.5f, 0.35f * randSize, SpriteEffects.None, 0);



            // Starcore
            Main.EntitySpriteDraw(orbTexture, Projectile.Center - Main.screenPosition, frame, Color.White, 0, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 40, targetHitbox);
    }
}
