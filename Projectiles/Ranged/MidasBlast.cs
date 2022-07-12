using System;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using System.Collections.Generic;
using System.IO;

namespace CalamityMod.Projectiles.Ranged
{
    public class MidasBlast : ModProjectile
    {
        internal PrimitiveTrail TrailDrawer;

        public static int Pause = 6 * 6;
        public static int CoinPause = 3 * 30;

        public ref float Ricochets => ref Projectile.ai[0];
        public ref float PauseTime => ref Projectile.ai[1];

        internal List<Vector2> cachedPos;
        public Vector2 StoredDirection = Vector2.Zero;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 4000;
        }

        public static int Lifetime = 600;
        public static int TrailLenght = 35;

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.extraUpdates = 7;
            Projectile.alpha = 255;



            Pause = 4 * 6;
        }

        public override bool? CanDamage() => Projectile.numHits == 0;
        public override bool ShouldUpdatePosition() => PauseTime <= 0;

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (Color.Gold * 0.8f).ToVector3() * 0.5f);
            if (ShouldUpdatePosition())
                UpdateCache();

            if (Ricochets < 4)
            {
                int[] flyingCoins = new int[4];
                int coinIndex = 0;
                int struckCoinIndex = -1;

                //Check for coins
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.ModProjectile != null && proj.type == ModContent.ProjectileType<MidasCoin>() && proj.active && proj.owner == Projectile.owner)
                    {
                        flyingCoins[coinIndex] = proj.whoAmI;
                        coinIndex++;

                        if (Collision.CheckAABBvAABBCollision(proj.Hitbox.TopLeft(), proj.Hitbox.Size(), Projectile.Hitbox.TopLeft(), Projectile.Hitbox.Size()))
                        {
                            struckCoinIndex = proj.whoAmI;
                            coinIndex--; // We don't care about this coin, don't redirect it to itself
                        }

                        if (coinIndex == 4)
                            break;
                    }
                }

                if (struckCoinIndex >= 0)
                {
                    Projectile struckCoin = Main.projectile[struckCoinIndex];

                    if (Ricochets == 0)
                        PlanFullRicochet(flyingCoins, coinIndex, struckCoin);
                    else
                        SimpleRicochet(flyingCoins, coinIndex, struckCoin);
                }
            }

            PauseTime--;
        }

        public void RicochetEffect(Vector2 target, Projectile struckCoin, bool shouldPause = true)
        {
            //Play sound
            SoundEngine.PlaySound(DeadeyeRevolver.BlingHitSound, struckCoin.Center);
            Projectile.velocity = Projectile.DirectionTo(target) * 16f;
            Ricochets++;
            Main.player[Projectile.owner].Calamity().GeneralScreenShakePower = 5;

            float ripening = MathHelper.Clamp((MidasCoin.Lifetime - struckCoin.timeLeft) / (float)MidasPrime.RipeningTime, 0f, 1f); //Coins take a specific amount of time to "ripen" and give the full damage mult
            float damageMult = MathHelper.Lerp(1f, (struckCoin.ai[0] == 0 ? MidasPrime.SilverRicochetDamageMult : MidasPrime.GoldRicochetDamageMult), Math.Min((float)Math.Pow(ripening, 3f), 1f));
            Projectile.damage = (int)(Projectile.damage * damageMult);

            struckCoin.active = false;
            if (struckCoin.owner == Main.myPlayer)
            {
                int coin = Item.NewItem(struckCoin.GetSource_DropAsItem(), struckCoin.Center, Vector2.One, struckCoin.ai[0] == 0 ? ItemID.SilverCoin : ItemID.GoldCoin);
                Main.item[coin].GetGlobalItem<MidasPrimeItem>().magnetMode = true;
            }

            //Commented out. Could get activated through some funny easter egg thing, maybe the player being named V1/2 
            //ORDERSystem.ORDER();

            if (shouldPause)
                PauseTime = Pause;
        }

        public void SimpleRicochet(int[] flyingCoins, int flyingCoinCount, Projectile struckCoin)
        {
            //If we find a redirection target, do the ricochet and a bunch of cool effects.
            if (FindRicochetTarget(Projectile.Center, flyingCoins, flyingCoinCount, out Vector2 redirectionTarget, out int coinIndex))
                RicochetEffect(redirectionTarget, struckCoin, coinIndex >= 0);
            
            //If no target is found, just ricochet the shot forward
            else
                RicochetEffect(Projectile.Center + Projectile.velocity, struckCoin, false);
        }

        public void PlanFullRicochet(int[] flyingCoins, int flyingCoinCount, Projectile struckCoin)
        {
            Vector2 ricochetPosition = Projectile.Center;
            Vector2 firstRicochetTarget = Vector2.Zero;


            for (int i = 0; i <= flyingCoinCount; i++)
            {
                //Check for a potential ricochet target that isnt paused already (aka we already hit.
                if (FindRicochetTarget(ricochetPosition, flyingCoins, flyingCoinCount, out Vector2 redirectionTarget, out int redirectedCoinIndex, false))
                {
                    if (redirectedCoinIndex >= 0)
                    {
                        if (firstRicochetTarget == Vector2.Zero)
                            firstRicochetTarget = redirectionTarget;

                        Main.projectile[redirectedCoinIndex].ai[1] = CoinPause;
                        ricochetPosition = redirectionTarget;
                    }


                    else
                    {
                        //If we can't ricochet to any coins from the start but we still found a valid npc target, go to them.
                        if (i == 0)
                        {
                            RicochetEffect(redirectionTarget, struckCoin, false);
                            return;
                        }

                        else
                            break;
                    }
                }

                else
                {
                    if (i == 0)
                    {
                        //If no target at all was found, just propel the coin forward
                        RicochetEffect(Projectile.Center + Projectile.velocity, struckCoin, false);
                        return;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            RicochetEffect(firstRicochetTarget, struckCoin);
            return;
        }

        public bool FindRicochetTarget(Vector2 ricochetStart, int[] flyingCoins, int flyingCoinCount, out Vector2 redirectionTarget, out int redirectedCoinIndex, bool ignorePausedCoins = true)
        {
            bool targetFound = false;
            float closestCoinDistance = 1000;
            redirectionTarget = ricochetStart;
            redirectedCoinIndex = -1;

            //Search for a coin to ricochet off of.
            if (Ricochets < 3)
            {
                for (int i = 0; i < flyingCoinCount; i++)
                {
                    Projectile coin = Main.projectile[flyingCoins[i]];
                    if ((ignorePausedCoins || coin.ai[1] <= 0) && Collision.CanHitLine(ricochetStart - Projectile.Size / 2f, Projectile.width, Projectile.height, coin.position, coin.width, coin.height))
                    {
                        float coinDistance = (ricochetStart - coin.Center).Length();
                        if (closestCoinDistance > coinDistance)
                        {
                            targetFound = true;
                            closestCoinDistance = coinDistance;
                            redirectionTarget = coin.Center;
                            redirectedCoinIndex = coin.whoAmI;
                        }
                    }
                }
            }

            //If no coin was found, search for a npc to hit.
            if (!targetFound)
            {
                NPC target = ricochetStart.ClosestNPCAt(900, false, true);
                if (target != null)
                {
                    targetFound = true;
                    redirectionTarget = target.Center;
                }
            }

            if (redirectionTarget != ricochetStart)
                return true;

            return false;
        }



        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            Projectile.timeLeft = Math.Min((Lifetime - Projectile.timeLeft), TrailLenght);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.numHits++;
            target.AddBuff(BuffID.Midas, 60);
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = Math.Min((Lifetime - Projectile.timeLeft), TrailLenght);
        }

        internal Color ColorFunction(float completionRatio)
        {
            float fadeOpacity = Math.Min(Projectile.timeLeft / (float)TrailLenght, 1f);
            return Color.PaleGoldenrod * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio)
        {
            float width = Math.Min(Projectile.timeLeft / (float)TrailLenght, 1f);
            return (1 - completionRatio) * 6.4f * width;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (cachedPos is null)
                return false;

            if (TrailDrawer is null)
                TrailDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, PrimitiveTrail.RigidPointRetreivalFunction, specialShader: GameShaders.Misc["CalamityMod:TrailStreak"]);

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(Request<Texture2D>("CalamityMod/ExtraTextures/BasicTrail"));
            TrailDrawer.Draw(cachedPos, Projectile.Size * 0.5f - Main.screenPosition, TrailLenght);

            return false;
        }

        //Done like that for better control instead of relying on Projectile.OldPos()
        private void UpdateCache()
        {
            if (cachedPos == null)
            {
                cachedPos = new List<Vector2>();

                for (int i = 0; i < TrailLenght; i++)
                {
                    cachedPos.Add(Projectile.Center);
                }
            }

            cachedPos.Insert(0, Projectile.Center);

            while (cachedPos.Count > TrailLenght)
            {
                cachedPos.RemoveAt(cachedPos.Count - 1);
            }
        }
    }
}
