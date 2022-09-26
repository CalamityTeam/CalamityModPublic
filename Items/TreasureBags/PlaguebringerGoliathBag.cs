using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.PlaguebringerGoliath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class PlaguebringerGoliathBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (The Plaguebringer Goliath)");
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
			itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<PlaguebringerGoliath>()));

            // Materials
            itemLoot.Add(ModContent.ItemType<PlagueCellCanister>(), 1, 25, 30);
            itemLoot.Add(ModContent.ItemType<InfectedArmorPlating>(), 1, 30, 35);
            itemLoot.Add(ItemID.Stinger, 1, 4, 8);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<DiseasedPike>(),
                ModContent.ItemType<Pandemic>(),
                ModContent.ItemType<Virulence>(),
                ModContent.ItemType<BlightSpewer>(),
                ModContent.ItemType<Malevolence>(),
                ModContent.ItemType<PestilentDefiler>(),
                ModContent.ItemType<TheHive>(),
                ModContent.ItemType<PlagueStaff>(),
                ModContent.ItemType<FuelCellBundle>(),
                ModContent.ItemType<InfectedRemote>(),
                ModContent.ItemType<TheSyringe>()
            }));
            itemLoot.Add(ModContent.ItemType<Malachite>(), 10);

            // Equipment
            itemLoot.Add(ModContent.ItemType<ToxicHeart>());
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<PlaguebringerGoliathMask>(), 7);
            itemLoot.Add(ModContent.ItemType<PlagueCaller>(), 10);
        }
    }
}
