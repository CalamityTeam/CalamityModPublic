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
    public class PlaguebringerSummon : ModProjectile
    {
        public const float auraRange = 960f;
        private int auraCounter = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lil' Plaguebringer");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 0;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (projectile.frameCounter++ > 6f)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 3) //dashing frames are unused
            {
                projectile.frame = 0;
            }

            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

            bool correctMinion = projectile.type == ModContent.ProjectileType<PlaguebringerSummon>();
            player.AddBuff(ModContent.BuffType<PlaguebringerSummonBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.plaguebringerPatronSummon = false;
                }
                if (modPlayer.plaguebringerPatronSummon)
                {
                    projectile.timeLeft = 2;
                }
            }
            if (!modPlayer.plaguebringerPatronSet)
                projectile.Kill();

            projectile.MinionAntiClump();

            int buffType = ModContent.BuffType<Plague>();
            float range = auraRange;
            bool dealDamage = auraCounter++ % 60 == 59;
            int dmg = projectile.damage;
            if (projectile.owner == Main.myPlayer)
            {
                for (int l = 0; l < Main.maxNPCs; l++)
                {
                    NPC npc = Main.npc[l];
                    if (npc.active && !npc.friendly && npc.damage > 0 && !npc.dontTakeDamage && !npc.buffImmune[buffType] && Vector2.Distance(projectile.Center, npc.Center) <= range)
                    {
                        if (npc.FindBuffIndex(buffType) == -1)
                        {
                            npc.AddBuff(buffType, 120, false);
                        }
                        if (dealDamage)
                        {
                            Projectile aura = Projectile.NewProjectileDirect(npc.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), dmg, 0f, projectile.owner, l);
                            if (aura.whoAmI.WithinBounds(Main.maxProjectiles))
                                aura.Calamity().forceMinion = true;
                        }
                    }
                }
            }

            float passiveMvtFloat = 0.5f;
            projectile.tileCollide = false;
            float safeDist = 100f;
            Vector2 projPos = new Vector2(projectile.Center.X, projectile.Center.Y);
            float xDist = player.Center.X - projPos.X;
            float yDist = player.Center.Y - projPos.Y;
            yDist += Main.rand.NextFloat(-10f, 20f);
            xDist += Main.rand.NextFloat(-10f, 20f);
            yDist -= 70f;
            Vector2 playerVector = new Vector2(xDist, yDist);
            float playerDist = playerVector.Length();
            float returnSpeed = 18f;

            //If player is close enough, resume normal
            if (playerDist < safeDist && player.velocity.Y == 0f &&
                projectile.position.Y + projectile.height <= player.position.Y + player.height &&
                !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                if (projectile.velocity.Y < -6f)
                {
                    projectile.velocity.Y = -6f;
                }
            }

            //Teleport to player if too far
            if (playerDist > 2000f)
            {
                projectile.position.X = player.Center.X - projectile.width / 2;
                projectile.position.Y = player.Center.Y - projectile.height / 2;
                projectile.netUpdate = true;
            }

            if (playerDist < 50f)
            {
                if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
                {
                    projectile.velocity *= 0.99f;
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
            if (projectile.velocity.X < playerVector.X)
            {
                projectile.velocity.X += passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && projectile.velocity.X < 0f)
                {
                    projectile.velocity.X += passiveMvtFloat;
                }
            }
            if (projectile.velocity.X > playerVector.X)
            {
                projectile.velocity.X -= passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && projectile.velocity.X > 0f)
                {
                    projectile.velocity.X -= passiveMvtFloat;
                }
            }
            if (projectile.velocity.Y < playerVector.Y)
            {
                projectile.velocity.Y += passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y += passiveMvtFloat * 2f;
                }
            }
            if (projectile.velocity.Y > playerVector.Y)
            {
                projectile.velocity.Y -= passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y -= passiveMvtFloat * 2f;
                }
            }
            if (projectile.velocity.X >= 0.25f)
            {
                projectile.direction = -1;
            }
            else if (projectile.velocity.X < -0.25f)
            {
                projectile.direction = 1;
            }
            //Tilting and change directions
            projectile.spriteDirection = projectile.direction;
            projectile.rotation = projectile.velocity.X * 0.01f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool CanDamage() => false;
    }
}
