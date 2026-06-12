using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    [LegacyName("TomeofFates")]
    public class Apathanull : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 30;
            Item.damage = 63;
            Item.DamageType = DamageClass.Magic;
            Item.crit = 12;
            Item.mana = 26;
            Item.useTime = 8;
            Item.useAnimation = 20;
            Item.reuseDelay = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = new SoundStyle("CalamityMod/Sounds/Item/MeldBurn") with { Volume = 0.7f, Pitch = Main.rand.NextFloat(-0.45f, -0.6f) };
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<CosmicTentacle>();
            Item.shootSpeed = 12f;
        }

        public override void HoldItem(Player player) => player.Calamity().mouseRotationListener = true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectileDirect(source, position, velocity.RotatedByRandom(0.7f), ModContent.ProjectileType<CosmicTentacle>(), damage, knockback, player.whoAmI);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpellTome).
                AddIngredient<MeldBlob>(18).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
