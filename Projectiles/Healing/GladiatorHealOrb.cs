using System;
using CalamityMod.Balancing;
using CalamityMod.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Healing
{
    public class GladiatorHealOrb : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Healing";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int target = -1;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 4800;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            float maxDistance = 150f;
            if (target < 0)
            {
                PassiveBehavior();
                for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
                {
                    Player player = Main.player[playerIndex];
                    if (player.lifeMagnet) { maxDistance = 225f; }
                    float targetDist = Vector2.Distance(player.Center, Projectile.Center);
                    if (targetDist < maxDistance)
                    {
                        maxDistance = targetDist;
                        target = playerIndex;
                    }
                }
            }
            else HealHome();
        }

        public void PassiveBehavior()
        {
            float maxYVelocity = 2f;
            CalamityUtils.StickToTiles(Projectile, false, false);
            Projectile.velocity.X *= 0.99f;
            if (Projectile.velocity.Y < maxYVelocity)
                Projectile.velocity.Y += 0.02f;
            if (Projectile.velocity.Y > maxYVelocity) { Projectile.velocity.Y = maxYVelocity; }
        }

        public void HealHome()
        {
            Player player = Main.player[target];
            //Code snippet taken from the HealingProjectile utility, to do what I want with it easier.
            Vector2 playerVector = player.Center - Projectile.Center;
            float playerDist = playerVector.Length();
            if (playerDist < 50f && Projectile.position.X < player.position.X + player.width && Projectile.position.X + Projectile.width > player.position.X && Projectile.position.Y < player.position.Y + player.height && Projectile.position.Y + Projectile.height > player.position.Y)
            {
                int heal = 10;
                player.HealPlayer(heal, HealTextType.Local);

                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/OrbHeal", 5) { Volume = 0.15f }, Projectile.Center);

                NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, target, heal, 0f, 0f, 0, 0, 0);

                Projectile.Kill();
            }

            Projectile.velocity = (playerVector.SafeNormalize(Vector2.UnitY) * 3.5f) + (player.velocity / 4);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SmallGreyscaleCircle").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                //Photoviscerator drawcode, edited slightly
                float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                Color color = Color.Lerp(Color.LightSeaGreen, Color.LimeGreen, colorInterpolation) * 0.4f;
                color.A = 0;
                Vector2 drawPosition = Projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(-32.5f, -32.5f); //Last vector is to offset the circle so that it is displayed where the hitbox actually is, instead of a bit down and to the right.
                Color outerColor = color;
                Color innerColor = color * 0.5f;
                float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);
                intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);
                if (Projectile.timeLeft <= 60) //Shrinks to nothing when projectile is nearing death
                {
                    intensity *= Projectile.timeLeft / 60f;
                }
                // Become smaller the futher along the old positions we are.
                Vector2 outerScale = new Vector2(1f) * intensity;
                Vector2 innerScale = new Vector2(1f) * intensity * 0.7f;
                outerColor *= intensity;
                innerColor *= intensity;
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, 0f, lightTexture.Size() * 0.5f, outerScale * 0.25f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, 0f, lightTexture.Size() * 0.5f, innerScale * 0.25f, SpriteEffects.None, 0);
            }
            return false;
        }
    }
}
