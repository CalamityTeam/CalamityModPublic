using CalamityMod.NPCs;
using CalamityMod.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class TheMonument : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";

        public const float MonumentTaxIncrease = 0.5f;
        public const float MonumentHappinessReduction = 0.15f;

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<TheMonumentTile>());
            Item.width = 40;
            Item.height = 50;
            Item.rare = ItemRarityID.Pink;
            Item.value = Item.buyPrice(platinum: 1); // Sold by Shady Salesman
        }

        public override void Load()
        {
            On_Player.CollectTaxes += AddMonumentTaxes;      
        }

        private static void AddMonumentTaxes(On_Player.orig_CollectTaxes orig, Player self)
        {
            orig(self);

            foreach (NPC n in Main.ActiveNPCs)
            {
                if (!n.homeless && !NPCID.Sets.IsTownPet[n.type] && NPC.TypeToDefaultHeadIndex(n.type) > 0 && n.GetGlobalNPC<CalamityGlobalTownNPC>().AffectedByTheMonument)
                    self.taxMoney += (int)(CalamityGlobalTownNPC.TotalTaxesPerNPC * MonumentTaxIncrease);
            }
            if (self.taxMoney > CalamityGlobalTownNPC.TaxesToCollectLimit)
                self.taxMoney = CalamityGlobalTownNPC.TaxesToCollectLimit;
        }
    }
}
