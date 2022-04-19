using CalamityMod.Dusts;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
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
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 30;

            NPC.damage = 42;
            NPC.lifeMax = 220;
            NPC.defense = 5;

            NPC.knockBackResist = 0f;
            AnimationType = NPCID.CorruptSlime;
            AIType = NPCID.ToxicSludge;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.alpha = 50;
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<IrradiatedSlimeBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.AcidRainTier2,
                //BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.AcidRainTier3,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Encrusted with gunk that evaporates slowly from the sulphurous sea, these slimes actually form in the upper atmosphere’s acidic rain, before falling back to the earth.")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Falling);
            writer.Write(NPC.aiStyle);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Falling = reader.ReadBoolean();
            NPC.aiStyle = reader.ReadInt32();
        }

        public override void AI()
        {
            Lighting.AddLight((int)((NPC.position.X + (float)(NPC.width / 2)) / 16f), (int)((NPC.position.Y + (float)(NPC.height / 2)) / 16f), 0.6f, 0.8f, 0.6f);
            if (Falling)
            {
                NPC.TargetClosest(false);
                Player player = Main.player[NPC.target];
                NPC.aiStyle = AIType = -1;

                NPC.noTileCollide = NPC.noGravity = true;
                if (player.Top.Y < NPC.Bottom.Y)
                {
                    NPC.noTileCollide = NPC.noGravity = false;
                    Falling = false;
                    NPC.netUpdate = true;
                }
                else
                {
                    NPC.velocity = Vector2.UnitY * 6f;
                }
            }
            else
            {
                NPC.aiStyle = 1;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("IrradiatedSlime").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("IrradiatedSlime2").Type, 1f);
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            CalamityGlobalNPC.DrawGlowmask(NPC, spriteBatch, ModContent.Request<Texture2D>(Texture + "Glow").Value);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 180);
        }
    }
}
