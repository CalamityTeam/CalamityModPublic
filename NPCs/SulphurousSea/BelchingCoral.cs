using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SulphurousSea
{
    public class BelchingCoral : ModNPC
    {
        public const float CheckDistance = 480f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Belching Coral");
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.damage = 45;
            npc.width = 54;
            npc.height = 42;
            npc.defense = 25;
            npc.lifeMax = 1000;
            npc.aiStyle = aiType = -1;
            npc.value = Item.buyPrice(0, 0, 3, 50);
            npc.HitSound = SoundID.NPCHit42;
            npc.DeathSound = SoundID.NPCDeath5;
            npc.knockBackResist = 0f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<BelchingCoralBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }
        public override void AI()
        {
            npc.velocity.Y += 0.25f;
            npc.TargetClosest(false);
            Player player = Main.player[npc.target];
            if (Math.Abs(player.Center.X - npc.Center.X) < CheckDistance && player.Bottom.Y < npc.Top.Y)
            {
                if (npc.ai[0]++ % 35f == 34f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-11f, -6f));
                    Projectile.NewProjectile(npc.Top + new Vector2(0f, 6f), velocity, ModContent.ProjectileType<BelchingCoralSpike>(), 27, 3f);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !spawnInfo.player.Calamity().ZoneSulphur || !CalamityWorld.downedAquaticScourge)
            {
                return 0f;
            }
            return 0.085f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<CorrodedFossil>(), 15); // Rarer to encourage fighting Acid Rain
            DropHelper.DropItemChance(npc, ModContent.ItemType<BelchingSaxophone>(), 10);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/BelchingCoralGore"), npc.scale);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/AcidRain/BelchingCoralGore2"), npc.scale);
            }
        }
    }
}
