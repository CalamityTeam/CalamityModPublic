using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class BetterHornetStinger : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 180;
            Projectile.width = Projectile.height = 32;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (!Main.dedServ)
            {
                if (Main.rand.NextBool(3))
                {
                    Dust trailDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.JungleGrass);
                    trailDust.noGravity = true;
                    trailDust.noLight = true;
                    trailDust.noLightEmittence = true;
                }

                Projectile.alpha = (int)Utils.Remap(Projectile.timeLeft, 30, 0, 0, 255);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.PotionSickness, 180);

            if (!Main.dedServ)
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust impactDust = Dust.NewDustPerfect(target.Center, DustID.JungleGrass, Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(3f, 6f));
                    impactDust.noGravity = true;
                    impactDust.noLight = true;
                    impactDust.noLightEmittence = true;
                }
            }

            Projectile.netUpdate = true;
            if (Projectile.netSpam >= 10)
                Projectile.netSpam = 9;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame();
            Vector2 origin = frame.Size() * 0.5f;

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation + MathHelper.PiOver2, origin, Projectile.scale, SpriteEffects.None);

            return false;
        }
    }
}
