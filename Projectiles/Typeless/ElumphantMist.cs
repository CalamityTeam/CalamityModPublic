using System;
using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Particles;
using CalamityMod.Systems.Graphic.PixelationSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Renderers;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class ElumphantMist : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public ref float time => ref Projectile.ai[0];
        public int textureChoice = 0;
        public float fade = 1;
        public int lifetime = 90;
        public float originalScale = 0;
        public Color mistColor = Color.White;
        public bool vis => Owner.Calamity().frozenCubeVisuals;
        public static Asset<Texture2D> SmokeTexture1 { get; private set; }
        public static Asset<Texture2D> SmokeTexture2 { get; private set; }
        public static Asset<Texture2D> SmokeTexture3 { get; private set; }
        public override void Load()
        {
            if (Main.dedServ)
                return;

            SmokeTexture1 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/SmokePuff1");
            SmokeTexture2 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/SmokePuff2");
            SmokeTexture3 = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/SmokePuff3");
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 38;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = lifetime;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Generic;
        }
        public override void AI()
        {
            if (time == 0)
            {
                textureChoice = (int)Projectile.ai[1];
                originalScale = Main.rand.NextFloat(0.7f, 1.1f);
                mistColor = Main.rand.NextBool() ? Elumphant.color1 : Elumphant.color2;
            }
            Projectile.velocity *= 0.96f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            fade = 1 - MathF.Pow(Utils.GetLerpValue(lifetime, 0, Projectile.timeLeft, true), 3 / Projectile.ai[2]);
            Projectile.scale = (1 + (1 - fade) * Projectile.ai[2]) * originalScale;

            float inverseFade = Utils.GetLerpValue(lifetime, 0, Projectile.timeLeft, true);
            float opacity = 0.4f * fade * (1.2f - inverseFade);
            if (Main.rand.NextBool(3) && vis) // Effects
            {
                Particle trail = new CustomColorChangeSpark(Projectile.Center + Main.rand.NextVector2Circular(40, 40) * Projectile.scale * (1 - fade) - Projectile.velocity.SafeNormalize(Vector2.UnitX) * (-35 + 65 * inverseFade) * Projectile.scale, Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.3f) * Main.rand.NextFloat(3, 4), Main.rand.NextBool(3) ? "CalamityMod/Particles/WaterFoam" : "CalamityMod/Particles/BloomCircle", false, Main.rand.Next(9, 12), Main.rand.NextFloat(0.75f, 1f) * Projectile.scale, (mistColor == Elumphant.color1 ? mistColor : Elumphant.color2) * opacity, (mistColor == Elumphant.color2 ? mistColor : Elumphant.color1) * opacity, new Vector2(0.15f + 0.85f * inverseFade, 1.4f - 0.4f * inverseFade));
                GeneralParticleHandler.SpawnParticle(trail, true);
            }
            if (Main.rand.NextBool(12) && vis)
            {
                float rotation = 0;
                Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.1f) * Main.rand.NextFloat(7f, 14f);
                Particle mist = new CustomPulsingSpark(Projectile.Center, velocity, "CalamityMod/Particles/ThinSparkle", "CalamityMod/Particles/BloomCircle", false, 35, Main.rand.NextFloat(0.95f, 1.35f) * MathF.Pow(Projectile.scale, 0.3f), (mistColor == Elumphant.color1 ? mistColor : Elumphant.color2) * opacity, (mistColor == Elumphant.color2 ? mistColor : Elumphant.color1) * opacity,
                    new Vector2(0.6f, 1.2f), true, true, Main.rand.Next(4, 7 + 1), colorFadeSpeed: 0.85f, noShrink: true, extraRotation: rotation, shrinkSpeed: 0.1f, turnRate: (Main.rand.NextBool() ? -1 : 1) * Main.rand.NextFloat(0.06f, 0.085f),
                    sineRate: Main.rand.NextFloat(0.15f, 0.35f), sineIntensity: (int)(8 + Main.rand.Next(35, 55 + 1) * inverseFade) * Projectile.scale, sineRotation: MathHelper.PiOver2 + velocity.ToRotation());
                GeneralParticleHandler.SpawnParticle(mist, true, Main.rand.NextBool() ? Enums.GeneralDrawLayer.AfterNPCs : Enums.GeneralDrawLayer.BeforeNPCs);
            }

            Lighting.AddLight(Projectile.Center, mistColor.ToVector3() * 0.3f * Projectile.scale);

            time++;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            target.AddBuff(FrozenCube.debuff, (int)(300 * Projectile.ai[2]));
            float minMult = 0.25f;
            int hitsToMinMult = 3;
            float damageMult = Utils.Remap(Projectile.numHits, 0, hitsToMinMult, 1, minMult, true);
            modifiers.SourceDamage *= damageMult;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CalamityUtils.CircularHitboxCollision(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 15 * Projectile.scale, 50 * Projectile.scale, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (time == 0)
                return false;
            Texture2D bTexture = (textureChoice == 0 ? SmokeTexture1.Value : textureChoice == 1 ? SmokeTexture2.Value : SmokeTexture3.Value);
            float inverseFade = Utils.GetLerpValue(lifetime, 0, Projectile.timeLeft, true);

            PixelationManager.AddPixelatedDrawer((_) =>
            {
                Main.EntitySpriteDraw(bTexture, Projectile.Center - Main.screenPosition, null, mistColor with { A = 0 } * fade * (vis ? 1 : 0.5f), Projectile.rotation, new Vector2(bTexture.Width / 2, bTexture.Height * (MathHelper.Lerp(0.7f, 0.4f, inverseFade))), new Vector2(0.3f + 0.7f * inverseFade, 1.4f - 0.4f * inverseFade) * Projectile.scale, SpriteEffects.None);
            }, Enums.GeneralDrawLayer.AfterProjectiles, default);
            return false;
        }
        public override bool? CanDamage() => null;
    }
}
