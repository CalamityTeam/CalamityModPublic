using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.Ravager;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Magic;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Humanizer.In;

namespace CalamityMod.Projectiles.Rogue
{
    public class ScourgeoftheDesertProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ScourgeoftheDesert";

        public int Time = 0;
        public int TimeUnderground = 0;
        public bool PostExitTiles = false;
        public bool InitialTileHit = false;
        public bool InsideTiles = false;
        public Vector2 SavedOldVelocity;
        public Vector2 NPCDestination;
        public bool SetPierce = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
        }
        public override void AI()
        {
            if (!SetPierce)
            {
                Projectile.penetrate = Projectile.Calamity().stealthStrike ? 4 : 2;
                SetPierce = true;
            }
            InsideTiles = Collision.SolidCollision(Projectile.Center, 50, 50);
            Time++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Player Owner = Main.player[Projectile.owner];
            float playerDist = Vector2.Distance(Owner.Center, Projectile.Center);

            if (Main.rand.NextBool(2))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(20, 20), Main.rand.NextBool() ? 288 : 207);
                dust.scale = Main.rand.NextFloat(0.3f, 0.55f);
                dust.noGravity = true;
                dust.velocity = -Projectile.velocity * 0.5f;
            }
            
            if (!InitialTileHit && Time > 45)
            {
                if (Projectile.velocity.Y < 0)
                {
                    Projectile.velocity.Y *= 0.95f;
                }
                Projectile.velocity.Y += 0.15f;
                Projectile.velocity.X *= 0.98f;
            }

            if (InitialTileHit && !InsideTiles && !PostExitTiles) // Emerge from ground
            {
                Projectile.extraUpdates = 4;
                if (!Projectile.Calamity().stealthStrike)
                    Projectile.timeLeft = 200;
                SoundEngine.PlaySound(SoundID.NPCHit11 with { Volume = 1.3f, Pitch = 1.1f }, Projectile.Center);
                for (int i = 0; i <= 25; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * 6.5f, Main.rand.NextBool() ? 207 : 216, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(0.3f, 0.9f), 0, default, Main.rand.NextFloat(1.3f, 1.8f));
                    dust.noGravity = true;
                }
                PostExitTiles = true;
            }
            if (PostExitTiles)
            {
                if (Projectile.timeLeft % 2 == 0 && playerDist < 1400f)
                {
                    SparkParticle spark = new SparkParticle(Projectile.Center - Projectile.velocity * 8f, -Projectile.velocity * 0.1f, false, 9, 2.3f, Color.White * 0.1f);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            if (InsideTiles)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].CanBeChasedBy(Projectile.GetSource_FromThis(), false))
                        NPCDestination = Main.npc[i].Center + Main.npc[i].velocity * 5f;
                }

                TimeUnderground++;
                Vector3 DustLight = new Vector3(0.171f, 0.124f, 0.086f);
                Lighting.AddLight(Projectile.Center + Projectile.velocity, DustLight * 4);
                if (Time % 15 == 0 && TimeUnderground < 120)
                {
                    SoundEngine.PlaySound(SoundID.WormDig with { Volume = 0.7f, Pitch = 0.2f }, Projectile.Center);
                }
                float returnSpeed = 10;
                float acceleration = 0.2f;
                float xDist = NPCDestination.X - Projectile.Center.X;
                float yDist = NPCDestination.Y - Projectile.Center.Y;
                float dist = (float)Math.Sqrt(xDist * xDist + yDist * yDist);
                dist = returnSpeed / dist;
                xDist *= dist;
                yDist *= dist;
                float targetDist = Vector2.Distance(NPCDestination, Projectile.Center);
                if (targetDist < 1800 && TimeUnderground > 25)
                {
                    if (Projectile.velocity.X < xDist)
                    {
                        Projectile.velocity.X = Projectile.velocity.X + acceleration;
                        if (Projectile.velocity.X < 0f && xDist > 0f)
                            Projectile.velocity.X += acceleration;
                    }
                    else if (Projectile.velocity.X > xDist)
                    {
                        Projectile.velocity.X = Projectile.velocity.X - acceleration;
                        if (Projectile.velocity.X > 0f && xDist < 0f)
                            Projectile.velocity.X -= acceleration;
                    }
                    if (Projectile.velocity.Y < yDist)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + acceleration;
                        if (Projectile.velocity.Y < 0f && yDist > 0f)
                            Projectile.velocity.Y += acceleration;
                    }
                    else if (Projectile.velocity.Y > yDist)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - acceleration;
                        if (Projectile.velocity.Y > 0f && yDist < 0f)
                            Projectile.velocity.Y -= acceleration;
                    }
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            SavedOldVelocity = oldVelocity;
            Projectile.tileCollide = false;
            if (!InitialTileHit) // Enter ground
            {
                for (int i = 0; i <= 25; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 3, Main.rand.NextBool() ? 207 : 216, -Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(0.3f, 0.9f), 0, default, Main.rand.NextFloat(1.3f, 1.8f));
                    dust.noGravity = true;
                }
                Projectile.velocity = oldVelocity * 0.7f;
                SoundEngine.PlaySound(SoundID.WormDig with { Volume = 1.5f, Pitch = 1.1f}, Projectile.Center);
                InitialTileHit = true;
            }
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i <= 5; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 32 : 216, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.1f, 0.6f), 0, default, Main.rand.NextFloat(0.4f, 0.8f));
                dust.noGravity = false;
            }
            if (Projectile.Calamity().stealthStrike)
            {
                Projectile.timeLeft = 600;
                Time = 10;
                TimeUnderground = 0;
                PostExitTiles = false;
                InitialTileHit = false;
                InsideTiles = false;
                Projectile.tileCollide = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i <= 25; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity, Main.rand.NextBool(3) ? 216 : 207, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.05f, 2.2f), 0, default, Main.rand.NextFloat(1.5f, 2.8f));
                dust.noGravity = true;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
