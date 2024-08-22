using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.AstrumAureus;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class AstralChunk : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.SummonItems";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12; // Truffle Worm
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Lime;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
        }

        public override bool CanUseItem(Player player)
        {
            return player.Calamity().ZoneAstral && !NPC.AnyNPCs(ModContent.NPCType<AstrumAureus>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            int posX = (int)(player.position.X + Main.rand.Next(-250, 251));
            int posY = (int)(player.position.Y - 500f);
            int bossToSpawn = ModContent.NPCType<AstrumAureus>();
            CalamityUtils.SpawnBossOnPosUsingItem(player, bossToSpawn, posX, posY, SoundID.Roar);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<StarblightSoot>(30).
                AddIngredient(ItemID.FallenStar, 20).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
