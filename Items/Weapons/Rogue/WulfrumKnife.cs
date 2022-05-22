using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class WulfrumKnife : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Knife");
            Tooltip.SetDefault("Stealth strikes make the knife fly further and hit several times at once");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.damage = 11;
            Item.noMelee = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 15;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 38;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(0, 0, 0, 5);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<WulfrumKnifeProj>();
            Item.shootSpeed = 12f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position, velocity * 1.3f, ModContent.ProjectileType<WulfrumKnifeProj>(), damage, knockback, player.whoAmI);
                Projectile proj = Main.projectile[p];
                if (p.WithinBounds(Main.maxProjectiles))
                {
                    proj.Calamity().stealthStrike = true;
                    proj.penetrate = 4;
                    proj.usesLocalNPCImmunity = true;
                    proj.localNPCHitCooldown = 1;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(100).
                AddIngredient<WulfrumShard>().
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
