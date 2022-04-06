using CalamityMod.Items.Armor;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class StarlightWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starlight Wings");
            Tooltip.SetDefault("Wings of the Nightingale\n" +
                "Horizontal speed: 7.50\n" +
                "Acceleration multiplier: 1.0\n" +
                "Average vertical speed\n" +
                "Flight time: 150\n" +
                "5% increased damage and critical strike chance while wearing the Daedalus Armor");
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
            if ((player.armor[0].type == ModContent.ItemType<DaedalusHat>() || player.armor[0].type == ModContent.ItemType<DaedalusHeadgear>() ||
                player.armor[0].type == ModContent.ItemType<DaedalusHelm>() || player.armor[0].type == ModContent.ItemType<DaedalusHelmet>() ||
                player.armor[0].type == ModContent.ItemType<DaedalusVisor>()) &&
                player.armor[1].type == ModContent.ItemType<DaedalusBreastplate>() && player.armor[2].type == ModContent.ItemType<DaedalusLeggings>())
            {
                player.GetDamage<GenericDamageClass>() += 0.05f;
                player.Calamity().AllCritBoost(5);
            }

            if (player.controlJump && player.wingTime > 0f && !player.jumpAgainCloud && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
            {
                int num59 = 4;
                if (player.direction == 1)
                {
                    num59 = -40;
                }
                int num60 = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)num59, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, 173, 0f, 0f, 100, default, 2.4f);
                Main.dust[num60].noGravity = true;
                Main.dust[num60].velocity *= 0.3f;
                if (Main.rand.NextBool(10))
                {
                    Main.dust[num60].fadeIn = 2f;
                }
                Main.dust[num60].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
            player.wingTimeMax = 150;
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

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 7.5f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<VerstaltiteBar>(5)
                .AddIngredient<EssenceofEleum>()
                .AddIngredient(ItemID.SoulofFlight, 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
