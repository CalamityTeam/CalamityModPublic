using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class RicoshotCoin : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        internal static readonly SoundStyle BlingSound = new("CalamityMod/Sounds/Custom/Ultrabling") { PitchVariance = 0.5f };
        internal static readonly SoundStyle BlingHitSound = new("CalamityMod/Sounds/Custom/UltrablingHit") { PitchVariance = 0.5f };
        private static Asset<Texture2D> sheenAsset;
        private static Asset<Texture2D> bloomAsset;

        public static float UsedCoinGrabRangeMultiplier = 5f;
        public static float CoinTossForce = 7.33f;

        // This variable controls how far shots are allowed to ricochet from one coin to another.
        // This creates a circle around the first coin hit to check for other coins for multi-ricoshots.
        public static float MaxIntraCoinRicoshotDistance = 1000f;

        // This variable controls how far shots are allowed to ricochet into NPCs.
        // This is intentionally higher than the intra-coin distance to make it easier to land successful ricoshots.
        public static readonly float RicoshotSearchDistance = 2000f;

        // Superprediction ratio for ricoshot targeting of DSO bullseyes and NPCs.
        // Valid range is 0.0 to 1.0.
        // Because ricoshots have slight frame delays for dramatic effect, setting this too high will make them miss hilariously.
        public static float SuperpredictionRatio = 0.1f;

        // The first copper coin struck adds +70% damage. Copper coins beyond the first add +20% damage.
        // Maximum: 4 copper coins = +130%
        //
        // The copper numbers are intentionally significantly lower because Crackshot Colt is pre-boss.
        // That, and the mechanic will always be available because they're dirt cheap. Killing a single enemy lets you perform dozens more ricoshots.
        public static float CopperBonus = 0.7f;
        public static float CopperMulticoinBonus = 0.2f;

        // The first silver coin struck adds +150% damage. Silver coins beyond the first add +50% damage.
        // Maximum: 4 silver coins = +300%
        //
        // Midas Prime uses coins from the piggy bank, so you will almost always be using gold.
        // Silver is not much worse than gold, because if you're broke and can't use gold you shouldn't be significantly penalized.
        // However, spending multiple silver on a multi-ricoshot is not too expensive, so the multicoin bonus is much lower.
        public static float SilverBonus = 1.5f;
        public static float SilverMulticoinBonus = 0.5f;

        // The first gold coin struck adds +200% damage. Gold coins beyond the first add +85% damage.
        // Maximum: 4 gold coins = +455%
        //
        // The intended usage of Midas Prime is with a single gold coin that you (ideally) pick up afterwards, especially when fighting normal enemies.
        // Gold multicoins are intentionally stronger than copper or silver ones because multi-ricoshots delete the extra gold coins, and that gets pricey FAST.
        // However, they can't be too strong, or the late-game burst damage abusability gets too ridiculous.
        public static float GoldBonus = 2f;
        public static float GoldMulticoinBonus = 0.85f;

        // Lifetime of a coin. Defined in terms of updates because Terraria's engine is trash.
        internal static readonly int UpdateCount = 4;
        internal static readonly int CoinLifetime = UpdateCount * CalamityUtils.SecondsToFrames(7);

        // Coins fade out for the last 30 frames of their existence.
        // This fading begins immediately if they are an extreme distance from the player.
        private static readonly int FadeoutTime = UpdateCount * 30;
        private static readonly float ForceFadeDistance = 2000f;

        // Number of frames of delay before firing at the coin guarantees a crit.
        public static int CritDelayFrames = 22;
        internal static int CritDelayTime => UpdateCount * CritDelayFrames;

        // 0f = copper
        // 1f = silver
        // 2f = gold
        // 3f = platinum (not currently implemented)
        internal ref float CoinType => ref Projectile.ai[0];

        // A counter for how long the coin remains frozen in place when struck.
        internal ref float ShotFreezeTimer => ref Projectile.ai[1];
        // The default length of the above freeze timer.
        internal static int RicochetPause = UpdateCount * 22;

        // Marker for whether the coin has been shot.
        // localAI is set to the number of the coin in a chain; i.e. the first coin gets 1, the second coin gets 2...
        public bool HasBeenShot => Projectile.localAI[0] > 0f;

        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 60;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;

            // 7 seconds should be more than enough to hit any coin.
            Projectile.MaxUpdates = UpdateCount;
            Projectile.timeLeft = CoinLifetime;

            Projectile.scale = 1.1f;
        }

        // Coins cannot damage anything.
        public override bool? CanDamage() => false;

        // Coins stop moving when shot for a certain number of frames.
        public override bool ShouldUpdatePosition() => ShotFreezeTimer <= 0;

        public override void AI()
        {
            // Decrement the timer for how long the coin should remain frozen, if it is currently frozen.
            if (ShotFreezeTimer > 0f)
            {
                --ShotFreezeTimer;

                // On the frame the coin stops being frozen, destroy it immediately.
                if (ShotFreezeTimer <= 0)
                {
                    Projectile.Kill();
                    return;
                }
            }

            // Update the coin animation so it spins.
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            // Produce a tiny amount of light based on coin type.
            // Dropped coins typically do not produce light, but do emit dust which is rendered fullbright.
            // Ricoshot coins do both.
            int dustID = DustID.CopperCoin;
            switch (CoinType)
            {
                case 1f:
                    Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.14f);
                    dustID = DustID.SilverCoin;
                    break;
                case 2f:
                    Lighting.AddLight(Projectile.Center, Color.Goldenrod.ToVector3() * 0.2f);
                    dustID = DustID.GoldCoin;
                    break;
                default:
                    Lighting.AddLight(Projectile.Center, Color.DarkOrange.ToVector3() * 0.11f);
                    break;
            }

            // Randomly spawn dust like standard coins would.
            if (Main.rand.NextBool(10))
            {
                Dust d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustID);
                d.noGravity = true;
            }

            // The coin spins in midair based on how fast it is traveling horizontally.
            Projectile.rotation = Projectile.velocity.X * 0.5f;

            // The coin experiences a gravity calculated to enable similar movement to the player.
            // No other modifications are made to its velocity, and it is updated only once per frame to match player physics.
            // This enables perfect parabolic arcs like in ULTRAKILL.
            if (Projectile.FinalExtraUpdate())
            {
                float coinGravity = Player.defaultGravity / Projectile.MaxUpdates;
                Projectile.velocity.Y += coinGravity;

                // Max coin fall speed commented out because it seems unnecessary.
                // It would also mess with the perfect parabolas.
                // Max coin fall speed is the same as the player's max default fall speed (10f).
                /*
                float maxCoinFallSpeed = 10f / Projectile.MaxUpdates;
                if (Projectile.velocity.Y > maxCoinFallSpeed)
                    Projectile.velocity.Y = maxCoinFallSpeed;
                */
            }

            // Coins that are extremely far away from the player begin fading immediately.
            if (Projectile.timeLeft > FadeoutTime && Projectile.Center.Distance(Owner.MountedCenter) > ForceFadeDistance)
                Projectile.timeLeft = FadeoutTime;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            return base.OnTileCollide(oldVelocity);
        }

        // Coins are occasionally refunded when these projectiles are destroyed. The exact refund rules are described below.
        public override void Kill(int timeLeft)
        {
            // Do nothing except on the client who owns this Ricoshot Coin.
            if (Projectile.owner != Main.myPlayer)
                return;

            // Potentially spawn the coin that was tossed when the Ricoshot Coin is destroyed.
            // If the coin wasn't shot, it is always refunded.
            bool spawnRefundCoin = !HasBeenShot;

            // If the coin WAS shot, then there is only a certain chance to refund it based on coin type.
            if (!spawnRefundCoin)
            {
                spawnRefundCoin = CoinType switch
                {
                    // Copper coins are never refunded.
                    0f => false,

                    // Silver coins are refunded half of the time if they are the first coin,
                    // and are never refunded if they are a subsequent coin in a chain.
                    1f => Projectile.localAI[0] == 1f ? Main.rand.NextFloat() < 0.5f : false,

                    // Gold coins are always refunded if they are the first coin,
                    // and are never refunded if they are a subsequent coin in a chain.
                    2f => Projectile.localAI[0] == 1f,

                    // If you don't know what's going on, don't refund a coin.
                    _ => false
                };
            }

            // Actually grant the coin refund. All refunded coins are given an incredibly high pickup range for convenience.
            if (spawnRefundCoin)
            {
                int itemID = CoinType switch
                {
                    1f => ItemID.SilverCoin,
                    2f => ItemID.GoldCoin,
                    _ => ItemID.CopperCoin,
                };
                int coin = Item.NewItem(Projectile.GetSource_DropAsItem(), Projectile.Center, Vector2.One, itemID, noBroadcast: false);
                Main.item[coin].Calamity().grabRangeMultiplier = UsedCoinGrabRangeMultiplier;

                // Sync this dropped coin in multiplayer.
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, coin, 1f);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            int numFrames = Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                /* X */ (int)CoinType * tex.Width / 3,
                /* Y */ Projectile.frame * tex.Height / numFrames,
                /* W */ tex.Width / 3 - 2,
                /* H */ tex.Height / numFrames - 2
            );

            float fadingOpacity = Math.Clamp(Projectile.timeLeft / (float)FadeoutTime, 0f, 1f);
            Color alphaColor = Projectile.GetAlpha(lightColor) * fadingOpacity;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, frame, alphaColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

            // Shine very sharply peaks around the moment the guaranteed crit starts being available. This is to draw the player's eye.
            // https://www.desmos.com/calculator/ngfg5fc9ds
            int numUpdatesPassed = CoinLifetime - Projectile.timeLeft;
            float x = Math.Clamp((float)numUpdatesPassed / CritDelayTime, 0f, 2f); // interpolant for the crit delay sheen
            float sheenFunction = Math.Min(MathF.Pow(x + 0.1f, 6f), MathF.Pow(x - 2.1f, 6f));
            float sheenOpacity = Math.Clamp(sheenFunction, 0f, 1f);

            // oh BOY another end begin boy
            if (sheenOpacity > 0f)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                sheenAsset ??= ModContent.Request<Texture2D>("CalamityMod/Particles/HalfStar");
                Texture2D shineTex = sheenAsset.Value;

                bloomAsset ??= ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
                Texture2D bloomTex = bloomAsset.Value;

                Vector2 shineScale = new Vector2(1f, 3f - sheenOpacity * 2f);
                Color shineColor = CoinType switch
                {
                    1f => Color.Silver,
                    2f => Color.Goldenrod,
                    _ => Color.DarkOrange,
                };

                Main.EntitySpriteDraw(bloomTex, Projectile.Center - Main.screenPosition, null, shineColor * sheenOpacity * 0.3f, MathHelper.PiOver2, bloomTex.Size() / 2f, shineScale * Projectile.scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(shineTex, Projectile.Center - Main.screenPosition, null, shineColor * sheenOpacity * 0.7f, MathHelper.PiOver2, shineTex.Size() / 2f, shineScale * Projectile.scale, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }

            return false;
        }
    }
}
