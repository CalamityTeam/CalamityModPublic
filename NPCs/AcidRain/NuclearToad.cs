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
        public const float ExplodeDistance = 185f;
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
            npc.damage = 75;
            npc.lifeMax = 255;
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
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = 302;
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
            if (npc.Distance(player.Center) < ExplodeDistance)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                        Projectile.NewProjectile(npc.Center, angle.ToRotationVector2() * Main.rand.NextFloat(6f, 9f), ModContent.ProjectileType<NuclearToadGoo>(), 27, 1f);
                    }
                }
                npc.life = 0;
                npc.HitEffect();
                npc.netUpdate = true;
                npc.active = false;
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
