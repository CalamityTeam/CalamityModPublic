using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    public class PolterghastBag : ModItem, ILocalizedModType
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
            Item.width = 32;
            Item.height = 34;
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

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/TreasureBags/PolterghastBagGlow").Value);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
			// Money
			itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(ModContent.NPCType<Polterghast>()));

            // Materials
            itemLoot.Add(ModContent.ItemType<RuinousSoul>(), 1, 10, 20);
            itemLoot.Add(ModContent.ItemType<Polterplasm>(), 1, 40, 50);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<BansheeHook>(),
                ModContent.ItemType<TerrorBlade>(),
                ModContent.ItemType<DaemonsFlame>(),
                ModContent.ItemType<FatesReveal>(),
                ModContent.ItemType<GhastlyVisage>(),
                ModContent.ItemType<EtherealSubjugator>(),
                ModContent.ItemType<GhoulishGouger>()
            }));

            // Equipment
            itemLoot.Add(ModContent.ItemType<Affliction>());
            itemLoot.AddIf((info) => CalamityWorld.revenge && !info.player.Calamity().adrenalineBoostThree, ModContent.ItemType<Ectoheart>());
            itemLoot.AddRevBagAccessories();

            // Vanity
            itemLoot.Add(ModContent.ItemType<PolterghastMask>(), 7);
            itemLoot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
        }
    }
}
