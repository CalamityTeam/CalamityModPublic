using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class NebulaElementalBeam : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override float MaxScale => 1.4f;
        public override float MaxLaserLength => 1000f;
        public override float Lifetime => 30f;
        public override Color LightCastColor => Color.White;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayStart", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayMid", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/UltimaRayEnd", AssetRequestMode.ImmediateLoad).Value;

        public const float UniversalAngularSpeed = MathHelper.Pi / 400f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 13;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)Lifetime;
        }

        public override void ExtraBehavior()
        {
            RotationalSpeed = UniversalAngularSpeed;
            // Generate a burst of bubble-like nebula dust.
            if (!Main.dedServ && Time == 5f)
            {
                int totalBubbles = 24;
                for (int i = 0; i < totalBubbles; i++)
                {
                    Dust nebulaBubble = Dust.NewDustPerfect(Projectile.Center, 242);
                    nebulaBubble.velocity = Main.rand.NextVector2Circular(6f, 6f);
                    nebulaBubble.scale = Main.rand.NextFloat(2f, 3f);
                    nebulaBubble.noGravity = true;
                }
            }
        }

        public override void DetermineScale() => Projectile.scale = Projectile.timeLeft / Lifetime * MaxScale;

        public override bool PreDraw(ref Color lightColor)
        {
            DrawBeamWithColor(new Color(190, 29, 209), Projectile.scale);
            DrawBeamWithColor(Color.Lerp(new Color(254, 126, 229), Color.Transparent, 0.35f), Projectile.scale * 0.6f);
            DrawBeamWithColor(Color.Lerp(new Color(254, 190, 243), Color.Transparent, 0.35f), Projectile.scale * 0.6f);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElementalMix>(), 30);
        }
    }
}
