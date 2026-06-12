using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class MirrorBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public bool isShard = true;

        public int shardShield = 0;

        public bool isShield => shardShield > 0;

        private bool hasSpawned = false;

        public int shardNum = -1;

        private Player player => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1800;
            Projectile.extraUpdates = 0;
            Projectile.noEnchantmentVisuals = true;
            Projectile.tileCollide = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(isShard);
            writer.Write(hasSpawned);
            writer.Write(shardNum);
            writer.Write(shardShield);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            isShard = reader.ReadBoolean();
            hasSpawned = reader.ReadBoolean();
            shardNum = reader.ReadInt32();
            shardShield = reader.ReadInt32();
        }
        public override bool? CanDamage()
        {
            return !isShard;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (target.Calamity().DR > 0.9f) {
                return false;
            }
            return base.CanHitNPC(target);
        }

        void SpawnLogic()
        {
            Projectile.netUpdate = true;
            //This is used when spawned by Evolution.
            //Set here instead of on spawn so it all syncs in MP
            if (Projectile.ai[0] != 0)
            {
                //Setting AP so high guarantees we get the full dmg of the projectile
                //We also want to guarantee the proj doesn't crit ever
                Projectile.ArmorPenetration = 1000;
                Projectile.DamageType = DamageClass.Generic;
                Projectile.CritChance = 0;

                //When not holding Mirror Blade, shards launch instantly
                if (Main.player[Projectile.owner].HeldItem.type != ModContent.ItemType<MirrorBlade>())
                {
                    shardShield = 10;
                    hasSpawned = true;
                    shardShield = 0;
                    isShard = false;
                    Projectile.timeLeft = 1200;
                    Projectile.velocity = Projectile.DirectionTo(player.Center) * -20f;
                    return;
                }
                //When holding Mirror Blade, the shards orbit the player like the blade shards.
                //Evoultion shards insert themselves at the *end* of the shard list. This means they will launch themselves before any Mirror Blade shards are launched
                //This is done because Evolution shards don't get the 2/3x dmg from shattering the mirror shield, so they're less valuable to be stored.
                else
                {

                    shardNum = player.ownedProjectileCounts[Projectile.type];
                    hasSpawned = true;
                    shardNum = 0;
                    foreach (var proj in Main.projectile)
                    {
                        if (proj.active && proj.type == ModContent.ProjectileType<MirrorBlast>() && proj.owner == Projectile.owner && proj.ModProjectile<MirrorBlast>().shardNum > -1)
                        {
                            shardNum++;
                        }
                    }
                    return;
                }
            }

            //If not from The Evolution, run normal spawn logic
            shardNum = player.ownedProjectileCounts[Projectile.type];
            hasSpawned = true;
            shardNum = 0;
            foreach (var proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<MirrorBlast>() && proj.owner == Projectile.owner)
                {
                    (proj.ModProjectile as MirrorBlast).shardNum++;
                    proj.netUpdate = true;
                }
            }
        }
        public override void AI()
        {
            player.Calamity().mouseWorldListener = true;
            if (!hasSpawned)
                SpawnLogic();
            if (isShield)
            {
                if (shardNum > 10 || Projectile.timeLeft < 2)
                {
                    shardShield = 0;
                    isShard = false;
                    Projectile.timeLeft = 1200;
                    Projectile.velocity = Projectile.DirectionTo(player.Center) * -20f;
                    Projectile.netUpdate = true;
                    return;
                }
                List<Vector2> positions = new List<Vector2>() //Hardcoded positions for the mirror shield shards so we can make it look nice
                {
                    new(0,75),
                    new(10,65),
                    new(-10,65),
                    new(20,75),
                    new(-20,75),
                    new(30,65),
                    new(-30,65),
                    new(14,55),
                    new(-14,55),
                    new(0,45)
                };
                Projectile.velocity = Vector2.Zero;
                Projectile.Center = Vector2.Lerp(Projectile.Center,
                player.Center + player.DirectionTo(player.Calamity().mouseWorld)
                    .RotatedBy(MathHelper.ToRadians(positions[shardNum - 1].X))
                    * positions[shardNum - 1].Y,
                0.5f);
                Projectile.rotation = Projectile.DirectionTo(player.Center).ToRotation() + MathHelper.PiOver2;
                shardShield--;
            }
            else if (isShard)
            {
                Projectile.velocity = Vector2.Zero;
                var shardCount = 0;
                foreach (var proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<MirrorBlast>() && proj.owner == Projectile.owner && (proj.ModProjectile as MirrorBlast).isShard)
                    {
                        shardCount++;
                    }
                }
                Projectile.Center = Vector2.Lerp(Projectile.Center, player.Center + new Vector2(0, MathHelper.Lerp(-90, -110, MathF.Sin(player.miscCounter / 300f * MathHelper.TwoPi + MathHelper.Pi * (shardNum % 2)))).RotatedBy(MathHelper.ToRadians(player.miscCounter / 300f * 360f + 360f / shardCount * shardNum)), 0.15f);
                Projectile.rotation = Projectile.DirectionTo(player.Center).ToRotation() + MathHelper.PiOver2;
                if (shardNum > 10 || Projectile.timeLeft < 2)
                {
                    isShard = false;
                    Projectile.timeLeft = 1200;
                    Projectile.velocity = Projectile.DirectionTo(player.Center) * -20f;
                }
            }
            else
            {
                float homingStrength = 0.025f; // Adjust this value for stronger or weaker homing
                if (Projectile.timeLeft < 1000)
                    homingStrength *= 2f;
                if (Projectile.timeLeft < 800)
                    homingStrength *= 2f;
                if (Projectile.timeLeft < 600)
                    homingStrength *= 2f;
                NPC target = FindClosestNPC(3200f);
                if (target != null)
                {
                    Vector2 direction = target.Center - Projectile.Center;
                    direction.Normalize();
                    direction *= 40f; // Adjust speed as needed
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction, homingStrength);

                    Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * Main.rand.NextFloat(-0.2f, -0.6f), Color.Black, 7, Main.rand.NextFloat(0.35f, 0.4f), 1f, Main.rand.NextFloat(-0.2f, 0.2f), false);
                    GeneralParticleHandler.SpawnParticle(smoke);
                    if (Main.rand.NextBool(5))
                    {
                        Dust trailDust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(5, 5) - Projectile.velocity, DustID.RainbowTorch);
                        trailDust.scale = Main.rand.NextFloat(0.7f, 0.85f);
                        trailDust.velocity = -Projectile.velocity * Main.rand.NextFloat(0.3f, 0.5f);
                        trailDust.color = Main.rand.NextBool() ? Color.AliceBlue : Color.SkyBlue;
                        trailDust.noGravity = true;
                    }
                }
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            Lighting.AddLight(Projectile.Center, 0.96f*0.33f, 0.91f*0.33f, 0.33f);
            if (Projectile.FinalExtraUpdate())
                Projectile.frameCounter++;
            if (Projectile.frameCounter > 8)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 3)
                Projectile.frame = 0;
        }

        public override void OnKill(int timeLeft)
        {
            if (!isShard)
            {
                SoundEngine.PlaySound(SoundID.Item27 with {Volume = 0.5f }, Projectile.position);
                Particle Star = new CritSpark(Projectile.Center, Vector2.Zero, Color.WhiteSmoke, Color.BlueViolet, Main.rand.NextFloat(1.5f, 1.6f), 10, 0.1f, 3f);
                GeneralParticleHandler.SpawnParticle(Star);

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(Projectile.Center - Projectile.velocity / 2f, 0, 0, DustID.GemDiamond, 0f, 0f, 100, default, 1f);
                    Main.dust[dust].velocity *= 2f;
                    Main.dust[dust].noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D BlastTex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/MirrorBlast").Value;
            Texture2D ShardTex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/MirrorShard").Value;
            Point BlastTextureDim = new Point(60, 26);
            Point ShardTextureDim = new Point(32, 14);
            Texture2D UsedTex = isShard ? ShardTex : BlastTex;
            Point UsedTextureDim = isShard ? ShardTextureDim : BlastTextureDim;
            Vector2 origin = isShard ? ShardTextureDim.ToVector2() / 2f : BlastTextureDim.ToVector2() / 2f + new Vector2(14, 0);
            Main.spriteBatch.Draw(UsedTex, Projectile.Center - Main.screenPosition, new Rectangle(0, UsedTextureDim.Y * Projectile.frame, UsedTextureDim.X, UsedTextureDim.Y), Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        private NPC FindClosestNPC(float maxRange)
        {
            NPC closestNPC = null;
            float closestDistance = maxRange;

            foreach (NPC npc in Main.npc)
            {
                if (npc.CanBeChasedBy(this) && !npc.friendly)
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPC = npc;
                    }
                }
            }

            return closestNPC;
        }
    }
}
