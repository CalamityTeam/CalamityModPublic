using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.DesertScourge;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class DesertScourgeBag : ModItem
    {
		public override int BossBagNPC => ModContent.NPCType<DesertScourgeHead>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (Desert Scourge)");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
			ItemID.Sets.BossBag[Item.type] = true;
            ItemID.Sets.PreHardmodeLikeBossBag[Item.type] = true;
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
            // Materials
            itemLoot.Add(ModContent.ItemType<PearlShard>(), 1, 30, 40);
            itemLoot.Add(ItemID.Coral, 1, 30, 40);
            itemLoot.Add(ItemID.Seashell, 1, 30, 40);
            itemLoot.Add(ItemID.Starfish, 1, 30, 40);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<AquaticDischarge>(),
                ModContent.ItemType<Barinade>(),
                ModContent.ItemType<StormSpray>(),
                ModContent.ItemType<SeaboundStaff>(),
                ModContent.ItemType<ScourgeoftheDesert>()
            }));

            // Equipment
            itemLoot.Add(ModContent.ItemType<OceanCrest>());
            itemLoot.Add(ModContent.ItemType<AeroStone>(), DropHelper.BagWeaponDropRateFraction);
            itemLoot.Add(ModContent.ItemType<SandCloak>(), DropHelper.BagWeaponDropRateFraction);
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<DesertScourgeMask>(), 7);

            // Fishing
            itemLoot.Add(ModContent.ItemType<SandyAnglingKit>());
        }
    }
}
