using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ExecutionersBlade : RogueWeapon
    {
        private int counter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Executioner's Blade");
            Tooltip.SetDefault("Throws a stream of homing blades\n" +
                "Stealth strikes summon a guillotine of blades on hit");
        }

        public override void SafeSetDefaults()
        {
            item.width = 64;
            item.damage = 200;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTime = 3;
            item.useAnimation = 9;
            item.reuseDelay = 1;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6.75f;
            item.UseSound = SoundID.Item73;
            item.autoReuse = true;
            item.height = 64;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = ItemRarityID.Red;
            item.shoot = ModContent.ProjectileType<ExecutionersBladeProj>();
            item.shootSpeed = 26f;
            item.Calamity().rogue = true;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Rogue/ExecutionersBladeGlow"));
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 12);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            bool usingStealth = player.Calamity().StealthStrikeAvailable() && counter == 0;
            if (usingStealth)
                damage = (int)(damage * 3.61);

            int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (usingStealth && stealth.WithinBounds(Main.maxProjectiles))
                Main.projectile[stealth].Calamity().stealthStrike = true;

            counter++;
            if (counter >= item.useAnimation / item.useTime)
                counter = 0;
            return false;
        }
    }
}
