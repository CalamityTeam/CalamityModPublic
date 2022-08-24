using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Yharon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class YharonBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (Jungle Dragon, Yharon)");
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
			itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Yharon>()));

            // Materials
            itemLoot.Add(ModContent.ItemType<YharonSoulFragment>(), 1, 30, 35);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<DragonRage>(),
                ModContent.ItemType<TheBurningSky>(),
                ModContent.ItemType<ChickenCannon>(),
                ModContent.ItemType<DragonsBreath>(),
                ModContent.ItemType<PhoenixFlameBarrage>(),
                ModContent.ItemType<YharonsKindleStaff>(),
                ModContent.ItemType<FinalDawn>(),
                ModContent.ItemType<Wrathwing>(),
            }));
            itemLoot.Add(ModContent.ItemType<YharimsCrystal>(), 10);

            // Equipment
            itemLoot.Add(ModContent.ItemType<DrewsWings>());
            itemLoot.Add(ModContent.ItemType<YharimsGift>());
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<YharonMask>(), 7);
            itemLoot.Add(ModContent.ItemType<ForgottenDragonEgg>(), 10);
            itemLoot.Add(ModContent.ItemType<McNuggets>(), 10);
            itemLoot.AddIf(() => CalamityWorld.revenge, ModContent.ItemType<FoxDrive>());
        }
    }
}
