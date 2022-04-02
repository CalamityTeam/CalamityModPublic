using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AcidRain
{
    public class GammaSlime : ModNPC
    {
        public float DustAngleMultiplier1;
        public float DustAngleMultiplier2;
        public Player Target => Main.player[npc.target];
        public float LaserTelegraphPower => Utils.InverseLerp(540f, 480f, LaserShootCountdown, true);
        public float LaserTelegraphLength => MathHelper.Lerp(20f, 550f, LaserTelegraphPower);
        public float LaserTelegraphOpacity => MathHelper.Lerp(0.3f, 0.9f, LaserTelegraphPower);
        public ref float GammaAcidShootTimer => ref npc.ai[0];
        public ref float LaserShootCountdown => ref npc.ai[1];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gamma Slime");
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.width = 40;
            npc.height = 44;

            npc.damage = 110;
            npc.lifeMax = 5060;
            npc.DR_NERD(0.15f);
            npc.defense = 25;

            npc.aiStyle = aiType = -1;

            npc.knockBackResist = 0f;
            animationType = NPCID.CorruptSlime;
            npc.value = Item.buyPrice(0, 0, 8, 30);
            npc.alpha = 50;
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<GammaSlimeBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {
            // Release green light at the position of the slime.
            Lighting.AddLight((int)npc.Center.X / 16, (int)npc.Center.Y / 16, 0.6f, 0.8f, 0.6f);

            npc.TargetClosest(false);

            if (npc.velocity.Y == 0f && LaserShootCountdown <= 0f && !Target.npcTypeNoAggro[npc.type])
            {
                npc.velocity.X *= 0.8f;

                GammaAcidShootTimer++;

                // Jump into the air and release gamma acid upward.
                if (GammaAcidShootTimer % 30f == 29f)
                {
                    npc.velocity.Y -= MathHelper.Clamp(Math.Abs(Target.Center.Y - npc.Center.Y) / 16f, 5f, 15f);
                    npc.velocity.X = npc.SafeDirectionTo(Target.Center).X * 16f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            float angle = MathHelper.TwoPi / 5f * i;
                            if (GammaAcidShootTimer % 60f == 58f)
                                angle += MathHelper.PiOver2;
                            Projectile.NewProjectile(npc.Center, angle.ToRotationVector2() * 7f, ModContent.ProjectileType<GammaAcid>(), Main.expertMode ? 36 : 45, 3f);
                        }
                    }
                    npc.netUpdate = true;
                }
            }
            else
                npc.velocity.X *= 0.9935f;

            if (LaserShootCountdown > 0f)
                LaserShootCountdown--;

            // Stop moving for a bit after the laser.
            if (LaserShootCountdown > 240f)
            {
                npc.velocity.X *= 0.95f;
                npc.velocity.Y += 0.2f;
            }

            // Hold energy for laser
            if (LaserShootCountdown > 480f)
            {
                float scale = LaserShootCountdown < 530f ? 2.25f : 1.65f;
                Vector2 destination = npc.Top + new Vector2(0f, 6f);
                Dust dust = Dust.NewDustPerfect(destination + Main.rand.NextVector2CircularEdge(12f, 12f), (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = Vector2.Normalize(destination - dust.position) * 3f;
                dust.scale = scale;
                dust.noGravity = true;

                if (LaserShootCountdown <= 540f)
                {
                    for (float i = npc.Top.Y + 4f; i >= npc.Top.Y + 4f - LaserTelegraphLength; i -= 8f)
                    {
                        float angle = i / 24f;
                        Vector2 spawnPosition = new Vector2(npc.Center.X, i);
                        dust = Dust.NewDustPerfect(spawnPosition, (int)CalamityDusts.SulfurousSeaAcid);
                        dust.scale = 1.5f;
                        dust.velocity = Vector2.UnitX * (float)Math.Cos(angle) * (1f - LaserTelegraphPower) * 4f;
                        dust.noGravity = true;
                    }
                }
            }

            // Release a gamma laser upward.
            if (LaserShootCountdown == 480f)
            {
                DustAngleMultiplier1 = Main.rand.NextFloat(3f);
                DustAngleMultiplier2 = Main.rand.NextFloat(4f);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Main.PlaySound(SoundID.Zombie, (int)npc.position.X, (int)npc.position.Y, 104);
                    Projectile.NewProjectile(npc.Center, -Vector2.UnitY, ModContent.ProjectileType<GammaBeam>(), Main.expertMode ? 96 : 120, 4f, Main.myPlayer, 0f, npc.whoAmI);
                }
                npc.netUpdate = true;
            }

            // Very complex particle effects while releasing the beam
            else if (LaserShootCountdown >= 300f)
            {
                float angle = LaserShootCountdown / 30f % MathHelper.TwoPi;
                float horizontalSpeed = (float)Math.Sin(angle * DustAngleMultiplier1) * (float)Math.Cos(angle) * 4.5f;
                float verticalSpeed = (float)Math.Cos(angle * DustAngleMultiplier1) * (float)Math.Sin(angle) * 2f;
                Vector2 velocity = new Vector2(horizontalSpeed, verticalSpeed);
                Dust dust = Dust.NewDustPerfect(npc.Center + angle.ToRotationVector2() * 8f, (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = velocity;
                dust.scale = (float)Math.Cos(angle) + 2f;
                dust.noGravity = true;

                dust = Dust.NewDustPerfect(npc.Center + angle.ToRotationVector2() * 8f, (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = -velocity;
                dust.scale = (float)Math.Cos(angle) + 2f;
                dust.noGravity = true;
            }

            // Randomly prepare to shoot a laser.
            if (Main.netMode != NetmodeID.MultiplayerClient && Math.Abs(Target.Center.X - npc.Center.X) < 250f && Target.Center.X < npc.Center.X &&
                LaserShootCountdown == 0f && Main.rand.NextBool(110) && !Target.npcTypeNoAggro[npc.type])
            {
                LaserShootCountdown = 600f;
                npc.netUpdate = true;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 10; k++)
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/GammaSlimeGore"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/GammaSlimeGore2"), npc.scale);
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (LaserShootCountdown >= 480f && LaserShootCountdown <= 540f)
            {
                Vector2 laserBottom = npc.Top + Vector2.UnitY * 4f;
                Vector2 laserTop = laserBottom - Vector2.UnitY * LaserTelegraphLength;
                Utils.DrawLine(spriteBatch, laserBottom, laserTop, Color.Lerp(Color.Lime, Color.Transparent, LaserTelegraphOpacity));
            }
            CalamityGlobalNPC.DrawGlowmask(npc, spriteBatch, ModContent.GetTexture(Texture + "Glow"));
        }

        public override void NPCLoot() => DropHelper.DropItemChance(npc, ModContent.ItemType<LeadCore>(), 30);

        public override void OnHitPlayer(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
    }
}
