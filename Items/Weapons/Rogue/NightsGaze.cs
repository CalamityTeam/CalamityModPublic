using CalamityMod.Projectiles.Rogue;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class NightsGaze : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Night's Gaze");
            Tooltip.SetDefault("Strike your foes with this spear of the night\n" +
				"Throws a spear that shatters when it hits an enemy\n" +
				"Stealth strikes cause the spear to summon homing stars as it flies");
        }

        public override void SafeSetDefaults()
        {
            item.width = 82;
            item.damage = 520;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 20;
            item.knockBack = 1f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 82;
            item.maxStack = 1;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<NightsGazeProjectile>();
            item.shootSpeed = 30f;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
				if (p.WithinBounds(Main.maxProjectiles))
					Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/NightsGazeGlow"));
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Lumenite>(), 7);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 4);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 12);
            recipe.AddIngredient(ItemID.DayBreak);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
    }
}
