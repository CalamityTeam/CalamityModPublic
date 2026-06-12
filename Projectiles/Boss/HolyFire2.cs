using System;
using System.Linq;
using CalamityMod.Graphics.Primitives;
using CalamityMod.NPCs.Providence;
using CalamityMod.Particles;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Utilities.Daybreak.Buffers;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class HolyFire2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            CooldownSlot = ImmunityCooldownID.Bosses;

            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void AI()
        {

            ProjectileID.Sets.TrailCacheLength[Type] = 15;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProvUtils.ApplyGFBDamage(Projectile, 120, 10);

            Lighting.AddLight(Projectile.Center, 0.3f, 0.225f, 0f);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 3)
                Projectile.frame = 0;

            float maxVelocity = Projectile.ai[0] == 0f ? 7f : CalamityWorld.death ? 11f : CalamityWorld.revenge ? 10f : Main.expertMode ? 9f : 7f;
            if (Math.Abs(Projectile.velocity.X) < maxVelocity)
                Projectile.velocity.X *= 1.05f;

            int playerTracker = Player.FindClosest(Projectile.Center, 1, 1);

            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] < 180f && Projectile.ai[1] > 60f)
            {
                float projVelocityMult = Projectile.velocity.Length();
                Vector2 playerDirection = Main.player[playerTracker].Center - Projectile.Center;
                playerDirection.Normalize();
                playerDirection *= projVelocityMult;
                float inertia = !ProvUtils.StandardAI() ? 20f : 25f;
                Projectile.velocity = (Projectile.velocity * (inertia - 1f) + playerDirection) / inertia;
                Projectile.velocity.Normalize();
                Projectile.velocity *= projVelocityMult;
            }

            float vel = Math.Clamp(((Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) / 2), 0f, 1f);

            if (Projectile.timeLeft % 3 == 0)
                GeneralParticleHandler.SpawnParticle(new GlowOrbParticle(Projectile.Center + new Vector2(Main.rand.NextFloat(15), 0).RotatedByRandom(MathHelper.TwoPi), Projectile.velocity.RotatedBy(Math.PI) * 0.2f, false, 10, Main.rand.NextFloat(0.8f, 1.2f), ProvUtils.GetProjectileColor(255)));

            if (Main.rand.NextBool())
                GeneralParticleHandler.SpawnParticle(new MediumMistParticle(Projectile.Center, Vector2.Zero, Color.LightSlateGray, Color.DarkSlateGray, Main.rand.NextFloat(vel), 150, MathHelper.ToRadians(Main.rand.NextFloat(-1f, 1f))));

            Projectile.rotation = MathHelper.Lerp(0f, MathHelper.WrapAngle(Vector2.Zero.AngleTo(Projectile.velocity) + MathHelper.ToRadians(-90f)), Math.Clamp(vel, 0f, 1f));
        }

        public override Color? GetAlpha(Color lightColor) => ProvUtils.GetProjectileColor(lightColor);

        public override bool PreDraw(ref Color lightColor)
        {

            Main.spriteBatch.End(out var ss);
            var device = Main.instance.GraphicsDevice;
            using var lease = RenderTargetPool.Shared.Rent(
                device,
                Main.screenWidth / 2,
                Main.screenHeight / 2,
                RenderTargetDescriptor.Default
            );
            using (lease.Scope(clearColor: Color.Transparent))
            {
                var list = Projectile.oldPos.Take(8).ToList();

                list.Insert(0, Projectile.position + (Projectile.rotation + MathHelper.PiOver2 * Projectile.spriteDirection).ToRotationVector2() * 16f);

                GameShaders.Misc["CalamityMod:ImpFlameTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
                PrimitiveRenderer.RenderTrail(list.ToArray(), new(FireWidthFunction, FireColorFunction, (_, _) => Projectile.Size * 0.5f, smoothen: true, pixelate: false, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"], useUnscaledMatrices: true), Projectile.oldPos.Length + 32);
            }

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            Main.spriteBatch.Draw(lease.Target, Vector2.Zero, null, Color.White * 0.75f, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();

            Main.spriteBatch.Begin(ss);
            Texture2D texture = ProvUtils.StandardAI() ? Terraria.GameContent.TextureAssets.Projectile[Type].Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HolyFire2Night").Value;
            int framing = texture.Height / Main.projFrames[Type];
            int y6 = framing * Projectile.frame;
            Projectile.DrawBackglow(ProvUtils.GetProjectileColor(lightColor, true), 4f, texture);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, framing)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public float FireWidthFunction(float completion, Vector2 pos)
        {
            float width;
            float maxBodyWidth = 16f * Projectile.scale;
            float curveRatio = 0.2f;
            var positions = Projectile.oldPos.ToList();
            positions.RemoveAll(x => x == Vector2.Zero);
            // Crop the tip of the trail into a conic shape.
            if (completion < curveRatio)
                width = MathF.Pow(completion / curveRatio, 0.5f) * maxBodyWidth;
            else
                width = Utils.Remap(completion, curveRatio, 1f, maxBodyWidth, 0f);

            // Pulse inwards and outwards over time.
            float pulseInterpolant = MathF.Cos(MathHelper.Pi * completion - Main.GlobalTimeWrappedHourly * 20f) * 0.5f + 0.5f;
            float additionalPulseWidth = MathHelper.Lerp(0f, 12f, pulseInterpolant);
            return (width + additionalPulseWidth) * positions.Count() / (float)ProjectileID.Sets.TrailCacheLength[Type];
        }

        public Color FireColorFunction(float completion, Vector2 pos)
        {
            Color mainColor = ProvUtils.GetProjectileColor(255);
            Color endColor = Color.Lerp(mainColor, Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            return Color.Lerp(mainColor, endColor, completion) * Projectile.Opacity;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            Projectile.position.X = Projectile.position.X + (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (Projectile.height / 2);
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (Projectile.height / 2);
            int dustType = ProvUtils.GetDustID();
            for (int i = 0; i < 5; i++)
            {
                int holyDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                Main.dust[holyDust].noGravity = true;
                if (Main.rand.NextBool())
                {
                    Main.dust[holyDust].scale = 0.5f;
                    Main.dust[holyDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 10; j++)
            {
                int holyDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 3f);
                Main.dust[holyDust2].noGravity = true;
                holyDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2f);
                Main.dust[holyDust2].noGravity = true;
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // If the player is dodging, don't apply debuffs
            if (info.Damage <= 0 || target.creativeGodMode)
                return;

            ProvUtils.ApplyDebuffs(target, 120);
        }
    }
}
