using CalamityMod.Events;
using CalamityMod.NPCs.Crabulon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.SummonItems
{
    public class DecapoditaSprout : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Decapodita Sprout");
            Tooltip.SetDefault("Summons Crabulon when used in the mushroom biome\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.rare = ItemRarityID.Green;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ZoneGlowshroom && !NPC.AnyNPCs(ModContent.NPCType<CrabulonIdle>()) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
            Main.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int npc = NPC.NewNPC((int)(player.position.X + Main.rand.Next(-250, 251)), (int)(player.position.Y - 500f), ModContent.NPCType<CrabulonIdle>(), 1);
                Main.npc[npc].timeLeft *= 20;
                CalamityUtils.BossAwakenMessage(npc);
            }
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<CrabulonIdle>());

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GlowingMushroom, 50);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
