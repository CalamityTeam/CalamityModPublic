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
    public class ElysianTracers : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elysian Tracers");
            Tooltip.SetDefault("Ludicrous speed!\n" +
                "Counts as wings\n" +
                "Horizontal speed: 10.50\n" +
                "Acceleration multiplier: 2.75\n" +
                "Great vertical speed\n" +
                "Flight time: 180\n" +
                "36% increased running acceleration\n" +
                "Greater mobility on ice\n" +
                "Water and lava walking\n" +
                "Immunity to the On Fire! debuff\n" +
                "Temporary immunity to lava");
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 32;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.accessory = true;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
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
                int num60 = Dust.NewDust(new Vector2(player.position.X + (float)(player.width / 2) + (float)num59, player.position.Y + (float)(player.height / 2) - 15f), 30, 30, Main.rand.NextBool(2) ? 206 : 173, 0f, 0f, 100, default, 2.4f);
                Main.dust[num60].noGravity = true;
                Main.dust[num60].velocity *= 0.3f;
                if (Main.rand.NextBool(10))
                {
                    Main.dust[num60].fadeIn = 2f;
                }
                Main.dust[num60].shader = GameShaders.Armor.GetSecondaryShader(player.cWings, player);
            }
            CalamityPlayer modPlayer = player.Calamity();
            player.accRunSpeed = 9.25f;
            player.moveSpeed += 0.2f;
            player.iceSkate = true;
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax += 240;
            player.buffImmune[BuffID.OnFire] = true;
            player.wingTimeMax = 180;
            player.noFallDmg = true;
            modPlayer.IBoots = !hideVisual;
            modPlayer.elysianFire = !hideVisual;
            modPlayer.eTracers = true;
        }

        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising, ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenFalling = 0.95f; //0.85
            ascentWhenRising = 0.15f;
            maxCanAscendMultiplier = 1.1f; //1
            maxAscentMultiplier = 3.15f; //3
            constantAscend = 0.135f;
        }

        public override void HorizontalWingSpeeds(Player player, ref float speed, ref float acceleration)
        {
            speed = 10.5f;
            acceleration *= 2.75f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<InfinityBoots>().
                AddIngredient<ElysianWings>().
                AddIngredient<CosmiliteBar>(5).
                AddIngredient<AscendantSpiritEssence>(4).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
