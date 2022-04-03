using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class LuminousStriker : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Luminous Striker");
            Tooltip.SetDefault("Send the stars back to where they belong\n"
                              +"Throws a stardust javelin trailed by rising stardust shards\n"
                              +"Explodes into additional stardust shards upon hitting enemies\n"
                              +"Stealth strikes cause the stardust shards to fly alongside the javelin instead of rising");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 86;
            Item.height = 102;
            Item.damage = 149;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 30;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(1, 0, 0, 0);
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<LuminousStrikerProj>();
            Item.shootSpeed = 20f;
            Item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int proj = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, (int)(damage * 1.25f), knockBack, player.whoAmI);
            if (Main.projectile.IndexInRange(proj))
                Main.projectile[proj].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<SpearofPaleolith>()).AddIngredient(ModContent.ItemType<ScourgeoftheSeas>()).AddIngredient(ModContent.ItemType<Turbulance>()).AddIngredient(ModContent.ItemType<MeldiateBar>(), 10).AddIngredient(ItemID.FragmentStardust, 10).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
