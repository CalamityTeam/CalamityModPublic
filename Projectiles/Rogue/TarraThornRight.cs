using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class TarraThornRight : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/Magic/NettleRight";

        public static int TotalSegments = 10;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.alpha -= 100;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    Projectile.ai[1] = 1f;

                    // This projectile normally does not move by itself, so this will manually move it one time only
                    // This is only for the first segment
                    if (Projectile.ai[0] == 0f)
                    {
                        Projectile.ai[0]++;
                        Projectile.position += Projectile.velocity;
                    }

                    // Spawn the next segment
                    if (Main.myPlayer == Projectile.owner && Projectile.ai[0] < TotalSegments)
                    {
                        int nextSegment = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Projectile.velocity, Projectile.velocity, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] + 1f);
                        NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, nextSegment);
                    }
                }
            }
            else // Begin fading out
            {
                int AlphaPerFrame = 8;
                Projectile.alpha += AlphaPerFrame;
                if (Projectile.alpha == AlphaPerFrame * 21)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        Dust thorn = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 7, Projectile.velocity.X * 0.025f, Projectile.velocity.Y * 0.025f, 200, default, 1.3f);
                        thorn.noGravity = true;
                        thorn.velocity *= 0.5f;
                    }
                }

                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
        }

        // This is essential for Vilethorn-type projectiles, as velocity is a stored parameter and isn't supposed to actually move the projectile
        public override bool ShouldUpdatePosition() => false;

        // Draw the tip for the final thorn
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            if (Projectile.ai[0] == TotalSegments)
                texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/NettleTip").Value;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
