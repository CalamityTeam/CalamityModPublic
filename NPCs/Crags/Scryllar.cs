using CalamityMod.BiomeManagers;
using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Crags
{
    public class Scryllar : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                PortraitPositionYOverride = 10f
            };
            value.Position.Y += 20f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 50;
            NPC.width = 80;
            NPC.height = 80;
            NPC.defense = 18;
            NPC.lifeMax = 90;
            NPC.alpha = 100;
            NPC.knockBackResist = 0.7f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
            if (DownedBossSystem.downedProvidence)
            {
                NPC.damage = 90;
                NPC.defense = 30;
                NPC.lifeMax = 2500;
            }
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ScryllarBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<BrimstoneCragsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Scryllar")
            });
        }

        public override void AI()
        {
            NPC.rotation = NPC.velocity.X * 0.04f;
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            bool flag19 = false;
            if (NPC.justHit)
            {
                NPC.ai[2] = 0f;
            }
            if (NPC.ai[2] >= 0f)
            {
                int num282 = 16;
                bool flag21 = false;
                bool flag22 = false;
                if (NPC.position.X > NPC.ai[0] - (float)num282 && NPC.position.X < NPC.ai[0] + (float)num282)
                {
                    flag21 = true;
                }
                else if ((NPC.velocity.X < 0f && NPC.direction > 0) || (NPC.velocity.X > 0f && NPC.direction < 0))
                {
                    flag21 = true;
                }
                num282 += 24;
                if (NPC.position.Y > NPC.ai[1] - (float)num282 && NPC.position.Y < NPC.ai[1] + (float)num282)
                {
                    flag22 = true;
                }
                if (flag21 && flag22)
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= 30f && num282 == 16)
                    {
                        flag19 = true;
                    }
                    if (NPC.ai[2] >= 60f)
                    {
                        NPC.ai[2] = -200f;
                        NPC.direction *= -1;
                        NPC.velocity.X = NPC.velocity.X * -1f;
                        NPC.collideX = false;
                    }
                }
                else
                {
                    NPC.ai[0] = NPC.position.X;
                    NPC.ai[1] = NPC.position.Y;
                    NPC.ai[2] = 0f;
                }
                NPC.TargetClosest(true);
            }
            else
            {
                NPC.TargetClosest(true);
                NPC.ai[2] += 2f;
            }
            int num283 = (int)((NPC.position.X + (float)(NPC.width / 2)) / 16f) + NPC.direction * 2;
            int num284 = (int)((NPC.position.Y + (float)NPC.height) / 16f);
            bool flag23 = true;
            int num285 = 3;
            for (int num308 = num284; num308 < num284 + num285; num308++)
            {
                if ((Main.tile[num283, num308].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num283, num308].TileType]) || Main.tile[num283, num308].LiquidAmount > 0)
                {
                    flag23 = false;
                    break;
                }
            }
            if (Main.player[NPC.target].npcTypeNoAggro[NPC.type])
            {
                bool flag25 = false;
                for (int num309 = num284; num309 < num284 + num285 - 2; num309++)
                {
                    if ((Main.tile[num283, num309].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num283, num309].TileType]) || Main.tile[num283, num309].LiquidAmount > 0)
                    {
                        flag25 = true;
                        break;
                    }
                }
                NPC.directionY = (!flag25).ToDirectionInt();
            }
            if (flag19)
            {
                flag23 = true;
            }
            if (flag23)
            {
                NPC.velocity.Y = NPC.velocity.Y + 0.1f;
                if (NPC.velocity.Y > 3f)
                {
                    NPC.velocity.Y = 3f;
                }
            }
            else
            {
                if (NPC.directionY < 0 && NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.1f;
                }
                if (NPC.velocity.Y < -4f)
                {
                    NPC.velocity.Y = -4f;
                }
            }
            if (NPC.collideX)
            {
                NPC.velocity.X = NPC.oldVelocity.X * -0.4f;
                if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 1f)
                {
                    NPC.velocity.X = 1f;
                }
                if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -1f)
                {
                    NPC.velocity.X = -1f;
                }
            }
            if (NPC.collideY)
            {
                NPC.velocity.Y = NPC.oldVelocity.Y * -0.25f;
                if (NPC.velocity.Y > 0f && NPC.velocity.Y < 1f)
                {
                    NPC.velocity.Y = 1f;
                }
                if (NPC.velocity.Y < 0f && NPC.velocity.Y > -1f)
                {
                    NPC.velocity.Y = -1f;
                }
            }
            float num311 = 4f;
            if (NPC.direction == -1 && NPC.velocity.X > -num311)
            {
                NPC.velocity.X = NPC.velocity.X - 0.1f;
                if (NPC.velocity.X > num311)
                {
                    NPC.velocity.X = NPC.velocity.X - 0.1f;
                }
                else if (NPC.velocity.X > 0f)
                {
                    NPC.velocity.X = NPC.velocity.X + 0.05f;
                }
                if (NPC.velocity.X < -num311)
                {
                    NPC.velocity.X = -num311;
                }
            }
            else if (NPC.direction == 1 && NPC.velocity.X < num311)
            {
                NPC.velocity.X = NPC.velocity.X + 0.1f;
                if (NPC.velocity.X < -num311)
                {
                    NPC.velocity.X = NPC.velocity.X + 0.1f;
                }
                else if (NPC.velocity.X < 0f)
                {
                    NPC.velocity.X = NPC.velocity.X - 0.05f;
                }
                if (NPC.velocity.X > num311)
                {
                    NPC.velocity.X = num311;
                }
            }
            num311 = 1.5f;
            if (NPC.directionY == -1 && NPC.velocity.Y > -num311)
            {
                NPC.velocity.Y = NPC.velocity.Y - 0.04f;
                if (NPC.velocity.Y > num311)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.05f;
                }
                else if (NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.03f;
                }
                if (NPC.velocity.Y < -num311)
                {
                    NPC.velocity.Y = -num311;
                }
            }
            else if (NPC.directionY == 1 && NPC.velocity.Y < num311)
            {
                NPC.velocity.Y = NPC.velocity.Y + 0.04f;
                if (NPC.velocity.Y < -num311)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.05f;
                }
                else if (NPC.velocity.Y < 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.03f;
                }
                if (NPC.velocity.Y > num311)
                {
                    NPC.velocity.Y = num311;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.Calamity().ZoneCalamity ? 0.25f : 0f;
        }

        public static void DefineScryllarLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule hardmode = npcLoot.DefineConditionalDropSet(DropHelper.Hardmode());
            LeadingConditionRule postProv = npcLoot.DefineConditionalDropSet(DropHelper.PostProv());
            hardmode.Add(ModContent.ItemType<EssenceofHavoc>(), 2);
            postProv.Add(ModContent.ItemType<Bloodstone>(), 4);
            postProv.Add(ModContent.ItemType<GuidelightofOblivion>(), 20);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => DefineScryllarLoot(npcLoot);

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Scryllar").Type, NPC.scale);
                }
            }
        }
    }
}
