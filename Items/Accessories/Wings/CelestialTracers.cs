using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    [AutoloadEquip(EquipType.Wings)]
    public class CelestialTracers : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestial Tracers");
            Tooltip.SetDefault("Counts as wings\n" +
                "Horizontal speed: 12\n" +
                "Acceleration multiplier: 3\n" +
                "Excellent vertical speed\n" +
                "Flight time: 280\n" +
                "Taking speed EVEN FURTHER BEYOND!\n" +
                "Greater mobility on ice\n" +
                "Water and lava walking\n" +
                "Temporary immunity to lava\n" +
                "Being hit for over 200 damage will make you immune for an extended period of time");
        }

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 32;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.accessory = true;
            item.Calamity().postMoonLordRarity = 15;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.controlJump && player.wingTime > 0f && !player.jumpAgainCloud && player.jump == 0 && player.velocity.Y != 0f && !hideVisual)
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
            player.accRunSpeed = 12f;
            player.rocketBoots = 3;
            player.moveSpeed += 0.5f;
            player.iceSkate = true;
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax += 240;
            player.wingTimeMax = 280;
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
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "ElysianTracers");
            recipe.AddIngredient(null, "DrewsWings");
            recipe.AddIngredient(null, "DarksunFragment", 5);
            recipe.AddIngredient(null, "AuricOre", 25);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
