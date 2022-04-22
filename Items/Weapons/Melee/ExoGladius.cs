using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ExoGladius : ModItem
    {
        public const int OnHitIFrames = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Exo Gladius");
            Tooltip.SetDefault("Do not underestimate the power of Exoblade's younger brother\n" +
                "Striking an enemy with the blade makes you immune for a short time and summons comets from the sky\n" +
                "Fires a rainbow orb that summons sword beams on hit");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.useTurn = false;
            Item.useAnimation = 12;
            Item.useTime = 12;
            Item.width = 56;
            Item.height = 56;
            Item.damage = 690;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 9.9f;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<ExoGladProj>();
            Item.shootSpeed = 19f;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position.X, position.Y, Item.shootSpeed * player.direction, 0f, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 107);
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.ExoDebuffs();
            OnHitEffects(player);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.ExoDebuffs();
            OnHitEffects(player);
        }

        private void OnHitEffects(Player player)
        {
            player.GiveIFrames(OnHitIFrames, false);

            if (player.whoAmI == Main.myPlayer)
            {
                var source = player.GetSource_ItemUse(Item);
                int damage = player.GetWeaponDamage(player.ActiveItem());
                CalamityUtils.ProjectileRain(source, player.Center, 400f, 100f, 500f, 800f, 25f, ModContent.ProjectileType<ExoGladComet>(), damage, 15f, player.whoAmI);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GalileoGladius>().
                AddIngredient<CosmicShiv>().
                AddIngredient<Lucrecia>().
                AddIngredient<MiracleMatter>().
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
