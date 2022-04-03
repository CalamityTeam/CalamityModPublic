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
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 66;
            Item.height = 68;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.UseSound = SoundID.Item109;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeamingBolt>();
            Item.shootSpeed = 20f;
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
            CreateRecipe(1).AddIngredient(ModContent.ItemType<ArchAmaryllis>()).AddIngredient(ModContent.ItemType<UeliaceBar>(), 10).AddIngredient(ModContent.ItemType<UnholyEssence>(), 10).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
