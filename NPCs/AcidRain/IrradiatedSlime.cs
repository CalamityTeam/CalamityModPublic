using CalamityMod.Dusts;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.AcidRain
{
    public class IrradiatedSlime : ModNPC
    {
        public bool Falling = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Irradiated Slime");
            Main.npcFrameCount[npc.type] = 2;
        }

        public override void SetDefaults()
        {
            npc.width = 40;
            npc.height = 30;

            npc.damage = 42;
            npc.lifeMax = 220;
            npc.defense = 5;

            npc.knockBackResist = 0f;
            animationType = NPCID.CorruptSlime;
            aiType = NPCID.ToxicSludge;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.alpha = 50;
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            banner = npc.type;
            bannerItem = ModContent.ItemType<IrradiatedSlimeBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Falling);
            writer.Write(npc.aiStyle);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Falling = reader.ReadBoolean();
            npc.aiStyle = reader.ReadInt32();
        }

        public override void AI()
        {
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.6f, 0.8f, 0.6f);
            if (Falling)
            {
                npc.TargetClosest(false);
                Player player = Main.player[npc.target];
                npc.aiStyle = aiType = -1;

                npc.noTileCollide = npc.noGravity = true;
                if (player.Top.Y < npc.Bottom.Y)
                {
                    npc.noTileCollide = npc.noGravity = false;
                    Falling = false;
                    npc.netUpdate = true;
                }
                else
                {
                    npc.velocity = Vector2.UnitY * 6f;
                }
            }
            else
            {
                npc.aiStyle = 1;
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
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/IrradiatedSlime"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/IrradiatedSlime2"), 1f);
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            CalamityGlobalNPC.DrawGlowmask(npc, spriteBatch, ModContent.GetTexture(Texture + "Glow"));
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemChance(npc, ModContent.ItemType<LeadCore>(), 30);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
    }
}
