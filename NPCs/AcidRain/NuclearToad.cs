using CalamityMod.Dusts;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
namespace CalamityMod.NPCs.AcidRain
{
    public class NuclearToad : ModNPC
    {
        public const float ExplodeDistanceHM = 185f;
        public const float ExplodeDistancePreHM = 295f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Nuclear Toad");
            Main.npcFrameCount[npc.type] = 5;
        }

        public override void SetDefaults()
        {
            npc.width = 62;
            npc.height = 34;
            npc.defense = 4;
            if (Main.hardMode)
            {
                npc.lifeMax = Main.expertMode ? 400 : 350;
                npc.damage = 75;
            }
            else
            {
                npc.lifeMax = Main.expertMode ? 255 : 200;
                npc.damage = 45;
            }

            npc.damage = Main.hardMode ? 75 : 45;
            npc.lifeMax = Main.hardMode ? 350 : 180;

            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.lavaImmune = false;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<NuclearToadBanner>();
        }
        public override void AI()
        {
            npc.TargetClosest(false);
            Player player = Main.player[npc.target];
            // Hover on the top of the water
            if (npc.wet)
            {
                if (npc.velocity.Y > 2f)
                {
                    npc.velocity.Y *= 0.9f;
                }
                npc.velocity.Y -= 0.16f;
                if (npc.velocity.Y < -4f)
                {
                    npc.velocity.Y = -4f;
                }
            }
            else
            {
                if (npc.velocity.Y < -2f)
                {
                    npc.velocity.Y *= 0.9f;
                }
                npc.velocity.Y += 0.16f;
                if (npc.velocity.Y > 3f)
                {
                    npc.velocity.Y = 3f;
                }
                npc.ai[0] = 5f;
            }
            if (Main.rand.NextBool(480))
                Main.PlaySound(SoundID.Zombie, npc.Center, 13); // Ribbit sound
            float explodeDistance = Main.hardMode ? ExplodeDistanceHM : ExplodeDistancePreHM;
            if (npc.Distance(player.Center) < explodeDistance)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int damage = Main.hardMode ? 27 : 17;
                    for (int i = 0; i < 7; i++)
                    {
                        float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                        Projectile.NewProjectile(npc.Center, angle.ToRotationVector2() * Main.rand.NextFloat(6f, 9f), ModContent.ProjectileType<NuclearToadGoo>(), damage, 1f);
                    }
                }
                Main.PlaySound(SoundID.DD2_KoboldExplosion, npc.Center);
                npc.life = 0;
                npc.HitEffect();
                npc.active = false;
                npc.netUpdate = true;
            }
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter >= 4)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y >= Main.npcFrameCount[npc.type] * frameHeight)
                {
                    npc.frame.Y = 0;
                }
            }
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.damage = Main.hardMode ? 76 : 52;
            npc.lifeMax = Main.hardMode ? 420 : 215;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 8; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/NuclearToadGore1"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/NuclearToadGore2"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/NuclearToadGore3"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/NuclearToadGore4"), npc.scale);
                for (int i = 0; i < 25; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, Main.rand.NextFloat(-2f, 2f), -1f, 0, default, 1f);
                }
            }
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 300);
        }
    }
}
