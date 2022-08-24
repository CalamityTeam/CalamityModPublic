using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    [LegacyName("AstrageldonBag")]
    public class AstrumAureusBag : ModItem
    {
		public override int BossBagNPC => ModContent.NPCType<AstrumAureus>();

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (Astrum Aureus)");
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
            // Materials
            itemLoot.Add(ModContent.ItemType<AureusCell>(), 1, 12, 16);
            itemLoot.Add(ModContent.ItemType<Stardust>(), 1, 30, 40);
            itemLoot.Add(ItemID.FallenStar, 1, 20, 30);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<Nebulash>(),
                ModContent.ItemType<AuroraBlazer>(),
                ModContent.ItemType<AlulaAustralis>(),
                ModContent.ItemType<BorealisBomber>(),
                ModContent.ItemType<AuroradicalThrow>()
            }));
            itemLoot.Add(ModContent.ItemType<LeonidProgenitor>(), 10);

            // Equipment
            itemLoot.Add(ModContent.ItemType<SuspiciousLookingJellyBean>());
            itemLoot.Add(ModContent.ItemType<GravistarSabaton>());
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<AstrumAureusMask>(), 7);

            // Other
            itemLoot.AddIf((info) => CalamityWorld.revenge && !info.player.Calamity().adrenalineBoostTwo, ModContent.ItemType<StarlightFuelCell>());
        }
    }
}
