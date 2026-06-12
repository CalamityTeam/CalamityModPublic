using CalamityMod.Graphics.Primitives;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class UrchinSpikeFugu : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";

        public ref float Time => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 90;
            Projectile.noEnchantments = true;
        }

        public override void AI()
        {
            Time++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.alpha = (int)Utils.Remap(Time, 0f, 12f, 255f, 0f);

            NPC potentialTarget = null;
            float range = 256f;
            foreach (NPC target in Main.ActiveNPCs)
            {
                if (target.CanBeChasedBy(Projectile) && Projectile.localNPCImmunity[target.whoAmI] == 0)
                {
                    float distance = Vector2.Distance(target.Center, Projectile.Center);
                    if (distance < range)
                    {
                        range = distance;
                        potentialTarget = target;
                    }
                }
            }

            if (potentialTarget != null && Time >= 12f)
            {
                Vector2 idealVelocity = Projectile.SafeDirectionTo(potentialTarget.Center) * 12f;
                Projectile.velocity = (Projectile.velocity * 20f + idealVelocity) / 21f;
                Projectile.velocity = Projectile.velocity.MoveTowards(idealVelocity, 2f);
            }
            else if (Time >= 48f)
                Projectile.velocity *= 0.9f;

            int dustRate = (int)MathF.Max(Utils.Remap(Time, 0f, 12f, 20f, 4f), Utils.Remap(Time, 60f, 90f, 4f, 20f));
            if (Main.rand.NextBool(dustRate))
            {
                Dust offTrail = Dust.NewDustPerfect(Projectile.Center, DustID.Venom, Main.rand.NextVector2Circular(0.2f, 0.2f));
                offTrail.noGravity = true;
                offTrail.scale = Main.rand.NextFloat(0.6f, 1.2f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Poisoned, 120);

        public override bool? CanDamage() => Time < 12f ? false : base.CanDamage();

        internal float WidthFunction(float completionRatio, Vector2 vertexPos) => (1f - completionRatio) * Projectile.scale * 4f;
        internal Color ColorFunction(float completionRatio, Vector2 vertexPos) => new Color(91, 62, 153) * Projectile.Opacity;
        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, (_,_) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 8);
            return true;
        }
    }
}
