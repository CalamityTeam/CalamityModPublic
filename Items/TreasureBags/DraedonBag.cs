using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    [LegacyName("DraedonTreasureBag")]
    public class DraedonBag : ModItem
    {

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Box (Exo Mechs)");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
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

        public override void PostUpdate() => CalamityUtils.ForceItemIntoWorld(Item);

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return CalamityUtils.DrawTreasureBagInWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Originally, masks were 1/3.5, for whatever reason. Keeping it that way.
            Fraction maskFraction = new Fraction(2, 7);

            // Materials
            itemLoot.Add(ModContent.ItemType<ExoPrism>(), 1, 30, 40);

            // Boss-specific items
            var ares = itemLoot.DefineConditionalDropSet(() => DownedBossSystem.downedAres);
            ares.Add(ModContent.ItemType<PhotonRipper>());
            ares.Add(ModContent.ItemType<TheJailor>());
            ares.Add(ModContent.ItemType<AresExoskeleton>());
            ares.Add(ModContent.ItemType<AresMask>(), maskFraction);

            var thanatos = itemLoot.DefineConditionalDropSet(() => DownedBossSystem.downedThanatos);
            thanatos.Add(ModContent.ItemType<SpineOfThanatos>());
            thanatos.Add(ModContent.ItemType<RefractionRotor>());
            thanatos.Add(ModContent.ItemType<ThanatosMask>(), maskFraction);

            var artemisAndApollo = itemLoot.DefineConditionalDropSet(() => DownedBossSystem.downedArtemisAndApollo);
            artemisAndApollo.Add(ModContent.ItemType<SurgeDriver>());
            artemisAndApollo.Add(ModContent.ItemType<TheAtomSplitter>());
            artemisAndApollo.Add(ModContent.ItemType<ArtemisMask>(), maskFraction);
            artemisAndApollo.Add(ModContent.ItemType<ApolloMask>(), maskFraction);

            // Equipment
            itemLoot.Add(ModContent.ItemType<DraedonsHeart>());
            itemLoot.Add(ModContent.ItemType<ExoThrone>());
            itemLoot.AddRevBagAccessories();

            // Vanity (Draedon Mask)
            itemLoot.Add(ModContent.ItemType<DraedonMask>(), maskFraction);
        }
    }
}
