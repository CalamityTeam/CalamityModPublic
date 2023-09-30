using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class WitherBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 25;
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.localNPCHitCooldown = -1;
            Projectile.width = Projectile.height = (int)(72 * Projectile.scale);
            Projectile.scale = 0.8f;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void AI()
        {
            float oldSpeed = Projectile.velocity.Length();

            NPC potentialTarget = Projectile.Center.MinionHoming(800f, Main.player[Projectile.owner]);
            if (potentialTarget != null)
                Projectile.velocity = (Projectile.velocity * 3f + Projectile.SafeDirectionTo(potentialTarget.Center) * oldSpeed) / 4f;
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * oldSpeed * 1.01f;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
            Projectile.timeLeft = 10;
            Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 0.25f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D boltTexture = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float completionRatio = i / (float)Projectile.oldPos.Length;
                Color drawColor = Color.Lerp(lightColor, Color.Olive, 0.6f);
                drawColor = Color.Lerp(drawColor, Color.Black, completionRatio);
                drawColor = Color.Lerp(drawColor, Color.Transparent, completionRatio);

                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition;
                Main.EntitySpriteDraw(boltTexture, drawPosition, null, Projectile.GetAlpha(drawColor), Projectile.oldRot[i], boltTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
            {
                for (int i = 0; i < Projectile.oldPos.Length / 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Dust plague = Dust.NewDustDirect(Projectile.oldPos[i], Projectile.width / 2, Projectile.height / 2, 107);
                        plague.velocity = (Projectile.oldRot[i] - MathHelper.PiOver2).ToRotationVector2() * 4.5f + Main.rand.NextVector2Circular(2f, 2f);
                        plague.color = Color.Olive;
                        plague.noGravity = true;
                    }
                }
            }
        }
    }
}
