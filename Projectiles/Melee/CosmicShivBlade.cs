using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class CosmicShivBlade : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public Vector2 StartPosition = Vector2.Zero;
        public int ColorIndex = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.penetrate = 5;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 6000;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 3;
            Projectile.alpha = 0;
            Projectile.scale = 1.2f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            StartPosition = Projectile.position;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            ColorIndex = Main.rand.Next(0, CosmicShivTrail.DustColors.Count);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            if (Projectile.Distance(StartPosition) > Projectile.ai[0] * 2)
                Projectile.Kill();

            Lighting.AddLight(Projectile.Center, Projectile.GetAlpha(Color.White).ToVector3() * 0.2f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Rectangle sourceRectangle = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = sourceRectangle.Size() / 2f;
            float rotation = Projectile.rotation;
            Color randomColor = Projectile.GetAlpha(lightColor);

            // Afterimages to make the blade look longer
            float scaleMult = 1.2f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 drawPos = Projectile.oldPos[i] + (Projectile.Size / 2f) - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                Color color = randomColor * ((float)(Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);

                // Smaller, more visible layer
                Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(sourceRectangle), color, rotation, origin, Projectile.scale * scaleMult, SpriteEffects.None, 0f);

                // Bigger, less visible layer
                Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(sourceRectangle), color * 0.5f, rotation, origin, Projectile.scale * 1.6f * scaleMult, SpriteEffects.None, 0f);
                scaleMult *= 0.95f;
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.RainbowMk2, 0f, 0f, 0, Projectile.GetAlpha(Color.Purple) * 0.4f, 1.5f);
                dust.noGravity = true;
            }
        }

        // Probably not the right way to use this function, but it works (hopefully)
        public override Color? GetAlpha(Color lightColor)
        {
            // Take random color from a list and make it more white
            Color color = CosmicShivTrail.DustColors[ColorIndex] * Projectile.Opacity;
            color = Color.Lerp(color, Color.White, 0.6f);
            color.A = 0;
            return color;
        }
    }
}
