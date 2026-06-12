using System;
using CalamityMod.Particles;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class ProtolithBangleProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public int time = 0;
        public int damageTime = 24;
        public int Soundtime1 = 2;
        public int Soundtime2 = 8;
        public int Soundtime3 = 15;
        public int explosionSize = 230;
        public SlotId SoundSlot;
        public Player Owner => Main.player[Projectile.owner];
        public bool visual => Owner.Calamity().protolithBangleVisual;
        public override void SetDefaults()
        {
            Projectile.width = explosionSize;
            Projectile.height = explosionSize;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 0;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = damageTime + 20;
            Projectile.ArmorPenetration = 25;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 0.8f;
        }
        public override void AI()
        {
            if (!(Projectile.ai[0] < 0f || Projectile.ai[0] > 199f) && time <= damageTime && Main.npc[(int)Projectile.ai[0]].active && Main.npc[(int)Projectile.ai[0]].life > 0)
                Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center;

            Vector2 particlePlace = Vector2.UnitY * 40;
            if (time == Soundtime1)
            {
                MakePusle(Projectile.Center + particlePlace);
            }
            if (time == Soundtime2)
            {
                MakePusle(Projectile.Center + particlePlace.RotatedBy(MathHelper.TwoPi / 3));
            }
            if (time == Soundtime3)
            {
                MakePusle(Projectile.Center + particlePlace.RotatedBy(-MathHelper.TwoPi / 3));
            }
            if (time == damageTime)
            {
                float visMult = (visual ? 1 : 0.3f);
                Projectile.ai[0] = -1;

                if (visual)
                {
                    Particle spark = new CustomSpark(Projectile.Center, Vector2.Zero, "CalamityMod/Particles/BloomCircle", false, 15, 1.2f, Color.Gold, Projectile.scale * new Vector2(1f, 1.3f), true, true, shrinkSpeed: 0.9f);
                    GeneralParticleHandler.SpawnParticle(spark);

                    Particle pulse = new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomRing", Projectile.scale * new Vector2(1.4f, 0.6f), 0, 0.3f, 1.35f, 15);
                    GeneralParticleHandler.SpawnParticle(pulse);
                }
                
                for (int i = 0; i < 28; i++)
                {
                    Particle marble = new CustomSpark(Projectile.Center, (Vector2.One * Main.rand.NextFloat(8, 13)).RotatedByRandom(MathHelper.TwoPi), "CalamityMod/Particles/Square", true, Main.rand.Next(40, 70 + 1), Projectile.scale * Main.rand.NextFloat(0.08f, 0.14f) * 15, Color.Lerp(Color.White, Color.Khaki, Main.rand.NextFloat()) * visMult, new Vector2(1f, Main.rand.NextFloat(1, 2)), false, false, extraRotation: Main.rand.NextFloat(-4, 4));
                    GeneralParticleHandler.SpawnParticle(marble);
                }
            }
            if (time == 0 && visual)
            {
                SoundStyle sound = new("CalamityMod/Sounds/Item/ProtolithBangleSound");
                SoundSlot = SoundEngine.PlaySound(sound with { Volume = 1f, MaxInstances = -1 }, Projectile.Center);
            }
            if (SoundEngine.TryGetActiveSound(SoundSlot, out var Sound) && Sound.IsPlaying)
                Sound.Position = Projectile.Center;
            time++;
        }
        public void MakePusle(Vector2 position)
        {
            if (!visual)
                return;

            Particle pulse = new CustomPulse(position, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomRing", Projectile.scale * Vector2.One, 0, 0.3f, 0.65f, 25);
            GeneralParticleHandler.SpawnParticle(pulse);

            for (int i = 0; i < 18; i++)
            {
                Vector2 outerVel = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(5, 7);
                Particle marble = new CustomSpark(position + outerVel * 5, -outerVel * 0.6f, "CalamityMod/Particles/Square", false, Main.rand.Next(10, 15 + 1), Projectile.scale * Main.rand.NextFloat(0.08f, 0.14f) * 10, Color.Lerp(Color.White, Color.Khaki, Main.rand.NextFloat()), new Vector2(1f, Main.rand.NextFloat(1, 2)), false, false, extraRotation: Main.rand.NextFloat(-4, 4));
                GeneralParticleHandler.SpawnParticle(marble);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ApplyScalingForcedCrit(Projectile);

            float minMult = 0.1f;
            int hitsToMinMult = 5;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;

            Vector2 launchVel = Utils.DirectionTo(Projectile.Center, target.Center) - Vector2.UnitY;
            float launchPower = 6;
            target.MoveNPC(launchVel, launchPower, true, Owner);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * 0.5f * Projectile.scale, targetHitbox);
        public override bool? CanDamage()
        {
            if (time >= damageTime)
                return null;
            else
                return false;
        }
    }
}
