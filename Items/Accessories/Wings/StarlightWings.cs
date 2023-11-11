using CalamityMod.Items.Armor.Daedalus;
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
    public class StarlightWings : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories.Wings";
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(150, 7.5f, 1f);
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if ((player.armor[0].type == ModContent.ItemType<DaedalusHeadMagic>() || player.armor[0].type == ModContent.ItemType<DaedalusHeadSummon>() ||
                player.armor[0].type == ModContent.ItemType<DaedalusHeadMelee>() || player.armor[0].type == ModContent.ItemType<DaedalusHeadRanged>() ||
                player.armor[0].type == ModContent.ItemType<DaedalusHeadRogue>()) &&
                player.armor[1].type == ModContent.ItemType<DaedalusBreastplate>() && player.armor[2].type == ModContent.ItemType<DaedalusLeggings>())
            {
                player.GetDamage<GenericDamageClass>() += 0.05f;
                player.GetCritChance<GenericDamageClass>() += 5;
            }

            if (player.controlJump && player.wingTime > 0f && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
            {
                int dustXOffset = 4;
                if (player.direction == 1)
                {
                    dustXOffset = -40;
                }
                int flightDust = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)dustXOffset, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, 173, 0f, 0f, 100, default, 2.4f);
                Main.dust[flightDust].noGravity = true;
                Main.dust[flightDust].velocity *= 0.3f;
                if (Main.rand.NextBool(10))
                {
                    Main.dust[flightDust].fadeIn = 2f;
                }
                Main.dust[flightDust].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
            player.noFallDmg = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.5f;
            ascentWhenRising = 0.1f;
            maxCanAscendMultiplier = 0.5f;
            maxAscentMultiplier = 1.5f;
            constantAscend = 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SoulofFlight, 20).
                AddIngredient<CryonicBar>(5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
