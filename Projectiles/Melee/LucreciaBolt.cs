using System;
using System.Linq;
using CalamityMod.Dusts;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Melee
{
    public class LucreciaBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static float MaxWidth = 30;
        public ref float Time => ref Projectile.ai[0];

        public static Asset<Texture2D> TrailTex;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 13;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 19;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 0;
            Projectile.timeLeft = 420;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 12;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float hitboxSize = Projectile.width * Projectile.ai[2];
            return CalamityUtils.CircularHitboxCollision(Projectile.Center, hitboxSize, targetHitbox);
        }
        public override void AI()
        {
            // Little bit of ambient lighting as it travels
            Lighting.AddLight(Projectile.Center, Color.WhiteSmoke.ToVector3() * 0.4f);

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 1.01f;
            Projectile.scale = Utils.GetLerpValue(0f, 0.1f, Projectile.timeLeft / 600f, true) * Projectile.ai[2];

            if (Projectile.FinalExtraUpdate())
                Time++;

            // CRUCIAL so that oldPos isnt initialized incorrectly when drawing
            if (Projectile.localAI[0] == 0f)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Projectile.oldPos[i] = Projectile.position;
                }
                Projectile.localAI[0] = 1f;
            }

            if (Projectile.ai[1] > 0)
            {
                Projectile.damage = 0;
                // Reduce opacity over time
                Projectile.Opacity -= 0.04f;
                if (Projectile.Opacity <= 0f)
                {
                    Projectile.Kill();
                }
            }
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            for (int i = 0; i < 6; i++)
            {
                float variance = Main.rand.NextFloat(-0.6f, 0.6f);
                int dustStyle = ModContent.DustType<SquashDust>();
                Dust dust2 = Dust.NewDustPerfect(target.Center, dustStyle);
                dust2.scale = Main.rand.NextFloat(1.2f, 1.6f) - Math.Abs(variance);
                dust2.velocity = (Projectile.velocity * 1.5f).RotatedBy(variance) * Main.rand.NextFloat(1.2f, 1.5f) * (1 - Math.Abs(variance));
                dust2.noGravity = true;
                dust2.color = Color.Lerp(Color.MediumPurple, Color.CornflowerBlue, Main.rand.NextFloat(0f, 1f));
            }
            
            // Flag set for fading out
            Projectile.ai[1] = 1f;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Lerp(Color.MediumPurple, Color.CornflowerBlue, 0.5f) with { A = 0 } * Projectile.Opacity;

        public float TrailWidth(float completionRatio, Vector2 vertexPos)
        {
            float sine = MathF.Sin(Main.GlobalTimeWrappedHourly * 9 - completionRatio * 6);
            float newSine = 0.7f - sine * 0.3f;
            float width = newSine * (completionRatio < 0.4f ? 1 - MathF.Pow(Utils.GetLerpValue(0.4f, 0f, completionRatio, true), 3) : Utils.GetLerpValue(1f, 0.4f, completionRatio, true)) * (float)Math.Sin(Math.Acos(1 - Utils.GetLerpValue(0f, 0.08f, completionRatio, true))) * MathF.Pow(Projectile.ai[2], 0.4f);

            width *= Utils.GetLerpValue(0f, 0.1f, Projectile.timeLeft / 600f, true);

            return width * (MaxWidth * .265f);
        }
        public Color TrailColor(float completionRatio, Vector2 vertexPos)
        {
            Color baseColor = Color.Lerp(Color.MediumPurple, Color.CornflowerBlue, completionRatio);

            return baseColor * 0.2f * Projectile.Opacity;
        }

        public Color MiniTrailColor(float completionRatio, Vector2 vertexPos)
        {
            Color baseColor = Color.Lerp(Color.MediumPurple, Color.CornflowerBlue, completionRatio);

            return baseColor * Projectile.Opacity;
        }

        public float MiniTrailWidth(float completionRatio, Vector2 vertexPos) => TrailWidth(completionRatio, vertexPos) * 5.5f * Projectile.ai[2];

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 412)
                return false;

            Color mainColor = Color.Lerp(Color.MediumPurple, Color.CornflowerBlue, ((float)Main.timeForVisualEffects * 0.5f + Projectile.whoAmI * 0.12f) % 1);
            Color secondaryColor = Color.Lerp(Color.MediumPurple, Color.CornflowerBlue, ((float)Main.timeForVisualEffects * 0.5f + Projectile.whoAmI * 0.12f + 0.2f) % 1);

            Main.spriteBatch.EnterShaderRegion();

            if (TrailTex == null)
                TrailTex = Request<Texture2D>("CalamityMod/ExtraTextures/Trails/BasicTrail");

            GameShaders.Misc["CalamityMod:ExobladePierce"].SetShaderTexture(TrailTex);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseImage2("Images/Extra_189");
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(mainColor);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(secondaryColor);
            GameShaders.Misc["CalamityMod:ExobladePierce"].Apply();

            Vector2 offset = Projectile.Size * 0.5f;

            // Apply the offset to the projectile's old position
            Vector2[] oldPosWithOffset = Projectile.oldPos.Select(p => p - offset).ToArray();

            PrimitiveRenderer.RenderTrail(oldPosWithOffset, new(TrailWidth, TrailColor, (_,_) => Projectile.Size * 1f, shader: GameShaders.Misc["CalamityMod:ExobladePierce"]), 30);

            GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(mainColor);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(secondaryColor);

            PrimitiveRenderer.RenderTrail(oldPosWithOffset, new(MiniTrailWidth, MiniTrailColor, (_,_) => Projectile.Size * 1f, shader: GameShaders.Misc["CalamityMod:ExobladePierce"]), 30);

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}
