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

namespace CalamityMod.Projectiles.Ranged
{
    public class MidasBlast : ModProjectile
    {
        internal PrimitiveTrail TrailDrawer;

        public ref float Ricochets => ref Projectile.ai[0];
        public ref float DieSoon => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = TrailLenght;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
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
        }

        public override bool? CanDamage() => Projectile.numHits == 0;

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (Color.Gold * 0.8f).ToVector3() * 0.5f);

            if (Ricochets < 4)
            {
                int[] flyingCoins = new int[3];
                int coinIndex = 0;
                int struckCoinIndex = -1;

                //Check for coins
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.ModProjectile != null && proj.type == ModContent.ProjectileType<MidasCoin>() && proj.active)
                    {
                        flyingCoins[coinIndex] = proj.whoAmI;
                        coinIndex++;

                        if (Collision.CheckAABBvAABBCollision(proj.Hitbox.TopLeft(), proj.Hitbox.Size(), Projectile.Hitbox.TopLeft(), Projectile.Hitbox.Size()))
                        {
                            struckCoinIndex = proj.whoAmI;
                            coinIndex--; // We don't care about this coin, don't redirect it to itself
                        }

                        if (coinIndex == 3)
                            break;
                    }
                }

                if (struckCoinIndex >= 0)
                {
                    bool ultrakilled = false;
                    float closestCoinDistance = 1000;
                    Projectile struckCoin = Main.projectile[struckCoinIndex];
                    Vector2 redirectionTarget = struckCoin.Center;

                    if (Ricochets < 3)
                    {
                        for (int i = 0; i < coinIndex; i++)
                        {
                            Projectile coin = Main.projectile[flyingCoins[i]];
                            if (Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, coin.position, coin.width, coin.height))
                            {
                                float coinDistance = (Projectile.Center - coin.Center).Length();
                                if (closestCoinDistance > coinDistance)
                                {
                                    ultrakilled = true;
                                    closestCoinDistance = coinDistance;
                                    redirectionTarget = coin.Center;
                                }
                            }
                        }
                    }

                    if (!ultrakilled)
                    {
                        NPC target = Projectile.Center.ClosestNPCAt(900, false, true);
                        if (target != null)
                        {
                            ultrakilled = true;
                            redirectionTarget = target.Center;
                        }
                    }


                    //If we managed to redirect the bullet towards another coin or an enemy, do the cool effects
                    if (ultrakilled)
                    {
                        SoundEngine.PlaySound(DeadeyeRevolver.BlingHitSound, struckCoin.Center);
                        Projectile.velocity = Projectile.DirectionTo(redirectionTarget) * 16f;
                        Ricochets++;
                        Main.player[Projectile.owner].Calamity().GeneralScreenShakePower = 5;

                        float ripening = MathHelper.Clamp((MidasCoin.Lifetime - struckCoin.timeLeft) / (float)MidasPrime.RipeningTime, 0f, 1f); //Coins take a specific amount of time to "ripen" and give the full damage mult
                        float damageMult = MathHelper.Lerp(1f, (struckCoin.ai[0] == 0 ? MidasPrime.SilverRicochetDamageMult : MidasPrime.GoldRicochetDamageMult), Math.Min((float)Math.Pow(ripening, 3f), 1f));
                        Projectile.damage = (int)(Projectile.damage * damageMult);

                        struckCoin.active = false;
                        if (struckCoin.owner == Main.myPlayer)
                            Item.NewItem(struckCoin.GetSource_DropAsItem(), struckCoin.Center, Vector2.One, struckCoin.ai[0] == 0 ? ItemID.SilverCoin : ItemID.GoldCoin);

                        //ORDERSystem.ORDER();
                    }
                }
            }
        
            if (DieSoon == 1)
                Projectile.velocity = Vector2.Zero;

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            
            Projectile.velocity = Vector2.Zero;
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            DieSoon = 1f;
            Projectile.timeLeft = Math.Min((Lifetime - Projectile.timeLeft), TrailLenght);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Midas, 60);
            DieSoon = 1f;
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
            if (TrailDrawer is null)
                TrailDrawer = new PrimitiveTrail(WidthFunction, ColorFunction, PrimitiveTrail.RigidPointRetreivalFunction, specialShader: GameShaders.Misc["CalamityMod:TrailStreak"]);

            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(Request<Texture2D>("CalamityMod/ExtraTextures/BasicTrail"));
            TrailDrawer.Draw(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, TrailLenght);

            return false;
        }
    }
}
