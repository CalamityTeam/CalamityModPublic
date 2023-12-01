using CalamityMod.Items.Ammo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class MortarRoundProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Items/Ammo/MortarRound";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 150;
            Projectile.DamageType = DamageClass.Ranged;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // If moving fast enough, produce dust. Since it's a bullet, it should always be moving fast enough.
            if (Projectile.velocity.Length() >= 8f)
            {
                for (int d = 0; d < 2; d++)
                {
                    float xOffset = 0f;
                    float yOffset = 0f;
                    if (d == 1)
                    {
                        xOffset = Projectile.velocity.X * 0.5f;
                        yOffset = Projectile.velocity.Y * 0.5f;
                    }
                    int fire = Dust.NewDust(new Vector2(Projectile.position.X + 3f + xOffset, Projectile.position.Y + 3f + yOffset) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, 6, 0f, 0f, 100, default, 1f);
                    Main.dust[fire].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                    Main.dust[fire].velocity *= 0.2f;
                    Main.dust[fire].noGravity = true;
                    int smoke = Dust.NewDust(new Vector2(Projectile.position.X + 3f + xOffset, Projectile.position.Y + 3f + yOffset) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, DustID.Smoke, 0f, 0f, 100, default, 0.5f);
                    Main.dust[smoke].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                    Main.dust[smoke].velocity *= 0.05f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft) => Explode();

        private void Explode()
        {
            // Apply damage a second time on explosion. This explosion also has double knockback.
            Projectile.ExpandHitboxBy(MortarRound.HitboxBlastRadius);
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.knockBack *= 2f;
            Projectile.Damage();

            // Actually destroy tiles. Blast radius is significantly increased in GFB.
            if (Projectile.owner == Main.myPlayer)
                Projectile.ExplodeTiles(MortarRound.TileBlastRadius, true);

            // Play standard explosion sound.
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            // Spawn standard explosion dust and gores.
            SpawnDust(Projectile);
            SpawnExplosionGores(Projectile);
        }

        // Spawn explosion dust (smoke and fire).
        // Reused by Rubber Mortar Rounds.
        internal static void SpawnDust(Projectile p)
        {
            for (int d = 0; d < 40; d++)
            {
                int smoke = Dust.NewDust(p.position, p.width, p.height, DustID.Smoke, 0f, 0f, 100, default, 2f);
                Main.dust[smoke].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[smoke].scale = 0.5f;
                    Main.dust[smoke].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 70; d++)
            {
                int fire = Dust.NewDust(p.position, p.width, p.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[fire].noGravity = true;
                Main.dust[fire].velocity *= 5f;
                fire = Dust.NewDust(p.position, p.width, p.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[fire].velocity *= 2f;
            }
        }

        // Spawn explosion gores (smoke puffs).
        // Reused by Rubber Mortar Rounds.
        internal static void SpawnExplosionGores(Projectile p)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            Vector2 goreSource = p.Center;
            int goreAmt = 3;
            Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
            for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
            {
                float velocityMult = 0.33f;
                if (goreIndex < (goreAmt / 3))
                {
                    velocityMult = 0.66f;
                }
                if (goreIndex >= (2 * goreAmt / 3))
                {
                    velocityMult = 1f;
                }
                int type = Main.rand.Next(61, 64);
                int smoke = Gore.NewGore(p.GetSource_FromAI(), source, default, type, 1f);
                Gore gore = Main.gore[smoke];
                gore.velocity *= velocityMult;
                gore.velocity.X += 1f;
                gore.velocity.Y += 1f;
                type = Main.rand.Next(61, 64);
                smoke = Gore.NewGore(p.GetSource_FromAI(), source, default, type, 1f);
                gore = Main.gore[smoke];
                gore.velocity *= velocityMult;
                gore.velocity.X -= 1f;
                gore.velocity.Y += 1f;
                type = Main.rand.Next(61, 64);
                smoke = Gore.NewGore(p.GetSource_FromAI(), source, default, type, 1f);
                gore = Main.gore[smoke];
                gore.velocity *= velocityMult;
                gore.velocity.X += 1f;
                gore.velocity.Y -= 1f;
                type = Main.rand.Next(61, 64);
                smoke = Gore.NewGore(p.GetSource_FromAI(), source, default, type, 1f);
                gore = Main.gore[smoke];
                gore.velocity *= velocityMult;
                gore.velocity.X -= 1f;
                gore.velocity.Y -= 1f;
            }
        }
    }
}
