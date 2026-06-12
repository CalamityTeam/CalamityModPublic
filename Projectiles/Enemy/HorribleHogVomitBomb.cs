using CalamityMod.Effects;
using CalamityMod.Particles;
using CalamityMod.Utilities.Daybreak;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Enemy
{
    public class HorribleHogVomitBomb : ModProjectile, ILocalizedModType
    {
        public bool HasExplodedYet
        {
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value.ToInt();
        }

        public ref float HogIndex => ref Projectile.ai[1];

        public new string LocalizationCategory => "Projectiles.Enemy";

        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Bomb;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.scale = 0f;
            Projectile.hostile = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override bool? CanHitNPC(NPC target) => target.whoAmI != (int)HogIndex;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.FinalDamage *= 5f;

        public override void AI()
        {
            if (!HasExplodedYet && Projectile.timeLeft <= 1)
            {
                Explode();
                return;
            }

            Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 0.25f);
            Projectile.ExpandHitboxBy(Projectile.scale);
            Projectile.spriteDirection = Projectile.direction;

            Projectile.rotation += Projectile.velocity.X * 0.035f;
            if (Projectile.velocity.Y < 8f)
                Projectile.velocity.Y += 0.125f;

            Vector2 dustPosition = Projectile.Center + new Vector2(4f, -14f).RotatedBy(Projectile.rotation);
            Dust.NewDustPerfect(dustPosition, DustID.Torch, Vector2.UnitY * -2f, Scale: 0.8f);

            int vomitAmt = Main.rand.Next(1, 3);
            for (int i = 0; i < vomitAmt; i++)
            {
                Vector2 velocity = Projectile.oldVelocity * Main.rand.NextFloat(-0.8f, -0.6f);
                int dustType = Utils.SelectRandom(Main.rand, DustID.ToxicBubble, DustID.GreenBlood, DustID.Blood);
                int dust = Dust.NewDust(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), 1, 1, dustType, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(1f, 1.4f));
                Main.dust[dust].noGravity = true;
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 velocity = Projectile.oldVelocity.RotatedByRandom(MathHelper.ToRadians(20f)) * Main.rand.NextFloat(-1.8f, -0.6f) * Projectile.direction;
                Color color = Color.Lerp(Color.DarkOliveGreen, Color.Green, Main.rand.NextFloat());
                float rotationSpeed = Main.rand.NextFloat(0.01f, 0.03f) * Main.rand.NextBool().ToDirectionInt();

                TimedSmokeParticle vomit = new(Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height), velocity, color, color, Main.rand.NextFloat(0.4f, 0.6f), Main.rand.NextFloat(0.8f, 1f), Main.rand.Next(25, 35), rotationSpeed);
                GeneralParticleHandler.SpawnParticle(vomit, false, Enums.GeneralDrawLayer.BeforeProjectiles);
            }
        }

        private void Explode()
        {
            Projectile.knockBack = 8f;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 6;
            Projectile.velocity *= 0f;
            Projectile.ExpandHitboxBy(80, 80);

            int dustCloudAmt = Main.rand.Next(12, 17);
            for (int i = 0; i < dustCloudAmt; i++)
            {
                Vector2 spawnPosition = Projectile.Center;
                Vector2 velocity = Main.rand.NextVector2Circular(8f, 8f);
                Color color = Color.Lerp(Color.OrangeRed, Color.Orange, Main.rand.NextFloat());
                float rotationSpeed = Main.rand.NextFloat(0.01f, 0.03f) * Main.rand.NextBool().ToDirectionInt();

                TimedSmokeParticle explosionCloud = new(spawnPosition, velocity, new Color(10, 10, 10), color, Main.rand.NextFloat(1.2f, 1.4f), 1f, Main.rand.Next(20, 30), rotationSpeed);
                GeneralParticleHandler.SpawnParticle(explosionCloud, true);
            }

            int sparkAmt = Main.rand.Next(7, 13);
            for (int i = 0; i < sparkAmt; i++)
            {
                Vector2 spawnPosition = Projectile.Center;
                Vector2 velocity = Main.rand.NextVector2Circular(8f, 8f);
                Color color = Color.Lerp(Color.Yellow, Color.Orange, Main.rand.NextFloat());

                SparkParticle explosionSpark = new(spawnPosition, velocity, false, Main.rand.Next(20, 30), Main.rand.NextFloat(1f, 1.2f), color);
                GeneralParticleHandler.SpawnParticle(explosionSpark, true);
            }

            int dustAmt = Main.rand.Next(6, 9);
            for (int i = 0; i < dustAmt; i++)
            {
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(8f, 8f);
                Vector2 velocity = Main.rand.NextVector2Circular(8f, 8f);
                Dust.NewDust(spawnPosition, 0, 0, DustID.Torch, velocity.X, velocity.Y, Scale: Main.rand.NextFloat(1.2f, 1.6f));
            }

            CalamityUtils.AddScreenshakeAt(Projectile.Center, 3f);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
            Projectile.ai[0] = 1f;
            Projectile.netUpdate = true;
        }

        public override bool PreKill(int timeLeft)
        {
            if (!HasExplodedYet)
            {
                Explode();
                return false;
            }

            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Explode();
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            int targetIndex = Player.FindClosest(Projectile.Center, 1, 1);
            Player target = Main.player[targetIndex];
            if (Projectile.Center.Y > target.Center.Y - 48f)
                fallThrough = false;

            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 origin = texture.Size() * 0.5f;
            SpriteEffects spriteEffects = Projectile.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float timeLeftInterpolant = Utils.Remap(Projectile.timeLeft, 75, 0, 1f, 0f, true);

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 0; i < Projectile.oldPos.Length; ++i)
                {
                    Vector2 afterimageDrawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                    float trailLengthInterpolant = ((float)(Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                    Color afterimageColor = Projectile.GetAlpha(Color.Orange) * 0.5f * trailLengthInterpolant * (1f - timeLeftInterpolant);

                    Main.spriteBatch.Draw(texture, afterimageDrawPosition, null, afterimageColor, Projectile.rotation, origin, Projectile.scale * trailLengthInterpolant, spriteEffects, 0f);
                }
            }

            using (Main.spriteBatch.Scope())
            {
                Effect tintShader = CalamityShaders.BasicTintShader.Value;
                tintShader.Parameters["uColor"].SetValue(Color.White.ToVector3());
                tintShader.Parameters["uOpacity"].SetValue(1f - timeLeftInterpolant);

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, tintShader, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, spriteEffects, 0f);
                Main.spriteBatch.End();
            }

            return false;
        }
    }
}
