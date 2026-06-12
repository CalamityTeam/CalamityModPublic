using CalamityMod.NPCs.Providence;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class MoltenBlob : ModProjectile, ILocalizedModType
    {
        float Landing = 1f;
        bool colliding = false;

        bool inLava = false;

        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.scale = 1.5f;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            ProvUtils.ApplyGFBDamage(Projectile, 120, 10);

            Lighting.AddLight(Projectile.Center, 0.3f, 0.225f, 0f);

            Landing += 0.1f;
            Landing = MathHelper.Clamp(Landing, 0f, 1f);

            colliding = Projectile.velocity.Y == 0f;

            if (colliding)
            {
                Landing = 0f;
                Projectile.localAI[2] = 5;

                Projectile.velocity.X *= 0.8f;
            }

            Projectile.velocity.X *= 0.95f;

            if (Projectile.wet || Projectile.lavaWet)
            {
                if (!inLava)
                {
                    Projectile.position.Y -= Projectile.velocity.Y;
                    Projectile.velocity.Y = 0f;
                    inLava = true;
                }
            }
            else
            {
                if (!inLava)
                {
                    Projectile.velocity.Y += 0.15f;
                }
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 1)
                Projectile.frame = 0;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override Color? GetAlpha(Color lightColor)
        {
            return ProvUtils.GetProjectileColor(0);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ProvUtils.StandardAI() ? Terraria.GameContent.TextureAssets.Projectile[Type].Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/MoltenBlobNight").Value;
            int framing = Terraria.GameContent.TextureAssets.Projectile[Type].Value.Height / Main.projFrames[Type];
            int y6 = framing * (1 + Projectile.frame);
            float squish = CalamityUtils.SineBumpEasing(Landing, 1) * 0.5f;
            Projectile.rotation = 0f;
            if (!colliding)
            { 
                y6 = 0;
                squish = -MathHelper.Clamp(Projectile.velocity.Length() / 15, 0, 0.5f);
                Projectile.rotation = Vector2.Zero.AngleTo(Projectile.velocity) + MathHelper.PiOver2;
            }
            Vector2 vec = new Vector2(0, framing * squish);
            Projectile.DrawBackglow(ProvUtils.GetProjectileColor(Projectile.alpha, true), 4f, new Vector2(1 + squish, 1 - squish), texture, offset: vec);
            Main.spriteBatch.Draw(texture, Projectile.Center + vec - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, framing)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, framing / 2f), new Vector2(1 + squish, 1 - squish), SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            Color hiColor = ProvUtils.GetProjectileColor(255, false);
            Color loColor = ProvUtils.GetProjectileColor(0, true);

            for (int i = 0; i < 25; i++)
            {
                GeneralParticleHandler.SpawnParticle(new GlowOrbParticle(Projectile.Center, new Vector2(Main.rand.NextFloat(3), 0).RotatedByRandom(MathHelper.TwoPi), false, 10, Main.rand.NextFloat(0.1f, 0.4f), hiColor));
            }

            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/BloomCircle", Vector2.One, 0f, 0.5f, 0.1f, 4));

            for (float i = 0; i < 1; i += 0.25f)
            {
                GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/SoftRoundExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.002f * i, 0.0125f * i, 8));
            }
            GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, hiColor, "CalamityMod/Particles/ShatteredExplosion", Vector2.One, Main.rand.NextFloat(MathHelper.TwoPi), 0.002f, 0.015f, 5));
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
