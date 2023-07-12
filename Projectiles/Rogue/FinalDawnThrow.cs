using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class FinalDawnThrow : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public const float DesiredSpeed = 38;
        public const float InterpolationTime = 15;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.0f;
            Projectile.extraUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player is null || player.dead)
                Projectile.Kill();

            if (Projectile.localAI[0] == 0)
            {
                SoundEngine.PlaySound(FinalDawn.UseSound, Projectile.Center);
                Projectile.localAI[0] = 1;
            }

            Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation += 0.25f * Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 30)
            {
                Vector2 desiredVelocity = Projectile.SafeDirectionTo(player.Center) * DesiredSpeed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / InterpolationTime);

                float distance = Projectile.Distance(player.Center);
                if (distance < 64)
                    Projectile.Kill();
            }

            int idx = Dust.NewDust(Projectile.position, Projectile.width , Projectile.height, ModContent.DustType<FinalFlame>(), 0f, 0f, 0, default, 0.5f);
            Main.dust[idx].velocity *= 0.5f;
            Main.dust[idx].velocity += Projectile.velocity * 0.5f;
            Main.dust[idx].noGravity = true;
            Main.dust[idx].noLight = false;
            Main.dust[idx].scale = 1.0f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D scytheTexture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D scytheGlowTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/FinalDawnThrow_Glow").Value;
            int height = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int yStart = height * Projectile.frame;
            Main.spriteBatch.Draw(scytheTexture,
                                  Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                                  new Rectangle?(new Rectangle(0, yStart, scytheTexture.Width, height)),
                                  Projectile.GetAlpha(lightColor),
                                  Projectile.rotation,
                                  new Vector2(scytheTexture.Width / 2f, height / 2f),
                                  Projectile.scale,
                                  Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            Main.spriteBatch.Draw(scytheGlowTexture,
                                  Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY,
                                  new Rectangle?(new Rectangle(0, yStart, scytheTexture.Width, height)),
                                  Projectile.GetAlpha(Color.White),
                                  Projectile.rotation,
                                  new Vector2(scytheTexture.Width / 2f, height / 2f),
                                  Projectile.scale,
                                  Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                                  0f);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Dragonfire>(), 240);
        }
    }
}
