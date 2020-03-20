using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.AcidRain
{
    public class FlakBaby : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Baby Flak Crab");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.width = 26;
            npc.height = 32;

            npc.damage = 0;
            npc.lifeMax = 5;
            npc.defense = 5;

            npc.knockBackResist = 0f;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.value = Item.buyPrice(0, 0, 5, 55);
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit41;
            npc.DeathSound = SoundID.DD2_WitherBeastDeath;
            banner = npc.type;
            bannerItem = ModContent.ItemType<FlakCrabBanner>();
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.85f);
        }
        public override void AI()
        {
            Player closest = Main.player[Player.FindClosest(npc.Top, 0, 0)];
            if (Math.Abs(closest.Center.X - npc.Center.X) > 600f)
            {
                npc.velocity.X *= 0.935f;
            }
            else
            {
                if (npc.velocity.Y == 0f && npc.collideX)
                {
                    npc.velocity.Y = -13f;
                }
                else
                {
                    npc.velocity.Y += 0.15f;
                }
                npc.spriteDirection = (closest.Center.X - npc.Center.X < 0).ToDirectionInt();
                if (Math.Abs(npc.velocity.X) < 35f)
                {
                    npc.velocity.X += npc.spriteDirection * 0.2f;
                }
            }
        }
        public override void FindFrame(int frameHeight)
        {
            Player closest = Main.player[Player.FindClosest(npc.Top, 0, 0)];
            if (Math.Abs(closest.Center.X - npc.Center.X) < 600f)
            {
                if (npc.ai[0]++ % 4 == 3)
                {
                    npc.frame.Y += frameHeight;
                }
                if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
                {
                    npc.frame.Y = frameHeight * 2;
                }
            }
            else
            {
                if (npc.ai[0]++ % 6 == 5)
                {
                    npc.frame.Y -= frameHeight;
                }
                if (npc.frame.Y <= 0)
                {
                    npc.frame.Y = 0;
                }
            }
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/FlakCrab"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/FlakCrab2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/FlakCrab3"), 1f);
            }
        }
    }
}
