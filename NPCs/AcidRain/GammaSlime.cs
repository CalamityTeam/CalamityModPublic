using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Enemy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.AcidRain
{
    public abstract class CalamityBoss : ModNPC
    {
        public abstract List<int> Loot { get; }
    }

    public class GammaSlime : ModNPC
    {
        public float angularMultiplier1;
        public float angularMultiplier2;

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
			npc.lifeMax = 9200;
			npc.DR_NERD(0.15f);
			npc.defense = 25;

            npc.aiStyle = aiType = -1;

			npc.knockBackResist = 0f;
            animationType = NPCID.CorruptSlime;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.value = Item.buyPrice(0, 0, 8, 30);
            npc.alpha = 50;
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<GammaSlimeBanner>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(angularMultiplier1);
            writer.Write(angularMultiplier2);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            angularMultiplier1 = reader.ReadSingle();
            angularMultiplier2 = reader.ReadSingle();
        }

        public override void AI()
        {
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.6f, 0.8f, 0.6f);
            npc.TargetClosest(false);
            Player player = Main.player[npc.target];
            if (npc.velocity.Y == 0f && npc.ai[3] <= 0f && !player.npcTypeNoAggro[npc.type])
            {
                npc.velocity.X *= 0.8f;
                if (npc.ai[0]++ >= 30f)
                {
                    npc.velocity.Y -= MathHelper.Clamp(Math.Abs(player.Center.Y - npc.Center.Y) / 16f, 5f, 15f);
                    npc.velocity.X = npc.DirectionTo(player.Center).X * 16f;
                    npc.ai[0] = 0f;
                    npc.ai[1]++;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            float angle = MathHelper.TwoPi / 5f * i + (npc.ai[1] % 2) * MathHelper.PiOver2;
                            Projectile.NewProjectile(npc.Center, angle.ToRotationVector2() * 7f, ModContent.ProjectileType<GammaAcid>(),
                                Main.expertMode ? 36 : 45, 3f);
                        }
                    }
                    npc.netUpdate = true;
                }
            }
            else
            {
                npc.velocity.X *= 0.9935f;
            }
            if (npc.ai[3] > 0f)
                npc.ai[3]--;
            if (npc.ai[3] > 240f)
            {
                npc.velocity.X *= 0.95f;
                npc.velocity.Y += 0.2f;
            }
            // Hold energy for laser
            if (npc.ai[3] > 480f)
            {
                float scale = npc.ai[3] < 530 ? 2.25f : 1.65f;
                Vector2 destination = npc.Top + new Vector2(0f, 6f);
                Dust dust = Dust.NewDustPerfect(destination + new Vector2(0f, 12f).RotatedByRandom(MathHelper.TwoPi), (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = Vector2.Normalize(destination - dust.position) * 3f;
                dust.scale = scale;
                dust.noGravity = true;
                if (npc.ai[3] <= 540f)
                {
                    float length = MathHelper.Lerp(20f, 550f, (npc.ai[3] - 480f) / 60f);
                    float outwardness = MathHelper.Lerp(1f, 0f, (npc.ai[3] - 480f) / 60f);
                    for (float i = npc.Top.Y + 4f; i >= npc.Top.Y + 4f - length; i -= 8f)
                    {
                        float angle = i / 24f;
                        Vector2 spawnPosition = new Vector2(npc.Center.X, i);
                        dust = Dust.NewDustPerfect(spawnPosition, (int)CalamityDusts.SulfurousSeaAcid);
                        dust.scale = 1.5f;
                        dust.velocity = Vector2.UnitX * (float)Math.Cos(angle) * 4f * outwardness;
                        dust.noGravity = true;
                    }
                }
            }
            // Release laser
            if (npc.ai[3] == 480f)
            {
                angularMultiplier1 = Main.rand.NextFloat(3f);
                angularMultiplier2 = Main.rand.NextFloat(4f);
                npc.netUpdate = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Main.PlaySound(SoundID.Zombie, (int)npc.position.X, (int)npc.position.Y, 104); // Moon lord beam sound
                    Projectile.NewProjectile(npc.Center, -Vector2.UnitY, ModContent.ProjectileType<GammaBeam>(), Main.expertMode ? 96 : 120, 4f, Main.myPlayer, 0f, npc.whoAmI);
                }
            }
            // Very complex particle effects while releasing the beam
            else if (npc.ai[3] >= 300f)
            {
                float angle = (npc.ai[3] / 30f) % MathHelper.TwoPi;
                float x = (float)Math.Sin(angle * angularMultiplier1) * (float)Math.Cos(angle);
                float y = (float)Math.Cos(angle * angularMultiplier1) * (float)Math.Sin(angle);
                Vector2 velocity = new Vector2(x * 4.5f, y * 2f);
                Dust dust = Dust.NewDustPerfect(npc.Center + angle.ToRotationVector2() * 8f, (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = velocity;
                dust.scale = (float)Math.Cos(angle) + 2f;
                dust.noGravity = true;

                dust = Dust.NewDustPerfect(npc.Center + angle.ToRotationVector2() * 8f, (int)CalamityDusts.SulfurousSeaAcid);
                dust.velocity = -velocity;
                dust.scale = (float)Math.Cos(angle) + 2f;
                dust.noGravity = true;
            }
            if (Math.Abs(player.Center.X - npc.Center.X) < 250f &&
                player.Center.X - npc.Center.X < 0f &&
                npc.ai[3] == 0f && 
                Main.rand.NextBool(110) && !player.npcTypeNoAggro[npc.type])
            {
                npc.ai[3] = 600f;
                npc.netUpdate = true;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/GammaSlimeGore"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/GammaSlimeGore2"), npc.scale);
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            if (npc.ai[3] >= 480f && npc.ai[3] <= 540f)
            {
                float length = MathHelper.Lerp(20f, 550f, (npc.ai[3] - 480f) / 60f);
                float opacity = MathHelper.Lerp(0.3f, 0.9f, (npc.ai[3] - 480f) / 60f);
                Utils.DrawLine(spriteBatch, npc.Top + new Vector2(0f, 4f), npc.Top + new Vector2(0f, 4f) - Vector2.UnitY * length, Color.Lerp(Color.Lime, Color.Transparent, opacity));
            }
            CalamityGlobalNPC.DrawGlowmask(npc, spriteBatch, ModContent.GetTexture(Texture + "Glow"));
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<LeadCore>(), 30);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 300);
        }
    }
}
