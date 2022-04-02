using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Buffs.DamageOverTime;
namespace CalamityMod.NPCs.AcidRain
{
    public class Radiator : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Radiator");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 67;
            npc.damage = 10;
            npc.width = 24;
            npc.height = 24;
            npc.defense = 5;
            npc.lifeMax = 50;

            if (CalamityWorld.downedPolterghast)
            {
                npc.damage = 60;
                npc.lifeMax = 3250;
                npc.defense = 20;
            }
            else if (CalamityWorld.downedAquaticScourge)
            {
                npc.damage = 30;
                npc.lifeMax = 130;
                npc.defense = 10;
            }

            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            aiType = NPCID.GlowingSnail;
            banner = npc.type;
            bannerItem = ModContent.ItemType<RadiatorBanner>();
            npc.catchItem = (short)ModContent.ItemType<RadiatingCrystal>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void AI()
        {            
            Lighting.AddLight(npc.Center, 0.3f, 1.5f, 0.3f);

            int auraSize = 200; //roughly 12 blocks (half the size of Wither Beast aura)
            Player player = Main.player[Main.myPlayer];
            if (!player.dead && player.active && (double) (player.Center - npc.Center).Length() < auraSize)
            {
                player.AddBuff(ModContent.BuffType<Irradiated>(), 3, false);
                player.AddBuff(BuffID.Poisoned, 2, false);
                if (CalamityWorld.downedPolterghast)
                {
                    player.AddBuff(ModContent.BuffType<SulphuricPoisoning>(), 3, false);
                    player.AddBuff(BuffID.Venom, 2, false);
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter > 8)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y > frameHeight * 2)
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
        }

        public override void OnCatchNPC(Player player, Item item)
        {
            try
            {
            } 
            catch
            {
                return;
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<SulfuricScale>(), 2, 1, 3);
        }
    }
}
