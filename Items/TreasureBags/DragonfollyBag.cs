using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.TreasureBags
{
    [LegacyName("BumblebirbBag")]
    public class DragonfollyBag : ModItem
    {

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
            DisplayName.SetDefault("Treasure Bag (The Dragonfolly)");
            Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
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

        public override bool CanRightClick() => true;

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            return CalamityUtils.DrawTreasureBagInWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        }

        public override void ModifyItemLoot(ItemLoot itemLoot)
        {
            // Materials
            itemLoot.Add(ModContent.ItemType<EffulgentFeather>(), 1, 30, 35);

            // Weapons
            itemLoot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, new int[]
            {
                ModContent.ItemType<GildedProboscis>(),
                ModContent.ItemType<GoldenEagle>(),
                ModContent.ItemType<RougeSlash>()
            }));
            itemLoot.Add(ModContent.ItemType<Swordsplosion>(), 10);

            // Equipment
            itemLoot.Add(ModContent.ItemType<FollyFeed>(), DropHelper.BagWeaponDropRateFraction);
            itemLoot.Add(ModContent.ItemType<DynamoStemCells>());

            // Vanity
            itemLoot.Add(ModContent.ItemType<BumblefuckMask>(), 7);

            // Other
            itemLoot.AddIf(() => CalamityWorld.revenge, ModContent.ItemType<RedLightningContainer>());
        }
    }
}
