using System;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Ranged
{
    public class CrackshotCoin : ModProjectile
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
            Rectangle frame = new Rectangle(0, Projectile.frame *  tex.Height / Main.projFrames[Projectile.type], tex.Width, tex.Height / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor) * FadePercent, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            
            return false;
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer && Main.rand.NextBool(4))
                Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Center, Vector2.One, ItemID.CopperCoin);
        }
    }
}
