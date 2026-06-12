using System;
using CalamityMod.Particles;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class BatholithBangleProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public int time = 0;
        public int damageTime = 120;
        public int Soundtime1 = 84;
        public int Soundtime2 = 90;
        public int Soundtime3 = 96;
        public int Soundtime4 = 107;
        public SlotId SoundSlot;
        public Color clr1 = new Color(59, 28, 136); // Light Granite
        public Color clr2 = new Color(16, 14, 36); // Dark Granite
        public Color clr3 = new Color(23, 186, 218); // Granite energy
        public int spinDir = 1;
        public bool invalidTarget => (Projectile.ai[0] < 0f || Projectile.ai[0] > 199f || !Main.npc[(int)Projectile.ai[0]].active || Main.npc[(int)Projectile.ai[0]].life <= 0);
        public Player Owner => Main.player[Projectile.owner];
        public bool visual => Owner.Calamity().batholithBangleVisual;
        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 0;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = damageTime + 2;
            Projectile.ArmorPenetration = 35;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 0.8f;
        }
        public override void AI()
        {
            if (time == 0 && visual)
            {
                spinDir = Main.rand.NextBool() ? 1 : -1;
                SoundStyle sound = new("CalamityMod/Sounds/Item/BatholithBangleSound");
                SoundSlot = SoundEngine.PlaySound(sound with { Volume = 1f, MaxInstances = -1 }, Projectile.Center);
            }
            if (!invalidTarget)
                Projectile.Center = Main.npc[(int)Projectile.ai[0]].Center;
            else if (time < Soundtime2)
            {
                NPC target = Projectile.Center.ClosestNPCAt(400, true, true);
                Projectile.ai[0] = (target == null ? -1 : target.whoAmI);
            }

            if (time > Soundtime1)
            {
                Projectile.rotation += 0.23f * spinDir * (float)Math.Pow(Utils.GetLerpValue(damageTime, Soundtime2, time, true), 1);
            }

            float stabLerp = (float)Math.Pow(Utils.GetLerpValue(Soundtime4, damageTime, time, true), 8);
            Vector2 triPlace = (Vector2.UnitY * (230 - 150 * stabLerp)) * Projectile.scale;
            if (time == Soundtime2)
                MakePusle(Projectile.Center + triPlace.RotatedBy(Projectile.rotation));
            if (time == Soundtime3)
                MakePusle(Projectile.Center + triPlace.RotatedBy(Projectile.rotation + MathHelper.TwoPi / 3));
            if (time == Soundtime4)
                MakePusle(Projectile.Center + triPlace.RotatedBy(Projectile.rotation - MathHelper.TwoPi / 3));
            if (time == damageTime)
            {
                float visMult = (visual ? 1 : 0.6f);
                if (visual)
                {
                    Owner.SetScreenshake(5f);

                    Particle spark = new CustomSpark(Projectile.Center, Vector2.Zero, "CalamityMod/Particles/BloomCircle", false, 12, 1.35f, clr3, Projectile.scale * new Vector2(2.5f, 1.3f), true, true, shrinkSpeed: 1.25f, extraRotation: MathHelper.PiOver2);
                    GeneralParticleHandler.SpawnParticle(spark);
                    Particle spark2 = new CustomSpark(Projectile.Center, Vector2.Zero, "CalamityMod/Particles/BloomCircle", false, 10, 1.05f, clr3, Projectile.scale * new Vector2(2f, 1.3f), true, true, shrinkSpeed: 0.7f);
                    GeneralParticleHandler.SpawnParticle(spark2);

                    Particle pulse = new CustomPulse(Projectile.Center, Vector2.Zero, clr1, "CalamityMod/Particles/BloomRing", Projectile.scale * new Vector2(0.6f, 1.4f), 0, 0.3f, 1.35f, 15);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    Particle pulse2 = new CustomPulse(Projectile.Center, Vector2.Zero, clr1, "CalamityMod/Particles/BloomRing", Projectile.scale * new Vector2(1.3f, 0.7f), 0, 0.25f, 1f, 15);
                    GeneralParticleHandler.SpawnParticle(pulse2);

                }

                for (int i = 0; i < 12; i++)
                {
                    Vector2 vel = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(5, 10);
                    Particle granite = new CustomSpark(Projectile.Center, vel, "CalamityMod/Particles/GlowTriangle", true, Main.rand.Next(30, 55 + 1), Projectile.scale * Main.rand.NextFloat(0.12f, 0.18f), (Main.rand.NextBool() ? clr1 : clr3) * visMult, Vector2.One, true, false, extraRotation: Main.rand.NextFloat(-4, 4), spin: Main.rand.NextFloat(-0.5f, 0.5f));
                    GeneralParticleHandler.SpawnParticle(granite);
                }
            }

            if (SoundEngine.TryGetActiveSound(SoundSlot, out var Sound) && Sound.IsPlaying)
                Sound.Position = Projectile.Center;
            time++;
        }
        public void MakePusle(Vector2 position)
        {
            if (!visual)
                return;
            for (int i = 0; i < 8; i++)
            {
                Vector2 outerVel = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1, 5);
                Particle marble = new CustomSpark(position + outerVel * 5, outerVel, "CalamityMod/Particles/BloomCircle", false, Main.rand.Next(18, 25 + 1), Projectile.scale * Main.rand.NextFloat(0.3f, 0.4f), Main.rand.NextBool() ? clr1 : clr3, Vector2.One, true, true, extraRotation: Main.rand.NextFloat(-4, 4), spin: Main.rand.NextFloat(-0.3f, 0.3f), glowOpacity: 0.8f);
                GeneralParticleHandler.SpawnParticle(marble);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ApplyScalingForcedCrit(Projectile);

            Vector2 launchVel = Utils.DirectionTo(Owner.Center, target.Center) - Vector2.UnitY;
            float launchPower = 9;
            target.MoveNPC(launchVel, launchPower, true, Owner);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, Projectile.width * 0.5f, targetHitbox);
        public override bool? CanDamage()
        {
            if (time >= damageTime && Projectile.numHits <= 0)
                return null;
            else
                return false;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if ((invalidTarget || Projectile.ai[0] == target.whoAmI) && Projectile.numHits <= 0)
                return null;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!visual)
                return false;
            Asset<Texture2D> tri = ModContent.Request<Texture2D>("CalamityMod/Particles/GlowTriangle");
            Asset<Texture2D> shine = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");

            float stabLerp = (float)Math.Pow(Utils.GetLerpValue(Soundtime4, damageTime, time, true), 8);
            Vector2 triPlace = Vector2.UnitY * (230 - 150 * stabLerp) * Projectile.scale;

            for (int i = 0; i < 6; i++)
            {
                float scale = Projectile.scale * (0.3f - 0.015f * i);
                float rotation = Projectile.rotation;
                float spawnScaleBonus = 0.3f;
                float squashPower = 0.4f;
                Color triColor = Color.Lerp(clr1, clr3, i * 0.14f);
                if (time >= Soundtime2)
                {
                    float lerp = (float)Math.Pow(Utils.GetLerpValue(Soundtime2 + 13, Soundtime2, time, true), 2);
                    Main.EntitySpriteDraw(tri.Value, Projectile.Center + triPlace.RotatedBy(rotation) - Main.screenPosition, null, triColor with { A = 0 }, rotation + MathHelper.Pi * lerp, tri.Size() * 0.5f, Projectile.scale * new Vector2(0.05f * i + 1 - squashPower * (1 - lerp), -0.05f * i + 1 + squashPower * (1 - lerp)) * (scale + spawnScaleBonus * lerp), SpriteEffects.None);
                }
                if (time >= Soundtime3)
                {
                    rotation += MathHelper.TwoPi / 3;
                    float lerp = (float)Math.Pow(Utils.GetLerpValue(Soundtime3 + 13, Soundtime3, time, true), 2);
                    Main.EntitySpriteDraw(tri.Value, Projectile.Center + triPlace.RotatedBy(rotation) - Main.screenPosition, null, triColor with { A = 0 }, rotation + MathHelper.Pi * lerp, tri.Size() * 0.5f, Projectile.scale * new Vector2(0.05f * i + 1 - squashPower * (1 - lerp), -0.05f * i + 1 + squashPower * (1 - lerp)) * (scale + spawnScaleBonus * lerp), SpriteEffects.None);
                }
                if (time >= Soundtime4)
                {
                    rotation += MathHelper.TwoPi / 3;
                    float lerp = (float)Math.Pow(Utils.GetLerpValue(Soundtime4 + 13, Soundtime4, time, true), 2);
                    Main.EntitySpriteDraw(tri.Value, Projectile.Center + triPlace.RotatedBy(rotation) - Main.screenPosition, null, triColor with { A = 0 }, rotation + MathHelper.Pi * lerp, tri.Size() * 0.5f, Projectile.scale * new Vector2(0.05f * i + 1 - squashPower * (1 - lerp), -0.05f * i + 1 + squashPower * (1 - lerp)) * (scale + spawnScaleBonus * lerp), SpriteEffects.None);
                }
                
            }
            for (int i = 0; i < 4; i++)
            {
                float shineLerp = (float)Math.Pow(Utils.GetLerpValue(0, Soundtime4, time, true), 3);
                Color clrLerp = Color.Lerp(clr1, clr3, i * 0.3f);
                Color shineColor = Color.Lerp(clrLerp, Color.White, i * 0.2f) * shineLerp;
                float shineScale = Projectile.scale * 0.4f + 0.3f * shineLerp;
                Main.EntitySpriteDraw(shine.Value, Projectile.Center - Main.screenPosition, null, shineColor with { A = 0 }, MathHelper.PiOver2, shine.Size() * 0.5f, new Vector2(0.5f - i * 0.15f, 1 + i * 0.2f) * (1 - i * 0.2f) * shineScale, SpriteEffects.None);
                Main.EntitySpriteDraw(shine.Value, Projectile.Center - Main.screenPosition, null, shineColor with { A = 0 }, 0, shine.Size() * 0.5f, new Vector2(0.5f - i * 0.15f, 1 + i * 0.2f) * (1 - i * 0.1f) * shineScale, SpriteEffects.None);
            }
            return false;
        }
    }
}
