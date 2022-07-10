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
    public class DeadeyeBlast : ModProjectile
    {
        internal PrimitiveTrail TrailDrawer;

        public ref float Boosted => ref Projectile.ai[0];
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

            if (Boosted == 0)
            {
                //Check for coins
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.ModProjectile != null && proj.type == ModContent.ProjectileType<DeadeyeCoin>() && proj.active)
                    {
                        if (Collision.CheckAABBvAABBCollision(proj.Hitbox.TopLeft(), proj.Hitbox.Size(), Projectile.Hitbox.TopLeft(), Projectile.Hitbox.Size()))
                        {
                            NPC target = Projectile.Center.ClosestNPCAt(900, false, true);
                            if (target != null)
                            {
                                Projectile.velocity = (Projectile.DirectionTo(target.Center) * 16f);
                                Boosted = 1;
                                Main.player[Projectile.owner].Calamity().GeneralScreenShakePower = 2;
                                Projectile.damage = (int)(Projectile.damage * DeadeyeRevolver.RicochetDamageMult);


                                //ORDERSystem.ORDER();
                            }

                            proj.active = false;
                            if (proj.owner == Main.myPlayer)
                                Item.NewItem(proj.GetSource_DropAsItem(), proj.Center, Vector2.One, ItemID.CopperCoin);

                            SoundEngine.PlaySound(DeadeyeRevolver.BlingHitSound, proj.Center);
                            return;
                        }
                    }
                }
            }
        
            if (DieSoon == 1)
                Projectile.velocity = Vector2.Zero;

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //sfx
            Projectile.velocity = Vector2.Zero;
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            DieSoon = 1f;
            Projectile.timeLeft = Math.Min((Lifetime - Projectile.timeLeft), TrailLenght);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            //sfx
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
