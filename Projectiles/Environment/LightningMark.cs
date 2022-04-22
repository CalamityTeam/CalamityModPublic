using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Environment
{
    public class LightningMark : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Mark");
        }

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 12;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1800;
            Projectile.extraUpdates = 10;
        }

        public override void AI()
        {
            if (Projectile.timeLeft <= 600)
            {
                if (Projectile.ai[0] == 0f && Projectile.owner == Main.myPlayer)
                {
                    Projectile.ai[0] = 1f;

                    Vector2 fireFrom = new Vector2(Projectile.Center.X, Projectile.Center.Y - 900f);
                    int tries = 0;
                    while (CalamityUtils.ParanoidTileRetrieval((int)(fireFrom.X / 16f), (int)(fireFrom.Y / 16f)).HasTile)
                    {
                        fireFrom.Y += 16f;

                        tries += 1;
                        if (tries > 25)
                            return;
                    }

                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/LightningStrike"), Projectile.Center);
                    Vector2 ai0 = Projectile.Center - fireFrom;
                    float ai = Main.rand.Next(100);
                    Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;

                    int damage = NPC.downedMoonlord ? 80 : NPC.downedPlantBoss ? 40 : Main.hardMode ? 20 : 10;

                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), fireFrom, velocity, ProjectileID.CultistBossLightningOrbArc, damage, 0f, Projectile.owner, ai0.ToRotation(), ai);
                    if (proj.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[proj].extraUpdates += 11;
                        Main.projectile[proj].friendly = true;
                        Main.projectile[proj].Calamity().lineColor = 1;
                    }
                }
            }
            else if (Projectile.velocity.Y == 0f)
            {
                Lighting.AddLight(Projectile.Center, 0.6f, 0.9f, 1f);

                if (Main.rand.NextBool(3))
                {
                    int num199 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 226, 0f, 0f, 100, default, 1f);
                    Main.dust[num199].position.X -= 2f;
                    Main.dust[num199].position.Y += 2f;
                    Main.dust[num199].scale = 0.7f;
                    Main.dust[num199].noGravity = true;
                    Main.dust[num199].velocity.Y -= 2f;
                }

                if (Main.rand.NextBool(15))
                {
                    int num200 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 31, 0f, 0f, 100, default, 1f);
                    Main.dust[num200].position.X -= 2f;
                    Main.dust[num200].position.Y += 2f;
                    Main.dust[num200].scale = 0.7f;
                    Main.dust[num200].noGravity = true;
                    Main.dust[num200].velocity *= 0.1f;
                }
            }

            if (Projectile.velocity.Y != Projectile.velocity.Y && Projectile.velocity.Y > 1f)
                Projectile.velocity.Y *= -0.5f;

            Projectile.velocity.Y += 0.2f;

            if (Projectile.velocity.Y > 16f)
                Projectile.velocity.Y = 16f;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
