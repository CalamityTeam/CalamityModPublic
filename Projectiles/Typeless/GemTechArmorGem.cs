using CalamityMod.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Graphics.Primitives;

namespace CalamityMod.Projectiles.Typeless
{
    public class GemTechArmorGem : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public ref float Time => ref Projectile.ai[0];
        public ref float Variant => ref Projectile.ai[1];
        public const int UpwardFlyTime = 24;
        public const int RedirectTime = 12;

        public override string Texture => "CalamityMod/Projectiles/Typeless/GemTechYellowGem";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 2;
            Projectile.MaxUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 13;
            Projectile.timeLeft = Projectile.MaxUpdates * 420;
            Projectile.DamageType = RogueDamageClass.Instance;
        }

        public Color GemColor => GemTechArmorState.GetColorFromGemType((GemTechArmorGemType)Variant);

        public override void AI()
        {
            // Create a puff of energy on the first frame.
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < 7; i++)
                {
                    Dust energyPuff = Dust.NewDustPerfect(Projectile.Center, 267);
                    energyPuff.velocity = -Vector2.UnitY.RotatedByRandom(0.81f) * Main.rand.NextFloat(1.25f, 4.5f);
                    energyPuff.color = Color.Lerp(GemColor, Color.White, Main.rand.NextFloat(0.5f));
                    energyPuff.scale = 1.1f;
                    energyPuff.alpha = 185;
                    energyPuff.noGravity = true;
                }
                Projectile.localAI[0] = 1f;
            }

            // This intentionally uses boss priority when homing.
            NPC potentialTarget = Projectile.Center.ClosestNPCAt(2700f, true, true);

            // Increment the timer on the last extra update.
            if (Projectile.FinalExtraUpdate())
                Time++;

            // Fly into the air.
            if (Time < UpwardFlyTime)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, -Vector2.UnitY * 3f, 0.1f);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                return;
            }

            // Point at nearby targets, if there is one. Also rapidly stop in place.
            if (Time < UpwardFlyTime + RedirectTime)
            {
                if (potentialTarget != null)
                {
                    float angleToTarget = Projectile.AngleTo(potentialTarget.Center) + MathHelper.PiOver2;
                    Projectile.rotation = Projectile.rotation.AngleLerp(angleToTarget, 0.225f).AngleTowards(angleToTarget, 0.6f);
                    Projectile.velocity = Projectile.velocity.MoveTowards(Vector2.Zero, 1.9f) * 0.9f;
                }
                return;
            }

            // Create some visual and acoustic effects right before charging.
            if (Time == UpwardFlyTime + RedirectTime && Projectile.FinalExtraUpdate())
            {
                SoundEngine.PlaySound(SoundID.Item72, Projectile.Center);
                for (int i = 0; i < 12; i++)
                {
                    Dust energyPuff = Dust.NewDustPerfect(Projectile.Center, 267);
                    energyPuff.velocity = (MathHelper.TwoPi * i / 12f).ToRotationVector2() * 5f;
                    energyPuff.color = GemColor;
                    energyPuff.scale = 1.125f;
                    energyPuff.alpha = 175;
                    energyPuff.noGravity = true;
                }
            }

            // Rapidly move towards the nearest target.
            if (potentialTarget != null && Projectile.penetrate >= Projectile.maxPenetrate)
            {
                float distanceFromTarget = Projectile.Distance(potentialTarget.Center);
                float moveInterpolant = Utils.GetLerpValue(0f, 100f, distanceFromTarget, true) * Utils.GetLerpValue(600f, 400f, distanceFromTarget, true);
                Vector2 targetCenterOffsetVec = potentialTarget.Center - Projectile.Center;
                float movementSpeed = MathHelper.Min(37.5f, targetCenterOffsetVec.Length());
                Vector2 idealVelocity = targetCenterOffsetVec.SafeNormalize(Vector2.Zero) * movementSpeed;

                // Ensure velocity never has a magnitude less than 4.
                if (Projectile.velocity.Length() < 4f)
                    Projectile.velocity += Projectile.velocity.RotatedBy(MathHelper.PiOver4).SafeNormalize(Vector2.Zero) * 4f;

                // Die if anything goes wrong with the velocity.
                if (Projectile.velocity.HasNaNs())
                    Projectile.Kill();

                // Approach the ideal velocity.
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, idealVelocity, moveInterpolant * 0.08f);
                Projectile.velocity = Projectile.velocity.MoveTowards(idealVelocity, 2f);
            }
        }

        public override bool? CanDamage() => Time > UpwardFlyTime + RedirectTime ? null : false;

        public Color TrailColor(float completionRatio)
        {
            float trailOpacity = Utils.GetLerpValue(0f, 0.067f, completionRatio, true) * Utils.GetLerpValue(0.7f, 0.58f, completionRatio, true);
            Color startingColor = Color.Lerp(Color.White, GemColor, 0.47f);
            Color middleColor = GemColor;
            Color endColor = Color.Transparent;
            return CalamityUtils.MulticolorLerp(completionRatio, startingColor, middleColor, endColor) * trailOpacity;
        }

        public static float TrailWidth(float completionRatio) => MathHelper.SmoothStep(12f, 4.25f, completionRatio);

        public override bool PreDraw(ref Color lightColor)
        {
            // Prepare the flame trail shader with its map texture.
            GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));

            Texture2D texture;
            switch (Variant)
            {
                case (int)GemTechArmorGemType.Melee:
                default:
                    texture = ModContent.Request<Texture2D>(Texture).Value;
                    break;
                case (int)GemTechArmorGemType.Ranged:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechGreenGem").Value;
                    break;
                case (int)GemTechArmorGemType.Magic:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechPurpleGem").Value;
                    break;
                case (int)GemTechArmorGemType.Summoner:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechBlueGem").Value;
                    break;
                case (int)GemTechArmorGemType.Rogue:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechRedGem").Value;
                    break;
                case (int)GemTechArmorGemType.Base:
                    texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechPinkGem").Value;
                    break;
            }

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            if (Projectile.ai[0] > UpwardFlyTime + RedirectTime)
                PrimitiveSet.Prepare(Projectile.oldPos, new(TrailWidth, TrailColor, (_) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"]), 71);

            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.damage = 0;
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = ProjectileID.Sets.TrailCacheLength[Projectile.type];
            Projectile.netUpdate = true;
        }
    }
}
