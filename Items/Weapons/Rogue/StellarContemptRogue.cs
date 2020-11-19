using CalamityMod.Projectiles.Hybrid;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class StellarContemptRogue : RogueWeapon
    {
        public static int BaseDamage = 250;
        public static float Speed = 18f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stellar Contempt");
            Tooltip.SetDefault("Lunar flares rain down on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 66;
            item.height = 64;
            item.damage = BaseDamage;
            item.knockBack = 9f;
            item.useTime = 13;
            item.useAnimation = 13;
            item.autoReuse = true;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;

            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.value = Item.buyPrice(1, 20, 0, 0);

            item.Calamity().rogue = true;
            item.shoot = ModContent.ProjectileType<StellarContemptHammer>();
            item.shootSpeed = Speed;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            Main.projectile[proj].Calamity().forceRogue = true;
			Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddIngredient(ModContent.ItemType<TruePaladinsHammer>());
            r.AddIngredient(ItemID.LunarBar, 5);
            r.AddIngredient(ItemID.FragmentSolar, 10);
            r.AddIngredient(ItemID.FragmentNebula, 10);
            r.AddTile(TileID.LunarCraftingStation);
            r.AddRecipe();
        }
    }
}
