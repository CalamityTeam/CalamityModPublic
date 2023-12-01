using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class BrimstoneWaifuBag : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.TreasureBags";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
			ItemID.Sets.BossBag[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.width = 24;
            Item.height = 24;
            Item.expert = true;
            Item.rare = ItemRarityID.Cyan;
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
			itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<BrimstoneElemental>()));

            // Materials
            itemLoot.Add(ModContent.ItemType<EssenceofHavoc>(), 1, 10, 12);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<Brimlance>(),
                ModContent.ItemType<SeethingDischarge>(),
                ModContent.ItemType<DormantBrimseeker>()
            }));
            itemLoot.Add(ModContent.ItemType<Hellborn>(), 10);

            // Equipment
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<RoseStone>(),
            }));
            itemLoot.Add(ModContent.ItemType<FlameLickedShell>());
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<BrimstoneWaifuMask>(), 7);
            itemLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            itemLoot.AddIf(() => CalamityWorld.revenge, ModContent.ItemType<CharredRelic>());
        }
    }
}
