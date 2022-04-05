using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.NPCs.Crags
{
    public class CalamityEye : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamity Eye");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.lavaImmune = true;
            NPC.aiStyle = 2;
            NPC.damage = 40;
            NPC.width = 30;
            NPC.height = 30;
            NPC.defense = 12;
            NPC.lifeMax = 140;
            NPC.knockBackResist = 0f;
            animationType = NPCID.DemonEye;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            if (DownedBossSystem.downedProvidence)
            {
                NPC.damage = 80;
                NPC.defense = 20;
                NPC.lifeMax = 3000;
            }
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<CalamityEyeBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void AI()
        {
            if (NPC.life < NPC.lifeMax * 0.5)
            {
                if (NPC.direction == -1 && NPC.velocity.X > -6f)
                {
                    NPC.velocity.X -= 0.1f;
                    if (NPC.velocity.X > 6f)
                        NPC.velocity.X -= 0.1f;
                    else if (NPC.velocity.X > 0f)
                        NPC.velocity.X += 0.05f;
                    if (NPC.velocity.X < -6f)
                        NPC.velocity.X = -6f;
                }
                else if (NPC.direction == 1 && NPC.velocity.X < 6f)
                {
                    NPC.velocity.X += 0.1f;
                    if (NPC.velocity.X < -6f)
                        NPC.velocity.X += 0.1f;
                    else if (NPC.velocity.X < 0f)
                        NPC.velocity.X -= 0.05f;
                    if (NPC.velocity.X > 6f)
                        NPC.velocity.X = 6f;
                }
                if (NPC.directionY == -1 && NPC.velocity.Y > -4f)
                {
                    NPC.velocity.Y -= 0.1f;
                    if (NPC.velocity.Y > 4f)
                        NPC.velocity.Y -= 0.1f;
                    else if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y += 0.05f;
                    if (NPC.velocity.Y < -4f)
                        NPC.velocity.Y = -4f;
                }
                else if (NPC.directionY == 1 && NPC.velocity.Y < 4f)
                {
                    NPC.velocity.Y += 0.1f;
                    if (NPC.velocity.Y < -4f)
                        NPC.velocity.Y += 0.1f;
                    else if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y -= 0.05f;
                    if (NPC.velocity.Y > 4f)
                        NPC.velocity.Y = 4f;
                }
            }
            if (Main.rand.NextBool(40))
            {
                int index = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y + NPC.height * 0.25f), NPC.width, (int)(NPC.height * 0.5), DustID.Blood, NPC.velocity.X, 2f, 0, new Color(), 1f);
                Main.dust[index].velocity.X *= 0.5f;
                Main.dust[index].velocity.Y *= 0.1f;
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return spawnInfo.Player.Calamity().ZoneCalamity ? 0.25f : 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
        }

        public override void NPCLoot()
        {
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<Bloodstone>(), DownedBossSystem.downedProvidence, 2, 1, 1);
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<EssenceofChaos>(), Main.hardMode, 3, 1, 1);
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<BlightedLens>(), Main.hardMode, 2, 1, 1);
            DropHelper.DropItemChance(NPC, ItemID.Lens, 2);
        }
    }
}
