using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Melee;

namespace CalamityMod.NPCs.AcidRain
{
    public class NuclearTerror : ModNPC
    {
        public int AttackIndex = 0;
        public int DelayTime = 0;
        public bool Dying = false;
        public bool Walking = false;
        public float JumpTimer = 0f;
        public Vector2 ShootPosition;
        public static readonly int[] PhaseArray = new int[]
        {
            2, 0, 1, 1, 2, 1, 0, 2, 1, 1, 0, 1, 2, 1, 0, 2, 1
        };
        public const int AttackCycleTime = 520;
        public const int SpecialAttackTime = 240;
        public const float TeleportTime = 60f;
        public const float TeleportFadeinTime = 10f;
        public const float TeleportCooldown = 60f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nuclear Terror");
            Main.npcFrameCount[npc.type] = 14;
            NPCID.Sets.TrailCacheLength[npc.type] = 6;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;

			npc.width = 176;
            npc.height = 138;
            npc.aiStyle = aiType = -1;

            npc.lifeMax = 360420;
            npc.defense = 50;

            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 20, 0, 0);
			npc.DR_NERD(0.3f);
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit56;
            npc.DeathSound = SoundID.NPCDeath60;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.dontTakeDamage);
            writer.Write(Dying);
            writer.Write(Walking);
            writer.Write(AttackIndex);
            writer.Write(DelayTime);
            writer.Write(JumpTimer);
            writer.WriteVector2(ShootPosition);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.dontTakeDamage = reader.ReadBoolean();
            Dying = reader.ReadBoolean();
            Walking = reader.ReadBoolean();
            AttackIndex = reader.ReadInt32();
            DelayTime = reader.ReadInt32();
            JumpTimer = reader.ReadSingle();
            ShootPosition = reader.ReadVector2();
        }
        public override void AI()
        {
            Lighting.AddLight(npc.Center, (Dying ? Color.Lime.ToVector3() : Color.White.ToVector3()) * 2f);
            if (Dying)
                return;
            bool phase2 = npc.life / (float)npc.lifeMax < 0.5f;
            if (DelayTime > 0)
            {
                DelayTime--;
                npc.velocity.X *= 0.9f;
                if (npc.velocity.Y < 18f)
                {
                    npc.velocity.Y += 0.35f;
                }
                return;
            }
            if (npc.target < 0 || npc.target >= 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest(false);
                npc.netUpdate = true;
            }
            if (npc.ai[1] > -TeleportCooldown)
            {
                TeleportEffects();
            }
            Player player = Main.player[npc.target];
            npc.defDamage = 170;
            npc.damage = Dying ? 0 : npc.defDamage;
            TeleportCheck(player);
            npc.ai[0]++;
            Walking = false;
            // For teleporting if constantly spam-colliding
            if (npc.collideX)
            {
                if (npc.localAI[0] > 0)
                {
                    npc.localAI[1]++;
                }
                npc.localAI[0] = 20f;
            }
            if (npc.localAI[0] > 0)
            {
                npc.localAI[0]--;
            }
            if (npc.ai[0] % AttackCycleTime < 240f)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X *= 0.8f;
                    if (JumpTimer++ >= 70f || !Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                    {
                        npc.velocity.Y -= MathHelper.Clamp(Math.Abs(player.Center.Y - npc.Center.Y) / 12.5f, 8f, 18f);
                        npc.velocity.X = npc.DirectionTo(player.Center).X * 18f;
                        JumpTimer = 0f;
                        npc.netUpdate = true;
                    }
                    else
                    {
                        bool wasWalking = Walking;
                        if (wasWalking != Math.Abs(npc.velocity.X) > 4f)
                        {
                            Walking = Math.Abs(npc.velocity.X) > 4f;
                            npc.netUpdate = true;
                        }
                        if (npc.collideX)
                        {
                            JumpTimer = 50; // Force a jump the next frame to overcome the obstacle
                            npc.netUpdate = true;
                        }
                        else if (Math.Abs(player.Center.X - npc.Center.X) > 125f)
                        {
                            npc.velocity.X += Math.Sign(npc.DirectionTo(player.Center).X) * 3f;
                            npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -28f, 28f);
                        }
                        else
                        {
                            npc.velocity.X *= 0.99f;
                        }
                    }
                }
                npc.spriteDirection = (npc.velocity.X < 0).ToDirectionInt();
            }
            else
            {
                if (npc.ai[0] % AttackCycleTime == 255f)
                {
                    ShootPosition = player.Center;
                    npc.netUpdate = true;
                    npc.spriteDirection = (ShootPosition.X - npc.Center.X < 0).ToDirectionInt();
                }
                switch (PhaseArray[AttackIndex])
                {
                    // Tightly packed, diverging bullets
                    case 0:
                        npc.velocity.X *= 0.9f;
                        if (((npc.ai[0] % AttackCycleTime >= AttackCycleTime - SpecialAttackTime + 20f && npc.ai[0] % AttackCycleTime <= AttackCycleTime - SpecialAttackTime + 32f) ||
                            (npc.ai[0] % AttackCycleTime >= AttackCycleTime - SpecialAttackTime + 50f && npc.ai[0] % AttackCycleTime <= AttackCycleTime - SpecialAttackTime + 62f) ||
                            (npc.ai[0] % AttackCycleTime >= AttackCycleTime - SpecialAttackTime + 80f && npc.ai[0] % AttackCycleTime <= AttackCycleTime - SpecialAttackTime + 92f) ||
                            (npc.ai[0] % AttackCycleTime >= AttackCycleTime - SpecialAttackTime + 110f && npc.ai[0] % AttackCycleTime <= AttackCycleTime - SpecialAttackTime + 122f) ||
                            (npc.ai[0] % AttackCycleTime >= AttackCycleTime - SpecialAttackTime + 140f && npc.ai[0] % AttackCycleTime <= AttackCycleTime - SpecialAttackTime + 152f)) && npc.ai[0] % 3f == 0f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float angle = (npc.ai[0] % AttackCycleTime - (AttackCycleTime - SpecialAttackTime + 20f)) % 12f / 12f * MathHelper.ToRadians(15f) - MathHelper.ToRadians(7.5f);
                                int idx = Projectile.NewProjectile(npc.Center, npc.DirectionTo(ShootPosition).RotatedBy(angle) * 14f, ModContent.ProjectileType<NuclearBulletLarge>(), 48, 4f);
                                Main.projectile[idx].localAI[0] = angle;
                            }
                            Main.PlaySound(SoundID.NPCDeath13, npc.Center);
                        }
                        if (npc.ai[0] % AttackCycleTime >= (AttackCycleTime - SpecialAttackTime + 35f) && npc.ai[0] % 10f == 9f)
                        {
                            Projectile.NewProjectile(npc.Center, npc.DirectionTo(player.Center) * 12f, ModContent.ProjectileType<NuclearBulletLarge>(), 48, 3f);
                        }
                        break;
                    // Cone of bullets
                    case 1:
                        npc.velocity.X *= 0.9f;
                        if (npc.ai[0] % AttackCycleTime >= AttackCycleTime - SpecialAttackTime + 35f && npc.ai[0] % 4f == 3f)
                        {
                            float angle = MathHelper.Lerp(MathHelper.ToRadians(35f), MathHelper.ToRadians(5f), (npc.ai[0] % AttackCycleTime - (AttackCycleTime - SpecialAttackTime + 35f)) / (SpecialAttackTime + 35f));
                            Projectile.NewProjectile(npc.Center, npc.DirectionTo(player.Center).RotatedBy(angle) * 16f, ModContent.ProjectileType<NuclearBulletLarge>(), 48, 4.5f);
                            Projectile.NewProjectile(npc.Center, npc.DirectionTo(player.Center).RotatedBy(-angle) * 16f, ModContent.ProjectileType<NuclearBulletLarge>(), 48, 4.5f);
                        }
                        break;
                    // Shotgun bursts of bullets
                    case 2:
                        npc.velocity.X *= 0.9f;
                        if (npc.ai[0] % AttackCycleTime >= AttackCycleTime - SpecialAttackTime + 35f && npc.ai[0] % 20f == 19f)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                float angle = MathHelper.Lerp(-0.5f, 0.5f, i / 3f);
                                Projectile.NewProjectile(npc.Center, npc.DirectionTo(ShootPosition).RotatedBy(angle) * 13f, ModContent.ProjectileType<NuclearBulletMedium>(), 48, 4f);
                            }
                        }
                        break;
                }
                if (npc.ai[0] % AttackCycleTime == AttackCycleTime - 1f)
                {
                    DelayTime = phase2 ? 45 : 75;
                    AttackIndex++;
                    AttackIndex %= PhaseArray.Length;
                }
            }
        }
        public void TeleportCheck(Player player)
        {
            if (npc.ai[1] <= -TeleportCooldown)
            {
                if (npc.Distance(player.Center) > 2700f || 
                    (!Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height) && npc.Distance(player.Center) > 900f) ||
                    StuckOnPlatform(player) ||
                    npc.wet ||
                    npc.localAI[1] > 5f)
                {
                    Point playerPositionTileCoords = player.position.ToTileCoordinates();
                    Point npcPositionTileCoords = npc.position.ToTileCoordinates();
                    int tries = 0;
                    int maxTeleportDistance = 20;
                    bool cannotTeleport = false;
                    while (!cannotTeleport && tries < 250)
                    {
                        tries++;
                        int x = Main.rand.Next(playerPositionTileCoords.X - maxTeleportDistance, playerPositionTileCoords.X + maxTeleportDistance);
                        int yStart = Main.rand.Next(playerPositionTileCoords.Y - maxTeleportDistance, playerPositionTileCoords.Y + maxTeleportDistance);
                        if (StuckOnPlatform(player))
                        {
                            yStart = Main.rand.Next(playerPositionTileCoords.Y, playerPositionTileCoords.Y + 4 * maxTeleportDistance);
                        }
                        for (int y = yStart; y < playerPositionTileCoords.Y + maxTeleportDistance; y++)
                        {
                            if ((y < playerPositionTileCoords.Y - 12 || y > playerPositionTileCoords.Y + 12 || x < playerPositionTileCoords.X - 12 || x > playerPositionTileCoords.X + 12)
                                && (y < npcPositionTileCoords.Y - 8 || y > npcPositionTileCoords.Y + 8 || x < npcPositionTileCoords.X - 7 || x > npcPositionTileCoords.X + 7)
                                && CalamityUtils.ParanoidTileRetrieval(x, y).nactive())
                            {
                                bool canTeleport = true;
                                if (CalamityUtils.ParanoidTileRetrieval(x, y - 1).lava())
                                {
                                    canTeleport = false;
                                }
                                if (canTeleport &&
                                    Main.tileSolid[CalamityUtils.ParanoidTileRetrieval(x, y).type] && 
                                    !Collision.SolidTiles(x - 12, x + 12, y - 7, y - 7))
                                {
                                    for (int dy = y - 5; dy <= y; dy++)
                                    {
                                        if (CalamityUtils.ParanoidTileRetrieval(x, dy).liquid > 0)
                                        {
                                            continue;
                                        }
                                    }
                                    npc.ai[1] = TeleportTime;
                                    npc.ai[2] = x;
                                    npc.ai[3] = y - 3;
                                    cannotTeleport = true;
                                    npc.localAI[1] = 0f;
                                    break;
                                }
                            }
                        }
                    }
                    npc.netUpdate = true;
                }
            }
        }
        public void TeleportEffects()
        {
            if (npc.ai[1] > TeleportTime)
                npc.ai[1] = TeleportTime;
            npc.ai[1]--;
            if (npc.ai[1] >= 0f)
            {
                if (npc.ai[1] == 0f && npc.ai[2] != 0f && npc.ai[3] != 0f)
                {
                    npc.position.X = npc.ai[2] * 16f - npc.width / 2 + 8f;
                    npc.position.Y = npc.ai[3] * 16f - npc.height;
                    npc.netUpdate = true;
                    npc.velocity = Vector2.Zero;
                }
                else
                {
                    npc.alpha = (int)MathHelper.Lerp(0f, 255f, 1f - npc.ai[1] / TeleportTime);
                    int totalDust = (int)(30 * npc.alpha / 255f);
                    for (int i = 0; i < totalDust; i++)
                    {
                        Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid);
                        dust.noGravity = true;
                        dust.velocity = npc.DirectionFrom(dust.position) * 2f;
                        dust.scale = 1.6f;
                    }
                    npc.velocity.X *= 0.95f;
                    if (npc.velocity.Y < 18f)
                    {
                        npc.velocity.Y += 0.35f;
                    }
                }
                return;
            }
            else if (npc.ai[1] >= -TeleportFadeinTime)
            {
                npc.alpha = (int)MathHelper.Lerp(255f, 0f, npc.ai[1] / -TeleportFadeinTime);
                if (npc.ai[1] == -TeleportFadeinTime)
                {
                    for (int i = 0; i < 48; i++)
                    {
                        Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid);
                        dust.noGravity = true;
                        dust.velocity = npc.DirectionFrom(dust.position) * Main.rand.NextFloat(2f, 3.6f);
                        dust.scale = 1.8f;
                    }
                }
            }
        }
        public bool StuckOnPlatform(Player player)
        {
            for (int i = 0; i < 18; i++)
            {
                Point bottom = (npc.Bottom + Vector2.UnitY * i).ToTileCoordinates();
                if (TileID.Sets.Platforms[CalamityUtils.ParanoidTileRetrieval(bottom.X, bottom.Y).type] && player.Top.Y > npc.Bottom.Y + 48)
                    return true;
            }
            return false;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            int framesNeeded = Dying ? 7 : 6;
            if (Walking)
            {
                framesNeeded = 8 - (int)Math.Ceiling(Math.Abs(npc.velocity.X) / 5f); // Walk faster the faster we're moving
            }
            if (npc.frameCounter >= framesNeeded)
            {
                npc.frame.Y += frameHeight;
                npc.frameCounter = 0;
            }
            if (Dying)
            {
                if (npc.frame.Y < frameHeight * 8)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            int type = Main.rand.NextBool(4) ? ModContent.ProjectileType<SulphuricAcidMist>() : ModContent.ProjectileType<NuclearBulletLarge>();
                            float angle = MathHelper.TwoPi / 16f * i;
                            Projectile.NewProjectile(npc.Center, angle.ToRotationVector2() * Main.rand.NextFloat(4f, 11f), type, 48, 3f);
                        }
                    }
                    for (int i = 0; i < 60; i++)
                    {
                        Dust dust = Dust.NewDustDirect(npc.Center, 45, 45, (int)CalamityDusts.SulfurousSeaAcid);
                        dust.velocity = Utils.NextVector2Unit(Main.rand) * Main.rand.NextFloat(4f, 15f);
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(2f, 3f);
                    }
                    npc.frame.Y = frameHeight * 8;
                }
                if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
                {
                    npc.StrikeNPCNoInteraction(9999, 0f, 0);
                }
            }
            else if (npc.frame.Y >= (Walking ? 8 : 4) * frameHeight)
            {
                npc.frame.Y = Walking ? 4 * frameHeight : 0;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            CalamityGlobalNPC.DrawGlowmask(npc, spriteBatch, null, true);
            if (npc.velocity.Length() > 0f)
            {
                Color endColor = Color.DarkOliveGreen;
                endColor.A = Color.Transparent.A;
                CalamityGlobalNPC.DrawAfterimage(npc, spriteBatch, drawColor, endColor, directioning: true, invertedDirection: true);
            }
            return false;
        }
        public override bool CheckDead()
        {
            if (!Dying)
            {
                npc.active = true;
                npc.life = 1;
                npc.dontTakeDamage = true;
                Dying = true;
                npc.velocity = Vector2.Zero;
                npc.netUpdate = true;
                return false;
            }
            return Dying;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 300);
        }
        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<GammaHeart>(), 3);
            DropHelper.DropItemChance(npc, ModContent.ItemType<PhosphorescentGauntlet>(), 3);
        }
    }
}
