using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Projectiles.Summon
{
    public class YharonMinionFireball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public ref float InitialSpeed => ref Projectile.ai[0];
        public override string Texture => "CalamityMod/Projectiles/Boss/YharonFireball";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % Main.projFrames[Projectile.type];
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.075f, 0f, 1f);

            if (InitialSpeed == 0f)
            {
                InitialSpeed = Projectile.velocity.Length();
                Projectile.netUpdate = true;
                return;
            }

            NPC potentialTarget = Projectile.Center.MinionHoming(600f, Main.player[Projectile.owner]);
            if (potentialTarget != null)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(potentialTarget.Center) * InitialSpeed, 0.15f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Dragonfire>(), 120);

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            SpriteEffects direction = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                direction = SpriteEffects.FlipHorizontally;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    Vector2 afterimageDrawPosition = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
                    Color afterimageColor = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - i) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, afterimageDrawPosition, frame, afterimageColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0);
                }
            }

            Vector2 drawPosition = Projectile.Center - Main.screenPosition - Vector2.UnitY * Projectile.gfxOffY;
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);

            for (int d = 0; d < 2; d++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 55, 0f, 0f, 50, default, 1.5f);

            for (int d = 0; d < 20; d++)
            {
                Dust fire = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 0, default, 2.2f);
                fire.noGravity = true;
                fire.velocity *= 4f;

                fire = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 50, default, 1.5f);
                fire.velocity *= 5f;
                fire.noGravity = true;
            }
        }
    }
}
