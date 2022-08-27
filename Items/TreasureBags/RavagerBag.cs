using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Ravager;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class RavagerBag : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (Ravager)");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
			ItemID.Sets.BossBag[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.maxStack = 999;
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
			itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<RavagerBody>()));

            // Materials (contained in Geodes to prevent overwhelming item floods)
            itemLoot.AddIf(() => !DownedBossSystem.downedProvidence, ModContent.ItemType<FleshyGeode>());
            itemLoot.AddIf(() => DownedBossSystem.downedProvidence, ModContent.ItemType<NecromanticGeode>());

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<UltimusCleaver>(),
                ModContent.ItemType<RealmRavager>(),
                ModContent.ItemType<Hematemesis>(),
                ModContent.ItemType<SpikecragStaff>(),
                ModContent.ItemType<CraniumSmasher>(),
            }));
            itemLoot.Add(ModContent.ItemType<Vesuvius>(), 10);
            itemLoot.Add(ModContent.ItemType<CorpusAvertor>(), 20);

            // Equipment
            itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1,
                ModContent.ItemType<BloodPact>(),
                ModContent.ItemType<FleshTotem>()
            ));
            itemLoot.AddIf(() => DownedBossSystem.downedProvidence, ModContent.ItemType<BloodflareCore>());
            itemLoot.AddRevBagAccessories();
            itemLoot.AddIf((info) => CalamityWorld.revenge && !info.player.Calamity().rageBoostTwo, ModContent.ItemType<InfernalBlood>());

            // Vanity
            itemLoot.Add(ModContent.ItemType<RavagerMask>(), 7);
        }
    }
}
