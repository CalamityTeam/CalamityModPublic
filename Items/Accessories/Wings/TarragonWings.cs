using CalamityMod.Dusts;
using CalamityMod.Items.Armor;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class TarragonWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Tarragon Wings");
            Tooltip.SetDefault("Born of the jungle\n" +
                "Horizontal speed: 9.50\n" +
                "Acceleration multiplier: 2.5\n" +
                "Great vertical speed\n" +
                "Flight time: 250\n" +
                "+15 defense and +2 life regen while wearing the Tarragon Armor");
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(250, 9.5f, 2.5f);
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.rare = ItemRarityID.Purple;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if ((player.armor[0].type == ModContent.ItemType<TarragonHelm>() || player.armor[0].type == ModContent.ItemType<TarragonHelmet>() ||
                player.armor[0].type == ModContent.ItemType<TarragonHornedHelm>() || player.armor[0].type == ModContent.ItemType<TarragonMask>() ||
                player.armor[0].type == ModContent.ItemType<TarragonVisage>()) &&
                player.armor[1].type == ModContent.ItemType<TarragonBreastplate>() && player.armor[2].type == ModContent.ItemType<TarragonLeggings>())
            {
                player.statDefense += 15;
                player.lifeRegen += 2;
            }

            if (player.controlJump && player.wingTime > 0f && !player.canJumpAgain_Cloud && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
            {
                int num59 = 4;
                if (player.direction == 1)
                {
                    num59 = -40;
                }
                int num60 = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)num59, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, (int)CalamityDusts.SulfurousSeaAcid, 0f, 0f, 100, default, 2.4f);
                Main.dust[num60].noGravity = true;
                Main.dust[num60].velocity *= 0.3f;
                if (Main.rand.NextBool(10))
                {
                    Main.dust[num60].fadeIn = 2f;
                }
                Main.dust[num60].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
            player.noFallDmg = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.85f;
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<UeliaceBar>(5).
                AddIngredient(ItemID.SoulofFlight, 30).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
