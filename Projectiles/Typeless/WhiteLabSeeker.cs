using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class WhiteLabSeeker : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public ref float Time => ref Projectile.ai[0];
        public override string Texture => "CalamityMod/Items/LabFinders/WhiteSeekingMechanism";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override void AI() => RedLabSeeker.Behavior(Projectile, CalamityWorld.IceLabCenter, Color.White, ref Time);

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(16f, 16f), 267);
                dust.color = Color.White;
                dust.scale = Main.rand.NextFloat(0.95f, 1.25f);
                dust.velocity = Main.rand.NextVector2Circular(2.5f, 2.5f);
                dust.velocity.Y -= 1.5f;
                dust.fadeIn = 1.2f;
                dust.noGravity = true;
            }
        }

        public override bool? CanCutTiles() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            if (Time > 80f && Projectile.ai[1] == 0f)
            {
                for (int i = 0; i < 24; i += 2)
                {
                    Vector2 drawPosition = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * i * Utils.GetLerpValue(80f, 125f, Time, true) * 25f - Main.screenPosition;
                    Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor) * (1f - i / 24f), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
                }
            }
            return true;
        }
    }
}
