using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Calamitas;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class CalamitasBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (Calamitas)");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
			ItemID.Sets.BossBag[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ItemRarityID.Cyan;
            Item.expert = true;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossBags;
		}

        public override bool CanRightClick() => true;

		public override Color? GetAlpha(Color lightColor) => Color.Lerp(lightColor, Color.White, 0.4f);

        public override void PostUpdate() => Item.TreasureBagLightAndDust();

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return CalamityUtils.DrawTreasureBagInWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
			// Money
			itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<CalamitasClone>()));

            // Materials
            itemLoot.Add(ModContent.ItemType<AshesofCalamity>(), 1, 30, 35);
            itemLoot.Add(ModContent.ItemType<EssenceofChaos>(), 1, 10, 15);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<Oblivion>(),
                ModContent.ItemType<Animosity>(),
                ModContent.ItemType<LashesofChaos>(),
                ModContent.ItemType<EntropysVigil>()
            }));

            // Equipment
            itemLoot.Add(ModContent.ItemType<ChaosStone>(), DropHelper.BagWeaponDropRateFraction);
            itemLoot.Add(ModContent.ItemType<VoidofCalamity>());
            itemLoot.Add(ModContent.ItemType<Regenator>(), 10);
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<CalamitasMask>(), 7);
            var calamityRobes = ItemDropRule.Common(ModContent.ItemType<RobesOfCalamity>(), 10);
            calamityRobes.OnSuccess(ItemDropRule.Common(ModContent.ItemType<HoodOfCalamity>()));
            itemLoot.Add(calamityRobes);
            itemLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
        }
    }
}
