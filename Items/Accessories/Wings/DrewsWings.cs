using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class DrewsWings : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Drew's Wings");
            Tooltip.SetDefault("Absolutely Fabulous\n" +
                "Horizontal speed: 12.00\n" +
                "Acceleration multiplier: 3.0\n" +
                "Excellent vertical speed\n" +
                "Flight time: 361");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.controlJump && player.wingTime > 0f && !player.canJumpAgain_Cloud && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
            {
                int num59 = 4;
                if (player.direction == 1)
                {
                    num59 = -40;
                }
                int num60 = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)num59, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, 91, 0f, 0f, 100, default, 2.4f);
                Main.dust[num60].noGravity = true;
                Main.dust[num60].velocity *= 0.3f;
                if (Main.rand.NextBool(10))
                {
                    Main.dust[num60].fadeIn = 2f;
                }
                Main.dust[num60].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
            player.wingTimeMax = 361;
            player.noFallDmg = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 1f; //0.85
            ascentWhenRising = 0.175f; //0.15
            maxCanAscendMultiplier = 1.2f; //1
            maxAscentMultiplier = 3.25f; //3
            constantAscend = 0.15f; //0.135
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 12f;
            acceleration *= 3f;
        }
    }
}
