using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class DoGDeath : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";

        public float TelegraphDelay
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public Vector2 OldVelocity;
        public const float TelegraphTotalTime = 35f;
        public const float TelegraphFadeTime = 5f;
        public const float TelegraphWidth = 2400f;
        public const float FadeTime = 20f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.Opacity = 0f;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(OldVelocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            OldVelocity = reader.ReadVector2();
        }

        public override void AI()
        {
            // Determine the relative opacities for each player based on their distance.
            // This has a lower bound of 0.35 to prevent the laser from going completely invisible and players getting hit by cheap shots.
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.netUpdate = true;
            }

            // Fade in after telegraphs have faded.
            if (TelegraphDelay > TelegraphTotalTime)
            {
                if (Projectile.timeLeft < FadeTime)
                {
                    Projectile.Opacity = Projectile.timeLeft / FadeTime;
                }
                else
                {
                    if (Projectile.Opacity < 1f)
                        Projectile.Opacity += 0.05f;
                    if (Projectile.Opacity > 1f)
                        Projectile.Opacity = 1f;
                }

                // If an old velocity is in reserve, set the true velocity to it and make it as "taken" by setting it to <0,0>
                if (OldVelocity != Vector2.Zero)
                {
                    Projectile.velocity = OldVelocity * (BossRushEvent.BossRushActive ? 1.25f : 1f);
                    OldVelocity = Vector2.Zero;
                    Projectile.netUpdate = true;
                }

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            // Otherwise, be sure to save the velocity the projectile started with. It will be set again when the telegraph is over.
            else if (OldVelocity == Vector2.Zero)
            {
                OldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
                Projectile.rotation = OldVelocity.ToRotation() + MathHelper.PiOver2;
            }

            TelegraphDelay++;
        }

        public override bool CanHitPlayer(Player target) => TelegraphDelay > TelegraphTotalTime && Projectile.Opacity == 1f;

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            if (TelegraphDelay > TelegraphTotalTime && Projectile.Opacity == 1f)
                target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 0) * Projectile.Opacity;

        public override bool PreDraw(ref Color lightColor)
        {
            if (TelegraphDelay >= TelegraphTotalTime)
                return true;

            // for old times sake
            if (Main.zenithWorld)
                return false;

            Texture2D laserTelegraph = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/LaserWallTelegraphBeam").Value;

            float yScale = 2f;
            if (TelegraphDelay < TelegraphFadeTime)
                yScale = MathHelper.Lerp(0f, 2f, TelegraphDelay / TelegraphFadeTime);
            if (TelegraphDelay > TelegraphTotalTime - TelegraphFadeTime)
                yScale = MathHelper.Lerp(2f, 0f, (TelegraphDelay - (TelegraphTotalTime - TelegraphFadeTime)) / TelegraphFadeTime);

            Vector2 scaleInner = new Vector2(TelegraphWidth / laserTelegraph.Width, yScale);
            Vector2 origin = laserTelegraph.Size() * new Vector2(0f, 0.5f);
            Vector2 scaleOuter = scaleInner * new Vector2(1f, 1.6f);

            Color colorOuter = Color.Lerp(Color.Cyan, Color.Purple, TelegraphDelay / TelegraphTotalTime * 2f % 1f); // Iterate through purple and cyan once and then flash.
            Color colorInner = Color.Lerp(colorOuter, Color.White, 0.75f);

            colorOuter *= 0.7f;
            colorInner *= 0.7f;

            Main.EntitySpriteDraw(laserTelegraph, Projectile.Center - Main.screenPosition, null, colorInner, OldVelocity.ToRotation(), origin, scaleInner, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(laserTelegraph, Projectile.Center - Main.screenPosition, null, colorOuter, OldVelocity.ToRotation(), origin, scaleOuter, SpriteEffects.None, 0);
            return false;
        }
    }
}
