using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ThornBlossom : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetStaticDefaults()
        {
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
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.UseSound = SoundID.Item109;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeamingBolt>();
            Item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.statLife -= 3;
            if (player.statLife <= 0)
            {
                player.KillMe(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.ThornBlossom").Format(player.name)), 1000.0, 0, false);
            }
            for (int index = 0; index < 3; ++index)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(30f)) * 1.5f, ModContent.ProjectileType<NettleRight>(), (int)(damage * 1.5), knockback, player.whoAmI);
            }
            Projectile.NewProjectile(source, position, velocity * 0.66f, type, damage, knockback, player.whoAmI, 1f);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArchAmaryllis>().
                AddIngredient<UelibloomBar>(10).
                AddIngredient<UnholyEssence>(10).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
