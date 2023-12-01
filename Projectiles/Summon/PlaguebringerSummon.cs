using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PlaguebringerSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player Owner = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = Owner.Calamity();

            if (Projectile.frameCounter++ >= 6f)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 3) //dashing frames are unused
            {
                Projectile.frame = 0;
            }

            bool correctMinion = Projectile.type == ModContent.ProjectileType<PlaguebringerSummon>();
            Owner.AddBuff(ModContent.BuffType<LilPlaguebringerBuff>(), 3600);
            if (correctMinion)
            {
                if (Owner.dead)
                {
                    modPlayer.plaguebringerPatronSummon = false;
                }
                if (modPlayer.plaguebringerPatronSummon)
                {
                    Projectile.timeLeft = 2;
                }
            }
            if (!modPlayer.plaguebringerPatronSet)
                Projectile.Kill();

            Projectile.MinionAntiClump();

            Projectile.ai[0]++;
            NPC Target = Projectile.Center.MinionHoming(800f, Owner, false);
            if (Projectile.owner == Main.myPlayer && Target != null)
            {
                if (Projectile.ai[0] % 12 == 11)
                {
                    int beeCount = Main.rand.Next(1, 3);
                    if (Owner.strongBees && Main.rand.NextBool(3))
                        ++beeCount;

                    for (int i = 0; i < beeCount; i++)
                    {
                        int beeType = Main.rand.NextBool(4) ? ModContent.ProjectileType<PlagueBeeSmall>() : Owner.beeType(); // 25% chance for plague bee, otherwise depends if Hive Pack or not
                        Projectile bee = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(0.25f, 0.25f), beeType, Projectile.damage, Owner.beeKB(0f), Projectile.owner);
                        bee.usesLocalNPCImmunity = true;
                        bee.localNPCHitCooldown = 10;
                        bee.penetrate = 2;
                        bee.DamageType = DamageClass.Generic;
                    }
                }
            }

            float passiveMvtFloat = 0.5f;
            float safeDist = 100f;
            float xDist = Owner.Center.X - Projectile.Center.X;
            float yDist = Owner.Center.Y - Projectile.Center.Y;
            yDist += Main.rand.NextFloat(-10f, 20f);
            xDist += Main.rand.NextFloat(-10f, 20f);
            yDist -= 70f;
            Vector2 playerVector = new Vector2(xDist, yDist);
            float playerDist = playerVector.Length();
            float returnSpeed = 18f;

            //If player is close enough, resume normal
            if (playerDist < safeDist && Owner.velocity.Y == 0f &&
                Projectile.position.Y + Projectile.height <= Owner.position.Y + Owner.height &&
                !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                if (Projectile.velocity.Y < -6f)
                {
                    Projectile.velocity.Y = -6f;
                }
            }

            //Teleport to player if too far
            if (playerDist > 2000f)
            {
                Projectile.position.X = Owner.Center.X - Projectile.width / 2;
                Projectile.position.Y = Owner.Center.Y - Projectile.height / 2;
                Projectile.netUpdate = true;
            }

            if (playerDist < 50f)
            {
                if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                {
                    Projectile.velocity *= 0.99f;
                }
                passiveMvtFloat = 0.01f;
            }
            else
            {
                if (playerDist < 100f)
                {
                    passiveMvtFloat = 0.1f;
                }
                if (playerDist > 300f)
                {
                    passiveMvtFloat = 1f;
                }
                playerDist = returnSpeed / playerDist;
                playerVector.X *= playerDist;
                playerVector.Y *= playerDist;
            }
            if (Projectile.velocity.X < playerVector.X)
            {
                Projectile.velocity.X += passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && Projectile.velocity.X < 0f)
                {
                    Projectile.velocity.X += passiveMvtFloat;
                }
            }
            if (Projectile.velocity.X > playerVector.X)
            {
                Projectile.velocity.X -= passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && Projectile.velocity.X > 0f)
                {
                    Projectile.velocity.X -= passiveMvtFloat;
                }
            }
            if (Projectile.velocity.Y < playerVector.Y)
            {
                Projectile.velocity.Y += passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && Projectile.velocity.Y < 0f)
                {
                    Projectile.velocity.Y += passiveMvtFloat * 2f;
                }
            }
            if (Projectile.velocity.Y > playerVector.Y)
            {
                Projectile.velocity.Y -= passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y -= passiveMvtFloat * 2f;
                }
            }
            if (Projectile.velocity.X >= 0.25f)
            {
                Projectile.direction = -1;
            }
            else if (Projectile.velocity.X < -0.25f)
            {
                Projectile.direction = 1;
            }
            //Tilting and change directions
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.X * 0.01f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool? CanDamage() => false;
    }
}
