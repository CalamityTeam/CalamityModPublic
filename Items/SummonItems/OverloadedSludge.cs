using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.SlimeGod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class OverloadedSludge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Overloaded Sludge");
            Tooltip.SetDefault("It looks corrupted\n" +
                "Summons the Slime God\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.LightRed;
            item.useAnimation = 45;
            item.useTime = 45;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<SlimeGodCore>()) && !NPC.AnyNPCs(ModContent.NPCType<SlimeGod>()) &&
                !NPC.AnyNPCs(ModContent.NPCType<SlimeGodSplit>()) && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodRun>()) && !NPC.AnyNPCs(ModContent.NPCType<SlimeGodRunSplit>()) && !BossRushEvent.BossRushActive;
        }

        public override bool UseItem(Player player)
        {
            Main.PlaySound(SoundID.Roar, player.position, 0);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<SlimeGodCore>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<SlimeGodCore>());

            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<EbonianGel>(), 40);
            recipe.AddRecipeGroup("AnyEvilBlock", 40);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
