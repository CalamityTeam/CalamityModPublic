using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools.ClimateChange
{
    public class Cosmolight : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        // Hardcoded times set by the vanilla Journey Mode buttons.
        // These are "halfway through day" and "halfway through night" respectively.
        private const int NoonCutoff = 27000;
        private const int MidnightCutoff = 16200;

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.useAnimation = 9;
            Item.useTime = 9;
            Item.autoReuse = false; // Explicitly not autofire, since it can be used quickly now
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.Item60;
            Item.consumable = false;
        }

        public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
        {
            itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.ToolsOther;
        }

        public override bool CanUseItem(Player player) => !CalamityPlayer.areThereAnyDamnBosses;

        public override bool? UseItem(Player player)
        {
            //Only SinglePlayer or DedServ should change time to prevent unwanted race condition
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return true;

            // Early Morning -> Noon
            if (Main.dayTime && Main.time < NoonCutoff)
                Main.SkipToTime(NoonCutoff, true);

            // Afternoon -> Dusk
            else if (Main.dayTime)
                Main.SkipToTime(0, false);

            // Early Night -> Midnight
            else if (!Main.dayTime && Main.time < MidnightCutoff)
                Main.SkipToTime(MidnightCutoff, false);

            // Late Night -> Dawn
            else if (!Main.dayTime)
                Main.SkipToTime(0, true);

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FallenStar, 10).
                AddIngredient(ItemID.SoulofLight, 7).
                AddIngredient(ItemID.SoulofNight, 7).
                AddIngredient<EssenceofSunlight>(5).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
