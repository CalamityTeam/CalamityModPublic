using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace CalamityMod.Items.Tools
{
    public class WulfrumDiggingTurtle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Digging Turtle");
            Tooltip.SetDefault("Throws a rickety mining contraption to dig out a small tunnel\n" +
            "In case of an emergency, right click to instantly detonate all your digging turtles");
            SacrificeTotal = 10;
        }

        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 8;
            Item.maxStack = 999;
            Item.consumable = true;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<WulfrumDiggingTurtleProjectile>();
            Item.width = 30;
            Item.height = 38;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.UseSound = SoundID.Item1;
            Item.value = Item.buyPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Blue;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.ToolsOther;
		}

        public override bool AltFunctionUse(Player player) => true;

        public override bool ConsumeItem(Player player)
        {
            return player.altFunctionUse != 2;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Play a sound and detonate all owned turtles.
            if (player.altFunctionUse == 2)
            {
                bool explodedAny = false;

                for (int i = 0; i < Main.maxProjectiles; ++i)
                {
                    Projectile p = Main.projectile[i];
                    if (!p.active || p.owner != player.whoAmI || p.type != Item.shoot)
                        continue;

                    p.ai[1] = 1f;
                    p.timeLeft = 1;
                    p.netUpdate = true;
                    p.netSpam = 0;

                    explodedAny = true;
                }

                if (explodedAny)
                    SoundEngine.PlaySound(SoundID.Item73, position);

                return false;
            }

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient(ItemID.Gel, 5). //Gel is a default combustible item to fuel the motors of the lil guys
                AddIngredient<WulfrumMetalScrap>(3).
                Register();
        }
    }
}
