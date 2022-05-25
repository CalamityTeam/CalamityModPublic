using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.NPCs.Yharon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class YharonBulletHellVortex : ModProjectile
    {
        public ref float TimeCountdown => ref Projectile.ai[0];
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bullet Hell Vortex");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = 408;
            Projectile.height = 408;
            Projectile.scale = 0.05f;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 60000;
        }

        public override void AI()
        {
            if (Main.npc[(int)Projectile.ai[1]].active && Main.npc[(int)Projectile.ai[1]].type == ModContent.NPCType<Yharon>())
            {
                if (TimeCountdown > 0f)
                {
                    if (TimeCountdown <= 20f)
                        Projectile.scale = MathHelper.Clamp(Projectile.scale - 0.05f, 0f, 1f);
                    else if (Projectile.scale < 1f)
                        Projectile.scale = MathHelper.Clamp(Projectile.scale + 0.05f, 0f, 1f);

                    TimeCountdown--;
                }
                else
                    Projectile.Kill();
            }
            else
                Projectile.Kill();
        }

        internal Color ColorFunction(float completionRatio) => Color.Lerp(Color.Yellow, Color.Yellow, completionRatio);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D vortexTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/OldDukeVortex").Value;
            for (int i = 0; i < 110; i++)
            {
                float angle = MathHelper.TwoPi * i / 50f + Main.GlobalTimeWrappedHourly * MathHelper.TwoPi;
                Color drawColor = Color.White * 0.04f;
                drawColor.A = 0;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition;

                drawPosition += (angle + Main.GlobalTimeWrappedHourly * i / 16f).ToRotationVector2() * 6f;
                Main.EntitySpriteDraw(vortexTexture, drawPosition, null, drawColor, angle + MathHelper.PiOver2, vortexTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }
    }
}
