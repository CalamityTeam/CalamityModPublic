using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Crags
{
    public class ScryllarRage : ModNPC
    {
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 65;
            NPC.width = 80;
            NPC.height = 80;
            NPC.defense = 10;
            NPC.lifeMax = 50;
            NPC.alpha = 100;
            NPC.knockBackResist = 0.9f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.NPCHit49;
            NPC.DeathSound = SoundID.NPCDeath51;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
            if (DownedBossSystem.downedProvidence)
            {
                NPC.damage = 110;
                NPC.defense = 20;
                NPC.lifeMax = 1500;
            }
            Banner = ModContent.NPCType<Scryllar>();
            BannerItem = ModContent.ItemType<ScryllarBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            NPC.rotation = NPC.velocity.X * 0.04f;
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;
            bool hoverDownDistCheck = false;
            if (NPC.justHit)
            {
                NPC.ai[2] = 0f;
            }
            if (NPC.ai[2] >= 0f)
            {
                int hoverDistance = 16;
                bool changeDirectionX = false;
                bool changeDirectionY = false;
                if (NPC.position.X > NPC.ai[0] - (float)hoverDistance && NPC.position.X < NPC.ai[0] + (float)hoverDistance)
                {
                    changeDirectionX = true;
                }
                else if ((NPC.velocity.X < 0f && NPC.direction > 0) || (NPC.velocity.X > 0f && NPC.direction < 0))
                {
                    changeDirectionX = true;
                }
                hoverDistance += 24;
                if (NPC.position.Y > NPC.ai[1] - (float)hoverDistance && NPC.position.Y < NPC.ai[1] + (float)hoverDistance)
                {
                    changeDirectionY = true;
                }
                if (changeDirectionX && changeDirectionY)
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= 30f && hoverDistance == 16)
                    {
                        hoverDownDistCheck = true;
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
            int npcTileX = (int)((NPC.position.X + (float)(NPC.width / 2)) / 16f) + NPC.direction * 2;
            int npcTileY = (int)((NPC.position.Y + (float)NPC.height) / 16f);
            bool hoverDownwards = true;
            int tileCheckLoopAmt = 3;
            for (int loopInc1 = npcTileY; loopInc1 < npcTileY + tileCheckLoopAmt; loopInc1++)
            {
                if ((Main.tile[npcTileX, loopInc1].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[npcTileX, loopInc1].TileType]) || Main.tile[npcTileX, loopInc1].LiquidAmount > 0)
                {
                    hoverDownwards = false;
                    break;
                }
            }
            if (Main.player[NPC.target].npcTypeNoAggro[NPC.type])
            {
                bool inTileNoAggro = false;
                for (int loopInc2 = npcTileY; loopInc2 < npcTileY + tileCheckLoopAmt - 2; loopInc2++)
                {
                    if ((Main.tile[npcTileX, loopInc2].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[npcTileX, loopInc2].TileType]) || Main.tile[npcTileX, loopInc2].LiquidAmount > 0)
                    {
                        inTileNoAggro = true;
                        break;
                    }
                }
                NPC.directionY = (!inTileNoAggro).ToDirectionInt();
            }
            if (hoverDownDistCheck)
            {
                hoverDownwards = true;
            }
            if (hoverDownwards)
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
            float maxXVelocity = 7f;
            if (NPC.direction == -1 && NPC.velocity.X > -maxXVelocity)
            {
                NPC.velocity.X = NPC.velocity.X - 0.1f;
                if (NPC.velocity.X > maxXVelocity)
                {
                    NPC.velocity.X = NPC.velocity.X - 0.1f;
                }
                else if (NPC.velocity.X > 0f)
                {
                    NPC.velocity.X = NPC.velocity.X + 0.05f;
                }
                if (NPC.velocity.X < -maxXVelocity)
                {
                    NPC.velocity.X = -maxXVelocity;
                }
            }
            else if (NPC.direction == 1 && NPC.velocity.X < maxXVelocity)
            {
                NPC.velocity.X = NPC.velocity.X + 0.1f;
                if (NPC.velocity.X < -maxXVelocity)
                {
                    NPC.velocity.X = NPC.velocity.X + 0.1f;
                }
                else if (NPC.velocity.X < 0f)
                {
                    NPC.velocity.X = NPC.velocity.X - 0.05f;
                }
                if (NPC.velocity.X > maxXVelocity)
                {
                    NPC.velocity.X = maxXVelocity;
                }
            }
            maxXVelocity = 1.5f;
            if (NPC.directionY == -1 && NPC.velocity.Y > -maxXVelocity)
            {
                NPC.velocity.Y = NPC.velocity.Y - 0.04f;
                if (NPC.velocity.Y > maxXVelocity)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.05f;
                }
                else if (NPC.velocity.Y > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.03f;
                }
                if (NPC.velocity.Y < -maxXVelocity)
                {
                    NPC.velocity.Y = -maxXVelocity;
                }
            }
            else if (NPC.directionY == 1 && NPC.velocity.Y < maxXVelocity)
            {
                NPC.velocity.Y = NPC.velocity.Y + 0.04f;
                if (NPC.velocity.Y < -maxXVelocity)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.05f;
                }
                else if (NPC.velocity.Y < 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.03f;
                }
                if (NPC.velocity.Y > maxXVelocity)
                {
                    NPC.velocity.Y = maxXVelocity;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.Calamity().ZoneCalamity ? 0.125f : 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) => Scryllar.DefineScryllarLoot(npcLoot);

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 60, true);
		}

		public override void OnKill()
		{
			// Increase the kill count of Scryllars for the Bestiary
			if (NPC.GetWereThereAnyInteractions())
			{
				NPC nPC = new NPC();
				nPC.SetDefaults(ModContent.NPCType<Scryllar>());
				Main.BestiaryTracker.Kills.RegisterKill(nPC);
			}
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
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScryllarRage").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScryllarRage2").Type, NPC.scale);
                }
            }
        }
    }
}
