using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class PrototypeAndromechaRing : ModItem
    {
        public const int CrippleTime = 360; // 6 seconds
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prototype Andromecha Ring");
            Tooltip.SetDefault("Summons an enormous mechanical behemoth that you control\n" +
                               "Right clicking displays a menu of 3 brackets\n" +
                               "You can only select 1 of 3 brackets\n" +
                               "Each bracket grants control over 2 of 3 icons.\n" +
                               "The left icon is not toggleable and is always on if a bracket entails it\n" +
                               "If the left icon is enabled, the robot shrinks down to a normal size, returning your hitbox to normal but with weaker attacks\n" +
                               "The right icon can be clicked, but has a cooldown\n" +
                               "Clicking the right icon causes you to charge up, and then release bursts of branching lightning\n" +
                               "The top icon is toggleable\n" +
                               "The top icon switches between two modes, melee and ranged, which both replace normal attacks\n" +
                               "The melee mode slashes enemies with Regicide, an enormous energy blade\n" +
                               "The ranged mode releases 3 quick laser beams\n" +
                               "Only one laser is shot if the player is small\n" +
                               "Exiting the mount while a boss is alive gives you a temporary cripple" +
                               "It reminds you of something long lost");
        }

        public override void SetDefaults()
        {
            item.mana = 200;
            item.damage = 9999;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.width = item.height = 28;
            item.useTime = item.useAnimation = 10;
            item.noMelee = true;
            item.knockBack = 1f;
            item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
            item.rare = 10;
            item.UseSound = SoundID.Item117;
            item.shoot = ModContent.ProjectileType<GiantIbanRobotOfDoom>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 100);
            recipe.AddIngredient(ModContent.ItemType<Excelsus>(), 4);
            recipe.AddIngredient(ModContent.ItemType<CosmicViperEngine>());
            recipe.AddIngredient(ItemID.WingsVortex);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool CanUseItem(Player player) => !(player.Calamity().andromedaCripple > 0 && CalamityPlayer.areThereAnyDamnBosses);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            // If the player has any robots, kill them all.
            if (player.ownedProjectileCounts[item.shoot] > 0)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].active && 
                        Main.projectile[i].type == item.shoot &&
                        Main.projectile[i].owner == player.whoAmI)
                    {
                        Main.projectile[i].Kill();
                    }
                }
                if (CalamityPlayer.areThereAnyDamnBosses)
                {
                    player.Calamity().andromedaCripple = CrippleTime;
                    player.AddBuff(ModContent.BuffType<AndromedaCripple>(), player.Calamity().andromedaCripple);
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AdrenalineBurnout1"), position);
                }
                return false;
            }
            // Otherwise create one.
            return true;
        }
    }
}
