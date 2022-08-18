using CalamityMod.Events;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.SlimeGod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Items.SummonItems
{
    public class OverloadedSludge : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Overloaded Sludge");
            Tooltip.SetDefault("It looks corrupted\n" +
                "Summons the Slime God\n" +
                "Not consumable");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
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
            return !NPC.AnyNPCs(ModContent.NPCType<SlimeGodCore>()) && !BossRushEvent.BossRushActive;
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<SlimeGodCore>());
            else
                NetMessage.SendData(MessageID.SpawnBoss, -1, -1, null, player.whoAmI, ModContent.NPCType<SlimeGodCore>());

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BlightedGel>(40).
                AddRecipeGroup("AnyEvilBlock", 40).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
