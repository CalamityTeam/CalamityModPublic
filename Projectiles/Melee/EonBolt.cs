using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Melee
{
    public class EonBolt : ModProjectile
    {
        internal PrimitiveTrail TrailDrawer;

        public override string Texture => "CalamityMod/Projectiles/Melee/GalaxiaBolt";

        public NPC target;
        public Player Owner => Main.player[projectile.owner];

        public ref float Hue => ref projectile.ai[0];
        public ref float HomingStrenght => ref projectile.ai[1];

        Particle Head;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eon Bolt");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 80;
            projectile.melee = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Head == null)
            {
                Head = new GenericSparkle(projectile.Center, Vector2.Zero, Color.White, Main.hslToRgb(Hue, 100, 50), 1.2f, 2, 0.06f, 3, true);
                GeneralParticleHandler.SpawnParticle(Head);
            }
            else
            {
                Head.Position = projectile.Center + projectile.velocity * 0.5f;
                Head.Time = 0;
                Head.Scale += (float)Math.Sin(Main.GlobalTime * 6) * 0.02f * projectile.scale;
            }


            if (target == null)
                target = projectile.Center.ClosestNPCAt(812f, true);

            else if (CalamityUtils.AngleBetween(projectile.velocity, target.Center - projectile.Center) < MathHelper.Pi) //Home in
            {
                float idealDirection = projectile.AngleTo(target.Center);
                float updatedDirection = projectile.velocity.ToRotation().AngleTowards(idealDirection, HomingStrenght);
                projectile.velocity = updatedDirection.ToRotationVector2() * projectile.velocity.Length() * 0.995f;
            }


            Lighting.AddLight(projectile.Center, 0.75f, 1f, 0.24f);

            if (Main.rand.Next(2) == 0)
            {
                Particle smoke = new HeavySmokeParticle(projectile.Center, projectile.velocity * 0.5f, Color.Lerp(Color.DodgerBlue, Color.MediumVioletRed, (float)Math.Sin(Main.GlobalTime * 6f)), 20, Main.rand.NextFloat(0.6f, 1.2f) * projectile.scale, 0.28f, 0, false, 0, true);
                GeneralParticleHandler.SpawnParticle(smoke);

                if (Main.rand.Next(3) == 0)
                {
                    Particle smokeGlow = new HeavySmokeParticle(projectile.Center, projectile.velocity * 0.5f, Main.hslToRgb(Hue, 1, 0.7f), 15, Main.rand.NextFloat(0.4f, 0.7f) * projectile.scale, 0.8f, 0, true, 0.05f, true);
                    GeneralParticleHandler.SpawnParticle(smokeGlow);
                }
            }
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeToEnd = MathHelper.Lerp(0.65f, 1f, (float)Math.Cos(-Main.GlobalTime * 3f) * 0.5f + 0.5f);
            float fadeOpacity = Utils.InverseLerp(1f, 0.64f, completionRatio, true) * projectile.Opacity;
            Color colorHue = Main.hslToRgb(Hue, 1, 0.8f);

            Color endColor = Color.Lerp(colorHue, Color.PaleTurquoise, (float)Math.Sin(completionRatio * MathHelper.Pi * 1.6f - Main.GlobalTime * 4f) * 0.5f + 0.5f);
            return Color.Lerp(Color.White, endColor, fadeToEnd) * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            float expansionCompletion = (float)Math.Pow(1 - completionRatio, 3);
            return MathHelper.Lerp(0f, 22 * projectile.scale * projectile.Opacity, expansionCompletion);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (TrailDrawer is null)
                TrailDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, specialShader: GameShaders.Misc["CalamityMod:TrailStreak"]);

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.GetTexture("CalamityMod/ExtraTextures/ScarletDevilStreak"));
            TrailDrawer.Draw(projectile.oldPos, projectile.Size * 0.5f - Main.screenPosition, 30);

            Texture2D texture = GetTexture("CalamityMod/Projectiles/Melee/GalaxiaBolt");
            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, Color.Lerp(lightColor, Color.White, 0.5f), projectile.rotation, texture.Size() / 2f, projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}