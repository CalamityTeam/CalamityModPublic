using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Primitives;
using CalamityMod.NPCs;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    [PierceResistException]
    public class PrismaticRay : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/Melee/PrismaticRayStart";
        public Player Owner => Main.player[Projectile.owner];

        public override Texture2D LaserBeginTexture => Terraria.GameContent.TextureAssets.Projectile[Type].Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/PrismaticRayMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/PrismaticRayEnd", AssetRequestMode.ImmediateLoad).Value;
        public override float MaxScale => 5f;
        public override float MaxLaserLength => 2400f;
        public override float Lifetime => 360f;
        public override Color LaserOverlayColor => Main.DiscoColor;

        public static readonly SoundStyle HitSound = new SoundStyle("CalamityMod/Sounds/Item/ExobladeDashImpact") with { Volume = 0.8f };
        public int HitSoundCooldown = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 5000;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.DamageType = MeleeRangedHybridDamageClass.Instance;
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 9;
            Projectile.hide = true;
        }

        public override void AttachToSomething()
        {
            if (Owner.CantUseHoldout())
            {
                if (Projectile.timeLeft > 30)
                    Projectile.timeLeft = 30;
            }
            if (Owner.active && !Owner.dead)
                Projectile.Center = Owner.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 20f;
        }

        public override void UpdateLaserMotion()
        {
            // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            Vector2 aimVector = (Main.MouseWorld - Owner.RotatedRelativePoint(Owner.MountedCenter, true)).SafeNormalize(Vector2.UnitY);
            aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(Projectile.velocity), PrismaticBreakerHoldout.LaserAimLag));

            if (aimVector != Projectile.velocity)
                Projectile.netUpdate = true;
            Projectile.velocity = aimVector;
        }

        public override void DetermineScale()
        {
            if (Time < 30f)
                Projectile.scale = MathHelper.Lerp(0f, 1f, Time / 30f) * MaxScale;
            else
                Projectile.scale = Utils.GetLerpValue(0f, 30f, Projectile.timeLeft, true) * MaxScale;
        }

        public override void ExtraBehavior()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Owner.SetScreenshake(3f);

            if (HitSoundCooldown > 0)
                HitSoundCooldown--;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 300);

            if (HitSoundCooldown == 0)
            {
                SoundEngine.PlaySound(HitSound, target.Center);
                HitSoundCooldown = 9;
            }
        }


        public float LaserWidthFunction(float _, Vector2 vertexPos) => Projectile.scale * Projectile.width;
        public Color LaserColorFunction(float completionRatio, Vector2 vertexPos) => Main.DiscoColor;
        public override bool PreDraw(ref Color lightColor)
        {
            // This should never happen, but just in case...
            if (Projectile.velocity == Vector2.Zero)
                return false;

            // Draw the actual laser
            Main.spriteBatch.EnterShaderRegion();
            Vector2 laserEnd = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * LaserLength;
            Vector2[] drawPoints = new Vector2[10];
            for (int i = 0; i < drawPoints.Length; i++)
                drawPoints[i] = Vector2.Lerp(Projectile.Center, laserEnd, i / (float)(drawPoints.Length - 1f));

            GameShaders.Misc["CalamityMod:ArtemisLaser"].UseColor(Main.DiscoColor);
            GameShaders.Misc["CalamityMod:ArtemisLaser"].UseImage1(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/MeltyNoise"));
            GameShaders.Misc["CalamityMod:ArtemisLaser"].UseImage2(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/LeviathanBomb"));

            PrimitiveRenderer.RenderTrail(drawPoints, new(LaserWidthFunction, LaserColorFunction, shader: GameShaders.Misc["CalamityMod:ArtemisLaser"]), 60);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
    }
}
