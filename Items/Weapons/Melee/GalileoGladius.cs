using System.Linq;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Melee.Shortswords;
using CalamityMod.Rarities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class GalileoGladius : BaseSwordHoldoutItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override int ProjectileType => ModContent.ProjectileType<GalileoGladiusProj>();
        public override bool SizeModifiers => true;
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 44;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.damage = 600;
            Item.DamageType = TrueMeleeDamageClass.Instance;
            Item.useAnimation = Item.useTime = 8;
            Item.knockBack = 10f;
            Item.UseSound = SoundID.Item1;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.shootSpeed = 10;
            base.SetDefaults();
        }
        public override void HoldItem(Player player)
        {
            player.Calamity().StratusStarburstResetTimer = (int)MathHelper.Max(player.Calamity().StratusStarburstResetTimer, 600);
        }

        public override bool AltFunctionUse(Player player)
        {
            return player.Calamity().AvaliableStarburst >= 10;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GalileoGladiusThrown>()] > 0)
            {
                var proj = Main.projectile.First(x => x.active && x.type == ModContent.ProjectileType<GalileoGladiusThrown>() && x.owner == player.whoAmI);
                if (proj.ai[0] > 0)
                    if (player.altFunctionUse == 2 && player.Calamity().AvaliableStarburst >= 20)
                        proj.ai[1] = 3;
                    else
                        proj.ai[1] = 2;
                proj.netUpdate = true;
                return false;
            }
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source,position,velocity*2,ModContent.ProjectileType<GalileoGladiusThrown>(),damage,knockback,player.whoAmI);
                return false;
            }
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Gladius).
                AddIngredient<Lumenyl>(8).
                AddIngredient<RuinousSoul>(5).
                AddIngredient<ExodiumCluster>(15).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
