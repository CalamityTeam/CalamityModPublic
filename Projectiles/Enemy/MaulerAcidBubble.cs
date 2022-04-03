using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Enemy
{
    public class MaulerAcidBubble : ModProjectile
    {
        public ref float Time => ref Projectile.ai[0];
        public override string Texture => "CalamityMod/Projectiles/Enemy/SulphuricAcidBubble";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Bubble");
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 150;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            // Handle frames.
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 4 % Main.projFrames[Projectile.type];

            // Home in on players after a sufficient amount of time has passed.
            if (Time > 60f)
            {
                float flySpeed = Main.expertMode ? 17.5f : 14.5f;
                Player target = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
                if (!Projectile.WithinRange(target.Center, 50f))
                    Projectile.velocity = (Projectile.velocity * 49f + Projectile.SafeDirectionTo(target.Center) * flySpeed) / 50f;
                Projectile.tileCollide = true;
            }

            // Emit light.
            Lighting.AddLight(Projectile.Center, 0.2f, 0.6f, 0.2f);

            // Rotate.
            Projectile.rotation += Projectile.direction * 0.04f;
            Time++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Projectile.localAI[1] < 1f)
            {
                return;
            }
            target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item54, Projectile.position);
        }
    }
}
