using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Crags
{
    public class Scryllar : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scryllar");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 50;
            npc.width = 80;
            npc.height = 80;
            npc.defense = 18;
            npc.lifeMax = 90;
            npc.alpha = 100;
            npc.knockBackResist = 0.7f;
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.HitSound = SoundID.NPCHit49;
            npc.DeathSound = SoundID.NPCDeath51;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.lavaImmune = true;
            if (CalamityWorld.downedProvidence)
            {
                npc.damage = 90;
                npc.defense = 30;
                npc.lifeMax = 2500;
            }
            banner = npc.type;
            bannerItem = ModContent.ItemType<ScryllarBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToCold = true;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            npc.rotation = npc.velocity.X * 0.04f;
            npc.spriteDirection = (npc.direction > 0) ? 1 : -1;
            bool flag19 = false;
            if (npc.justHit)
            {
                npc.ai[2] = 0f;
            }
            if (npc.ai[2] >= 0f)
            {
                int num282 = 16;
                bool flag21 = false;
                bool flag22 = false;
                if (npc.position.X > npc.ai[0] - (float)num282 && npc.position.X < npc.ai[0] + (float)num282)
                {
                    flag21 = true;
                }
                else if ((npc.velocity.X < 0f && npc.direction > 0) || (npc.velocity.X > 0f && npc.direction < 0))
                {
                    flag21 = true;
                }
                num282 += 24;
                if (npc.position.Y > npc.ai[1] - (float)num282 && npc.position.Y < npc.ai[1] + (float)num282)
                {
                    flag22 = true;
                }
                if (flag21 && flag22)
                {
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 30f && num282 == 16)
                    {
                        flag19 = true;
                    }
                    if (npc.ai[2] >= 60f)
                    {
                        npc.ai[2] = -200f;
                        npc.direction *= -1;
                        npc.velocity.X = npc.velocity.X * -1f;
                        npc.collideX = false;
                    }
                }
                else
                {
                    npc.ai[0] = npc.position.X;
                    npc.ai[1] = npc.position.Y;
                    npc.ai[2] = 0f;
                }
                npc.TargetClosest(true);
            }
            else
            {
                npc.TargetClosest(true);
                npc.ai[2] += 2f;
            }
            int num283 = (int)((npc.position.X + (float)(npc.width / 2)) / 16f) + npc.direction * 2;
            int num284 = (int)((npc.position.Y + (float)npc.height) / 16f);
            bool flag23 = true;
            int num285 = 3;
            for (int num308 = num284; num308 < num284 + num285; num308++)
            {
                if (Main.tile[num283, num308] == null)
                {
                    Main.tile[num283, num308] = new Tile();
                }
                if ((Main.tile[num283, num308].nactive() && Main.tileSolid[(int)Main.tile[num283, num308].type]) || Main.tile[num283, num308].liquid > 0)
                {
                    flag23 = false;
                    break;
                }
            }
            if (Main.player[npc.target].npcTypeNoAggro[npc.type])
            {
                bool flag25 = false;
                for (int num309 = num284; num309 < num284 + num285 - 2; num309++)
                {
                    if (Main.tile[num283, num309] == null)
                    {
                        Main.tile[num283, num309] = new Tile();
                    }
                    if ((Main.tile[num283, num309].nactive() && Main.tileSolid[(int)Main.tile[num283, num309].type]) || Main.tile[num283, num309].liquid > 0)
                    {
                        flag25 = true;
                        break;
                    }
                }
                npc.directionY = (!flag25).ToDirectionInt();
            }
            if (flag19)
            {
                flag23 = true;
            }
            if (flag23)
            {
                npc.velocity.Y = npc.velocity.Y + 0.1f;
                if (npc.velocity.Y > 3f)
                {
                    npc.velocity.Y = 3f;
                }
            }
            else
            {
                if (npc.directionY < 0 && npc.velocity.Y > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y - 0.1f;
                }
                if (npc.velocity.Y < -4f)
                {
                    npc.velocity.Y = -4f;
                }
            }
            if (npc.collideX)
            {
                npc.velocity.X = npc.oldVelocity.X * -0.4f;
                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 1f)
                {
                    npc.velocity.X = 1f;
                }
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -1f)
                {
                    npc.velocity.X = -1f;
                }
            }
            if (npc.collideY)
            {
                npc.velocity.Y = npc.oldVelocity.Y * -0.25f;
                if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
                {
                    npc.velocity.Y = 1f;
                }
                if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
                {
                    npc.velocity.Y = -1f;
                }
            }
            float num311 = 4f;
            if (npc.direction == -1 && npc.velocity.X > -num311)
            {
                npc.velocity.X = npc.velocity.X - 0.1f;
                if (npc.velocity.X > num311)
                {
                    npc.velocity.X = npc.velocity.X - 0.1f;
                }
                else if (npc.velocity.X > 0f)
                {
                    npc.velocity.X = npc.velocity.X + 0.05f;
                }
                if (npc.velocity.X < -num311)
                {
                    npc.velocity.X = -num311;
                }
            }
            else if (npc.direction == 1 && npc.velocity.X < num311)
            {
                npc.velocity.X = npc.velocity.X + 0.1f;
                if (npc.velocity.X < -num311)
                {
                    npc.velocity.X = npc.velocity.X + 0.1f;
                }
                else if (npc.velocity.X < 0f)
                {
                    npc.velocity.X = npc.velocity.X - 0.05f;
                }
                if (npc.velocity.X > num311)
                {
                    npc.velocity.X = num311;
                }
            }
            num311 = 1.5f;
            if (npc.directionY == -1 && npc.velocity.Y > -num311)
            {
                npc.velocity.Y = npc.velocity.Y - 0.04f;
                if (npc.velocity.Y > num311)
                {
                    npc.velocity.Y = npc.velocity.Y - 0.05f;
                }
                else if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y + 0.03f;
                }
                if (npc.velocity.Y < -num311)
                {
                    npc.velocity.Y = -num311;
                }
            }
            else if (npc.directionY == 1 && npc.velocity.Y < num311)
            {
                npc.velocity.Y = npc.velocity.Y + 0.04f;
                if (npc.velocity.Y < -num311)
                {
                    npc.velocity.Y = npc.velocity.Y + 0.05f;
                }
                else if (npc.velocity.Y < 0f)
                {
                    npc.velocity.Y = npc.velocity.Y - 0.03f;
                }
                if (npc.velocity.Y > num311)
                {
                    npc.velocity.Y = num311;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.player.Calamity().ZoneCalamity ? 0.25f : 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(npc, ModContent.ItemType<LanternoftheSoul>(), CalamityWorld.downedProvidence, 20, 1, 1);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Bloodstone>(), CalamityWorld.downedProvidence, 2, 1, 1);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<EssenceofChaos>(), Main.hardMode, 3, 1, 1);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Brimstone, hitDirection, -1f, 0, default, 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/ScryllarGores/Scryllar"), npc.scale);
            }
        }
    }
}
