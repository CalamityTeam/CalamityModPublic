using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ElementalFlare : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/ExtraTextures/SmallGreyscaleCircle";

        // Stats for both
        public static int Lifetime = 90; // effective, 1.5 seconds

        // Homing Flare stats
        public static float MinimumSpeed => 4f;
        public static float MaximumSpeed => 24f;
        public static float HomingRange => 720f; // 45 tiles
        public static float HomingSpeedMult => 1.6f;

        // Sticky Flare stats
        public static int StickDuration => 2; // already in seconds
        public static int MaxStick => 6;
        public static float StickyDamageFalloff => 0.5f;

        public bool StickToEnemies => Projectile.ai[2] == 1f;
        public bool StickToTiles => Projectile.ai[2] == 2f;
        public ref float StoredVelocity => ref Projectile.ai[0];
        public ref float Direction => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 2;
            Projectile.timeLeft = Lifetime * Projectile.MaxUpdates;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (StickToTiles)
                return;

            if (StickToEnemies)
            {
                Projectile.ai[2] = 1f;
                Projectile.StickyProjAI(StickDuration * Projectile.MaxUpdates);
                return;
            }

            float effectiveVelocity = MathHelper.Clamp(StoredVelocity, MinimumSpeed, MaximumSpeed);
            int targetNPC = Projectile.FindTargetWithLineOfSight(HomingRange);
            if (targetNPC != -1 && Projectile.timeLeft < (Lifetime - 20) * Projectile.MaxUpdates)
            {
                Vector2 destination = Main.npc[targetNPC].Center;
                Projectile.velocity = Projectile.SafeDirectionTo(destination) * effectiveVelocity * HomingSpeedMult;

                // If close enough, don't do the sine movement
                if (Projectile.Distance(destination) <= 160f)
                    return;
            }

            // Sine-like movement
            Projectile.position += (Vector2.UnitY * MathF.Sin(Projectile.timeLeft * MathHelper.Pi * 0.05f) * effectiveVelocity * Direction).RotatedBy(Projectile.velocity.ToRotation());
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 540);

            // Lose a bit of damage from repeated hits
            if ((StickToEnemies || StickToTiles) && Projectile.damage > 1)
                Projectile.damage = (int)(Projectile.damage * StickyDamageFalloff);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (StickToEnemies)
                Projectile.ModifyHitNPCSticky(MaxStick);            
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (StickToTiles || StickToEnemies)
            {
                Projectile.velocity = oldVelocity * 0.95f;
    			Projectile.position -= Projectile.velocity;

                // No more enemy sticking + reset duration
                if (StickToEnemies)
                {
                    Projectile.ai[2] = 2f;
                    Projectile.timeLeft = StickDuration * 60 * Projectile.MaxUpdates;
                }
                return false;
            }
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            // Circular spread of clouds
            for (int i = 0; i < 8; i++)
            {
                Vector2 smokeVel = Main.rand.NextVector2Circular(16f, 16f);
                Color smokeColor = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.8f);
                Particle smoke = new MediumMistParticle(Projectile.Center, smokeVel, smokeColor, Color.Black, Main.rand.NextFloat(0.6f, 1.6f), 200 - Main.rand.Next(60), 0.1f);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lightTexture = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float hue = 0.5f + 0.5f * i / (float)Projectile.oldPos.Length * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f);
                Color color = Main.hslToRgb(hue, 1f, 0.6f);
                Vector2 drawPosition = Projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(-15f, -15f);
                Color outerColor = color * 2f;
                Color innerColor = Color.Lerp(color, Color.White, 0.2f) * 0.5f;
                float intensity = 0.9f + 0.15f * MathF.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);

                // Become smaller the futher along the old positions we are.
                intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);

                Vector2 outerScale = new Vector2(1.25f) * intensity;
                Vector2 innerScale = new Vector2(1.25f) * intensity * 0.7f;
                outerColor *= intensity * Projectile.scale;
                innerColor *= intensity * Projectile.scale;
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, 0f, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, 0f, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
