using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Shortswords
{
    public class CosmicShivProj : BaseShortswordProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<CosmicShiv>();
        public override string Texture => "CalamityMod/Items/Weapons/Melee/CosmicShiv";
        public bool MeleeEffect = false;
        public int NumHits = 0;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(24);
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.DamageType = TrueMeleeDamageClass.Instance;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = 360;
            Projectile.extraUpdates = 1;
            Projectile.hide = true;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override Action<Projectile> EffectBeforePullback => (proj) =>
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 3f, ModContent.ProjectileType<CosmicShivTrail>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
        };

        public override void SetVisualOffsets()
        {
            const int HalfSpriteWidth = 48 / 2;
            const int HalfSpriteHeight = 48 / 2;

            int HalfProjWidth = Projectile.width / 2;
            int HalfProjHeight = Projectile.height / 2;

            DrawOriginOffsetX = 0;
            DrawOffsetX = -(HalfSpriteWidth - HalfProjWidth);
            DrawOriginOffsetY = -(HalfSpriteHeight - HalfProjHeight);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEffect();

            if (NumHits < 3)
            {
                // Just to avoid insanity from stacking so many super dummies or hitting many worm segments at once
                // 2x bonus for true melee hits (spawns an additional aura plus the one aura from the cosmic shiv ball)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Zero, ModContent.ProjectileType<CosmicShivAura>(), Projectile.damage, Projectile.knockBack, Projectile.owner, target.whoAmI);
                NumHits++;
            }

            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 240);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            OnHitEffect();

            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 240);
        }

        public void OnHitEffect()
        {
            float rand2PI = Main.rand.NextFloat(MathHelper.TwoPi);  // Random rotation offset
            int petalCount = Main.rand.Next(5, 8);                  // Number of star points
            float speed = Main.rand.Next(18, 24);                   // Size of star

            // Bool check so it only performs this effect once per strike, at a 50% chance
            if (!MeleeEffect && Main.rand.NextBool())
            {
                for (float k = 0f; k < MathHelper.TwoPi; k += 0.08f)
                {
                    float scale = Main.rand.NextFloat(1.5f, 1.9f);
                    float randomWhitingValue = Main.rand.NextFloat(0.0f, 0.2f);
                    Color color = Color.Lerp(CosmicShivTrail.DustColors[Main.rand.Next(0, CosmicShivTrail.DustColors.Count)], Color.White, randomWhitingValue);  // Just for even more variety in colors idk
                    Vector2 velocity = StarPolarEquation(petalCount, k, rand2PI) * speed * 2f;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, velocity, 0, color, scale);
                    dust.noGravity = true;
                    dust.fadeIn = -1f;      // I don't know if this does anything but it looks like the dust fades out faster with this

                    Vector2 velocity2 = StarPolarEquation(petalCount, k - 0.04f, rand2PI) * speed * 2 * 0.9f;   // Inner star
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, velocity2, 0, color, scale);
                    dust2.noGravity = true;
                    dust2.fadeIn = -1f;
                }
            }
        }

        public Vector2 StarPolarEquation(int pointCount, float angle, float offset)     // For OnHitNPC(), Thanks Dominic
        {
            // There should be a star point that looks directly upward. However, that isn't the case for odd star counts with the equation below.
            // To address this, a -90 degree rotation is performed.
            // Refer to desmos to view the resulting shape this creates. It's basically a black box of trig otherwise.
            float sqrt3 = 1.732051f;
            float numerator = MathF.Cos(MathHelper.Pi * (pointCount + 1f) / pointCount);
            float starAdjustedAngle = MathF.Asin(MathF.Cos(pointCount * angle + offset)) * 2f;
            float denominator = MathF.Cos((starAdjustedAngle + MathHelper.PiOver2 * pointCount) / (pointCount * 2f));
            Vector2 result = angle.ToRotationVector2() * numerator / denominator / sqrt3;
            return result;
        }
    }
}
