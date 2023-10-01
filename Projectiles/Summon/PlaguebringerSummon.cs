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
        public const float auraRange = 960f;
        private int auraCounter = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (Projectile.frameCounter++ > 6f)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 3) //dashing frames are unused
            {
                Projectile.frame = 0;
            }

            bool correctMinion = Projectile.type == ModContent.ProjectileType<PlaguebringerSummon>();
            player.AddBuff(ModContent.BuffType<LilPlaguebringerBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
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

            int buffType = ModContent.BuffType<Plague>();
            float range = auraRange;
            bool dealDamage = auraCounter++ % 60 == 59;
            int dmg = Projectile.damage;
            if (Projectile.owner == Main.myPlayer)
            {
                for (int l = 0; l < Main.maxNPCs; l++)
                {
                    NPC npc = Main.npc[l];
                    if (npc.IsAnEnemy() && !npc.dontTakeDamage && !npc.buffImmune[buffType] && Vector2.Distance(Projectile.Center, npc.Center) <= range)
                    {
                        if (npc.FindBuffIndex(buffType) == -1)
                        {
                            npc.AddBuff(buffType, 120, false);
                        }
                        if (dealDamage)
                        {
                            Projectile aura = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), npc.Center, Vector2.Zero, ModContent.ProjectileType<DirectStrike>(), dmg, 0f, Projectile.owner, l);
                            if (aura.whoAmI.WithinBounds(Main.maxProjectiles))
                                aura.DamageType = DamageClass.Summon;
                        }
                    }
                }
            }

            float passiveMvtFloat = 0.5f;
            Projectile.tileCollide = false;
            float safeDist = 100f;
            Vector2 projPos = new Vector2(Projectile.Center.X, Projectile.Center.Y);
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
                Projectile.position.Y + Projectile.height <= player.position.Y + player.height &&
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
                Projectile.position.X = player.Center.X - Projectile.width / 2;
                Projectile.position.Y = player.Center.Y - Projectile.height / 2;
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
