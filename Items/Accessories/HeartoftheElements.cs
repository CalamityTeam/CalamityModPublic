using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Accessories
{
    public class HeartoftheElements : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<Rainbow>();
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.brimstoneWaifu || modPlayer.sandWaifu || modPlayer.sandBoobWaifu || modPlayer.cloudWaifu || modPlayer.sirenWaifu)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player != null && !player.dead)
                Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, Main.DiscoR / 255f, Main.DiscoG / 255f, Main.DiscoB / 255f);

            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.allWaifus = true;
            modPlayer.elementalHeart = true;

            int brimmy = ProjectileType<BrimstoneElementalMinion>();
            int siren = ProjectileType<WaterElementalMinion>();
            int healer = ProjectileType<SandElementalHealer>();
            int sandy = ProjectileType<SandElementalMinion>();
            int cloudy = ProjectileType<CloudElementalMinion>();

            var source = player.GetSource_Accessory(Item);
            Vector2 velocity = new Vector2(0f, -1f);
            int elementalDmg = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(90);
            float kBack = 2f + player.GetKnockback<SummonDamageClass>().Additive;

            if (player.ownedProjectileCounts[brimmy] > 1 || player.ownedProjectileCounts[siren] > 1 ||
                player.ownedProjectileCounts[healer] > 1 || player.ownedProjectileCounts[sandy] > 1 ||
                player.ownedProjectileCounts[cloudy] > 1)
            {
                player.ClearBuff(BuffType<HotE>());
            }
            if (player != null && player.whoAmI == Main.myPlayer && !player.dead)
            {
                if (player.FindBuffIndex(BuffType<HotE>()) == -1)
                {
                    player.AddBuff(BuffType<HotE>(), 3600, true);
                }
                if (player.ownedProjectileCounts[brimmy] < 1)
                {
                    int p = Projectile.NewProjectile(source, player.Center, velocity, brimmy, elementalDmg, kBack, player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = 90;
                }
                if (player.ownedProjectileCounts[siren] < 1)
                {
                    int p = Projectile.NewProjectile(source, player.Center, velocity, siren, elementalDmg, kBack, player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = 90;
                }
                if (player.ownedProjectileCounts[healer] < 1)
                {
                    int p = Projectile.NewProjectile(source, player.Center, velocity, healer, elementalDmg, kBack, player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = 90;
                }
                if (player.ownedProjectileCounts[sandy] < 1)
                {
                    int p = Projectile.NewProjectile(source, player.Center, velocity, sandy, elementalDmg, kBack, player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = 90;
                }
                if (player.ownedProjectileCounts[cloudy] < 1)
                {
                    int p = Projectile.NewProjectile(source, player.Center, velocity, cloudy, elementalDmg, kBack, player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = 90;
                }
            }
        }

        public override void UpdateVanity(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.allWaifusVanity = true;
            // modPlayer.elementalHeart = true;

            int brimmy = ProjectileType<BrimstoneElementalMinion>();
            int siren = ProjectileType<WaterElementalMinion>();
            int healer = ProjectileType<SandElementalHealer>();
            int sandy = ProjectileType<SandElementalMinion>();
            int cloudy = ProjectileType<CloudElementalMinion>();

            var source = player.GetSource_Accessory(Item);
            Vector2 velocity = new Vector2(0f, -1f);
            int elementalDmg = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(90);
            float kBack = 2f + player.GetKnockback<SummonDamageClass>().Additive;

            if (player.ownedProjectileCounts[brimmy] > 1 || player.ownedProjectileCounts[siren] > 1 ||
                player.ownedProjectileCounts[healer] > 1 || player.ownedProjectileCounts[sandy] > 1 ||
                player.ownedProjectileCounts[cloudy] > 1)
            {
                player.ClearBuff(BuffType<HotE>());
            }
            if (player != null && player.whoAmI == Main.myPlayer && !player.dead)
            {
                if (player.FindBuffIndex(BuffType<HotE>()) == -1)
                {
                    player.AddBuff(BuffType<HotE>(), 3600, true);
                }
                if (player.ownedProjectileCounts[brimmy] < 1)
                {
                    int p = Projectile.NewProjectile(source, player.Center, velocity, brimmy, elementalDmg, kBack, player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = 90;
                }
                if (player.ownedProjectileCounts[siren] < 1)
                {
                    int p = Projectile.NewProjectile(source, player.Center, velocity, siren, elementalDmg, kBack, player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = 90;
                }
                if (player.ownedProjectileCounts[healer] < 1)
                {
                    int p = Projectile.NewProjectile(source, player.Center, velocity, healer, elementalDmg, kBack, player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = 90;
                }
                if (player.ownedProjectileCounts[sandy] < 1)
                {
                    int p = Projectile.NewProjectile(source, player.Center, velocity, sandy, elementalDmg, kBack, player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = 90;
                }
                if (player.ownedProjectileCounts[cloudy] < 1)
                {
                    int p = Projectile.NewProjectile(source, player.Center, velocity, cloudy, elementalDmg, kBack, player.whoAmI);
                    if (Main.projectile.IndexInRange(p))
                        Main.projectile[p].originalDamage = 90;
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WifeinaBottle>().
                AddIngredient<WifeinaBottlewithBoobs>().
                AddIngredient<PearlofEnthrallment>().
                AddIngredient<EyeoftheStorm>().
                AddIngredient<RoseStone>().
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
