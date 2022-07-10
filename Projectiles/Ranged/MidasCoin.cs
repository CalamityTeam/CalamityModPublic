using System;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using ReLogic.Content;

namespace CalamityMod.Projectiles.Ranged
{
    public class MidasCoin : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coin");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 8;
        }

        public static int Lifetime = 950;
        public float LifetimeCompletion => MathHelper.Clamp((Lifetime - Projectile.timeLeft) / (float)Lifetime, 0f, 1f);
        public float FadePercent => Math.Clamp(Projectile.timeLeft / FadeTime, 0f, 1f);
        public static float FadeTime => 30f;
        public Player Owner => Main.player[Projectile.owner];
        public static Asset<Texture2D> SheenTex;
        public static Asset<Texture2D> BloomTex;

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.timeLeft = Lifetime;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
            Projectile.scale = 1.1f;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            Lighting.AddLight(Projectile.Center, Color.Goldenrod.ToVector3() * 0.2f);


            Projectile.rotation = Projectile.velocity.X * 0.5f;
            Projectile.velocity *= 0.998f;

            if (Projectile.timeLeft < Lifetime - 100)
                Projectile.velocity.Y += 0.01f;

            if (Projectile.Center.Distance(Owner.MountedCenter) > 1300 && Projectile.timeLeft > FadeTime)
                Projectile.timeLeft = (int)FadeTime;

            if (Main.rand.NextBool(10))
            {
                int schyste = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 246);
                Main.dust[schyste].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

            return base.OnTileCollide(oldVelocity);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = new Rectangle((int)Projectile.ai[0] * tex.Width / 2, Projectile.frame *  tex.Height / Main.projFrames[Projectile.type], tex.Width / 2 - 2, tex.Height / Main.projFrames[Projectile.type] - 2);

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor) * FadePercent, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);


            float sheenOpacity = (float)Math.Sin(Math.Clamp((Lifetime - Projectile.timeLeft) / (MidasPrime.RipeningTime * 2f), 0f, 1f) * MathHelper.Pi);

            if (sheenOpacity > 0f)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                if (SheenTex == null)
                    SheenTex = Request<Texture2D>("CalamityMod/Particles/HalfStar");
                Texture2D shineTex = SheenTex.Value;

                if (BloomTex == null)
                    BloomTex = Request<Texture2D>("CalamityMod/Particles/BloomCircle");
                Texture2D bloomTex = BloomTex.Value;

                Vector2 shineScale = new Vector2(1f, 3f - sheenOpacity * 2f);
                Color shineColor = Projectile.ai[0] == 0 ? Color.Silver : Color.Goldenrod;


                Main.EntitySpriteDraw(bloomTex, Projectile.Center - Main.screenPosition, null, shineColor * sheenOpacity * 0.3f, MathHelper.PiOver2, bloomTex.Size() / 2f, shineScale * Projectile.scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(shineTex, Projectile.Center - Main.screenPosition, null, shineColor * sheenOpacity * 0.7f, MathHelper.PiOver2, shineTex.Size() / 2f, shineScale * Projectile.scale, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer && Main.rand.NextBool(4))
            {
                int coin = Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Center, Vector2.One, Projectile.ai[0] == 0 ? ItemID.SilverCoin : ItemID.GoldCoin);
                Main.item[coin].GetGlobalItem<MidasPrimeItem>().magnetMode = true;
            }
        }
    }
}
