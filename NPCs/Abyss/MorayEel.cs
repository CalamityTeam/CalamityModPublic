using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Melee;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public class MorayEel : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moray Eel");
            Main.npcFrameCount[npc.type] = 6;
        }

        public override void SetDefaults()
        {
            npc.noGravity = true;
            npc.damage = 55;
            npc.width = 136;
            npc.height = 40;
            npc.defense = 8;
            npc.lifeMax = 360;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(0, 0, 1, 0);
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.75f;
            banner = npc.type;
            bannerItem = ModContent.ItemType<MorayEelBanner>();
            npc.chaseable = false;
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            CalamityAI.PassiveSwimmingAI(npc, mod, 0, Main.player[npc.target].Calamity().GetAbyssAggro(300f, 250f), 0.15f, 0.1f, 6f, 4f, 0.1f);
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return npc.chaseable;
            }
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += npc.chaseable ? 0.15f : 0.075f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Venom, 90, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSulphur)
            {
                return 0f;
            }
            if (spawnInfo.player.Calamity().ZoneAbyssLayer1 && spawnInfo.water)
            {
                return SpawnCondition.CaveJellyfish.Chance * 1.2f;
            }
            return SpawnCondition.OceanMonster.Chance * 0.1f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<DepthBlade>(), Main.expertMode ? 10 : 15);
            DropHelper.DropItemChance(npc, ItemID.Flipper, 20);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 25; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
