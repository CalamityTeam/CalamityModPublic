using CalamityMod.Buffs.Pets;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Reaver
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("ReaverHeadgear")]
    public class ReaverHeadExplore : ModItem
    {
        //Exploration and Mining Helm
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Reaver Headgear");
            Tooltip.SetDefault("40% increased pick speed and block/wall placement speed\n" +
                "Temporary immunity to lava and can move freely through liquids");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 7; //40
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ReaverScaleMail>() && legs.type == ModContent.ItemType<ReaverCuisses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Highlights all treasure nearby\n" +
                "Increased item grab range and block placement range\n" +
                "Mining tiles restores breath while underwater\n" +
                "Summons a reaver orb to light up the area around you\n" +
                "Reduces enemy aggression, even in the abyss\n" +
                "Provides a small amount of light in the abyss";
            var modPlayer = player.Calamity();
            modPlayer.reaverExplore = true;
            modPlayer.wearingRogueArmor = true;
            player.findTreasure = true;
            player.blockRange += 4;
            player.aggro -= 200;

            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_ItemUse(Item);
                if (player.FindBuffIndex(ModContent.BuffType<ReaverOrbBuff>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<ReaverOrbBuff>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<ReaverOrb>()] < 1)
                {
                    Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<ReaverOrb>(), 0, 0f, player.whoAmI);
                }
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;
            player.pickSpeed -= 0.4f;
            player.tileSpeed += 0.4f;
            player.wallSpeed += 0.4f;
            player.lavaMax += 420;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PerennialBar>(6).
                AddIngredient(ItemID.JungleSpores, 4).
                AddIngredient<EssenceofSunlight>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
