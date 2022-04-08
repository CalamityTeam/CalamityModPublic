using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories.Wings
{
    [AutoloadEquip(EquipType.Wings)]
    public class CelestialTracers : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestial Tracers");
            Tooltip.SetDefault("Taking speed EVEN FURTHER BEYOND!\n" +
                "Counts as wings\n" +
                "Horizontal speed: 12.00\n" +
                "Acceleration multiplier: 3.0\n" +
                "Excellent vertical speed\n" +
                "Flight time: 250\n" +
                "50% increased running acceleration\n" +
                "Greater mobility on ice\n" +
                "Water and lava walking\n" +
                "Immunity to the On Fire! debuff\n" +
                "Temporary immunity to lava\n" +
                "Being hit for over 200 damage will make you immune for an extended period of time");
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.accessory = true;
            Item.Calamity().customRarity = CalamityRarity.Violet;
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
            CalamityPlayer modPlayer = player.Calamity();
            player.accRunSpeed = 10f;
            player.moveSpeed += 0.24f;
            player.iceSkate = true;
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax += 240;
            player.buffImmune[BuffID.OnFire] = true;
            player.wingTimeMax = 250;
            player.noFallDmg = true;
            modPlayer.IBoots = !hideVisual;
            modPlayer.elysianFire = !hideVisual;
            modPlayer.cTracers = true;
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

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ElysianTracers>().
                AddIngredient<DrewsWings>().
                AddIngredient<AuricBar>(5).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
