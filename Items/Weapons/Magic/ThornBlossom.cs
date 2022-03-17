using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ThornBlossom : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn Blossom");
            Tooltip.SetDefault("Every rose has its thorn");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 60;
            item.magic = true;
            item.mana = 10;
            item.width = 66;
            item.height = 68;
            item.useTime = 23;
            item.useAnimation = 23;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
            item.UseSound = SoundID.Item109;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BeamingBolt>();
            item.shootSpeed = 20f;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.statLife -= 3;
            if (player.statLife <= 0)
            {
                player.KillMe(PlayerDeathReason.ByCustomReason(player.name + " was violently pricked by a flower."), 1000.0, 0, false);
            }
            for (int index = 0; index < 3; ++index)
            {
                float SpeedX = speedX + Main.rand.Next(-120, 121) * 0.05f;
                float SpeedY = speedY + Main.rand.Next(-120, 121) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX * 1.5f, SpeedY * 1.5f, ModContent.ProjectileType<NettleRight>(), (int)(damage * 1.5), knockBack, player.whoAmI);
            }
            Projectile.NewProjectile(position.X, position.Y, speedX * 0.66f, speedY * 0.66f, type, damage, knockBack, player.whoAmI, 1f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ArchAmaryllis>());
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>(), 10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
