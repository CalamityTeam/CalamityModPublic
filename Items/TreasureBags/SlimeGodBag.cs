using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class SlimeGodBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (The Slime God)");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
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

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return CalamityUtils.DrawTreasureBagInWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Materials
            // No Gel is dropped here because the boss drops Gel directly
            itemLoot.Add(ModContent.ItemType<PurifiedGel>(), 1, 40, 52);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<OverloadedBlaster>(),
                ModContent.ItemType<AbyssalTome>(),
                ModContent.ItemType<EldritchTome>(),
                ModContent.ItemType<CorroslimeStaff>(),
                ModContent.ItemType<CrimslimeStaff>(),
                ModContent.ItemType<SlimePuppetStaff>()
            }));

            // Equipment
            itemLoot.Add(ModContent.ItemType<ManaPolarizer>());
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ItemDropRule.OneFromOptions(7, ModContent.ItemType<SlimeGodMask>(), ModContent.ItemType<SlimeGodMask2>()));

            // Other
            itemLoot.AddIf((info) => CalamityWorld.revenge && !info.player.Calamity().adrenalineBoostOne, ModContent.ItemType<ElectrolyteGelPack>());
        }
    }
}
