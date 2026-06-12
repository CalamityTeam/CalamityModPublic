using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class CloudElementalMinion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public int dust = 3;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 116;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.cloudElemental && !modPlayer.allElementals && !modPlayer.cloudElementalVanity && !modPlayer.allElementalsVanity)
            {
                Projectile.active = false;
                return;
            }
            bool correctMinion = Projectile.type == ModContent.ProjectileType<CloudElementalMinion>();
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.cloudEleBuff = false;
                }
                if (modPlayer.cloudEleBuff)
                {
                    Projectile.timeLeft = 2;
                }
            }
            dust--;
            if (dust >= 0)
            {
                int dustAmt = 50;
                for (int d = 0; d < dustAmt; d++)
                {
                    int index = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y + 16f), Projectile.width, Projectile.height - 16, DustID.Cloud, 0f, 0f, 0, default, 1f);
                    Main.dust[index].velocity *= 2f;
                    Main.dust[index].scale *= 1.15f;
                }
            }
            if (Math.Abs(Projectile.velocity.X) > 0.2f)
            {
                Projectile.spriteDirection = -Projectile.direction;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 7)
            {
                Projectile.frame = 0;
            }

            if (!modPlayer.cloudElementalVanity && !modPlayer.allElementalsVanity)
            {
                float lightScalar = (float)Main.rand.Next(90, 111) * 0.01f;
                lightScalar *= Main.essScale;
                Lighting.AddLight(Projectile.Center, 0.25f * lightScalar, 0.55f * lightScalar, 0.75f * lightScalar);
                Projectile.ChargingMinionAI(500f, 800f, 1200f, 400f, 0, 30f, 8f, 4f, new Vector2(500f, -60f), 40f, 8f, false, true, 1);
            }
            else
            {
                //Ignore tiles so she doesn't get stuck like Optic Staff
                Projectile.tileCollide = false;

                Projectile.ai[0] = 1f;

                //Player distance calculations
                Vector2 playerVec = player.Center - Projectile.Center + new Vector2(500f, -60f);
                float playerDist = playerVec.Length();

                //If the minion is actively returning, move faster
                float playerHomeSpeed = 6f;
                if (Projectile.ai[0] == 1f)
                {
                    playerHomeSpeed = 15f;
                }
                //Move somewhat faster if the player is kinda far~ish
                if (playerDist > 200f && playerHomeSpeed < 8f)
                {
                    playerHomeSpeed = 8f;
                }
                //Return to normal if close enough to the player
                if (playerDist < 400f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                {
                    Projectile.ai[0] = 0f;
                    Projectile.netUpdate = true;
                }
                //Teleport to the player if abnormally far
                if (playerDist > 2000f)
                {
                    Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                    Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                    Projectile.netUpdate = true;
                }
                //If more than 70 pixels away, move toward the player
                if (playerDist > 70f)
                {
                    playerVec.Normalize();
                    playerVec *= playerHomeSpeed;
                    Projectile.velocity = (Projectile.velocity * 40f + playerVec) / 41f;
                }
                //Minions never stay still
                else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
                {
                    Projectile.velocity.X = -0.15f;
                    Projectile.velocity.Y = -0.05f;
                }
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<WindChilled>(), 180);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<WindChilled>(), 180);
        }

        public override bool? CanDamage()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.cloudElementalVanity || modPlayer.allElementalsVanity)
            {
                return false;
            }
            else
            {
                return null;
            }
        }
    }
}
