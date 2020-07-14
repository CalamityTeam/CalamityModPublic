using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.DraedonsArsenal
{
    public class PlasmaGrenade : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Grenade");
            Tooltip.SetDefault("Throws a rolling barrel that explodes on wall collision\n" +
                               "Stealth strikes make it rain on collision");
        }

        public override void SafeSetDefaults()
        {
            item.width = 22;
            item.height = 28;
            item.damage = 50;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.consumable = true;
            item.maxStack = 999;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = item.useTime = 22;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<PlasmaGrenadeProjectile>();
            item.shootSpeed = 10f;
            item.Calamity().rogue = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile grenade = Projectile.NewProjectileDirect(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            grenade.Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 1);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 1);
            recipe.AddIngredient(ItemID.HallowedBar, 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 200);
            recipe.AddRecipe();
        }
    }
}
