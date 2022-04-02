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
            DisplayName.SetDefault("Flamsteed Ring");
            Tooltip.SetDefault("Summons a colossal controllable mech\n" +
                "Right click to display the mech's control panel\n" +
                "The panel has 3 configurations, selected using the brackets on the edges of the UI\n" +
                "Each bracket powers 2 out of 3 possible functions, represented by the circular icons.\n" +
                "The bottom left icon miniaturizes the mech to the size of a player, but weakens its weapons.\n" +
                "The bottom right icon is a powerful jet booster which greatly enhances movement.\n" +
                "The top icon is the mech's weaponry. It must be powered in order to attack.\n" +
                "Click the top icon to switch between Regicide, an enormous energy blade, and a powerful Gauss rifle.\n" +
                "Exiting the mount while a boss is alive will temporarily hinder your movement\n" +
            CalamityUtils.ColorMessage("Now, make them pay.", new Color(135, 206, 235)));
        }

        public override void SetDefaults()
        {
            item.mana = 200;
            item.damage = 1999;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.width = item.height = 28;
            item.useTime = item.useAnimation = 9;
            item.noMelee = true;
            item.knockBack = 1f;

            item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            item.Calamity().customRarity = CalamityRarity.HotPink;
            item.Calamity().devItem = true;

            item.UseSound = SoundID.Item117;
            item.shoot = ModContent.ProjectileType<GiantIbanRobotOfDoom>();
            item.shootSpeed = 10f;
            item.summon = true;
            item.Calamity().CannotBeEnchanted = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Excelsus>());
            recipe.AddIngredient(ModContent.ItemType<CosmicViperEngine>());
            recipe.AddIngredient(ItemID.WingsVortex);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 40);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
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
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/AndromedaCripple"), position);
                }
                return false;
            }
            // Otherwise create one.
            return true;
        }

        // Moved from CalamityGlobalItem since it's just a function called in one place.
        internal static bool TransformItemUsage(Item item, Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return false;

            int robotIndex = -1;

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active &&
                    Main.projectile[i].type == ModContent.ProjectileType<GiantIbanRobotOfDoom>() &&
                    Main.projectile[i].owner == player.whoAmI)
                {
                    robotIndex = i;
                    break;
                }
            }
            if (robotIndex != -1)
            {
                Projectile robot = Main.projectile[robotIndex];
                GiantIbanRobotOfDoom robotModProjectile = ((GiantIbanRobotOfDoom)robot.modProjectile);
                if (player.ownedProjectileCounts[ModContent.ProjectileType<AndromedaRegislash>()] <= 0 &&
                    robotModProjectile.TopIconActive &&
                    (robotModProjectile.RightIconCooldown <= GiantIbanRobotOfDoom.RightIconAttackTime ||
                     !robotModProjectile.RightIconActive)) // "Melee" attack
                {
                    int damage = player.Calamity().andromedaState == AndromedaPlayerState.SmallRobot ? GiantIbanRobotOfDoom.RegicideBaseDamageSmall : GiantIbanRobotOfDoom.RegicideBaseDamageLarge;
                    if (item.Calamity().rogue)
                    {
                        damage = (int)(damage * player.RogueDamage());
                    }
                    if (item.melee)
                    {
                        damage = (int)(damage * player.MeleeDamage());
                    }
                    if (item.ranged)
                    {
                        damage = (int)(damage * player.RangedDamage());
                    }
                    if (item.magic)
                    {
                        damage = (int)(damage * player.MagicDamage());
                    }
                    if (item.summon)
                    {
                        damage = (int)(damage * player.MinionDamage());
                    }
                    Projectile blade = Projectile.NewProjectileDirect(robot.Center + (robot.spriteDirection > 0).ToDirectionInt() * robot.width / 2 * Vector2.UnitX,
                               Vector2.Zero, ModContent.ProjectileType<AndromedaRegislash>(), damage, 15f, player.whoAmI, Projectile.GetByUUID(robot.owner, robot.whoAmI));

                    if (blade.whoAmI.WithinBounds(Main.maxProjectiles))
                    {
                        if (item.Calamity().rogue)
                        {
                            blade.Calamity().forceRogue = true;
                        }
                        if (item.melee)
                        {
                            blade.Calamity().forceMelee = true;
                        }
                        if (item.ranged)
                        {
                            blade.Calamity().forceRanged = true;
                        }
                        if (item.magic)
                        {
                            blade.Calamity().forceMagic = true;
                        }
                        if (item.summon)
                        {
                            blade.Calamity().forceMinion = true;
                        }
                    }
                }

                if (!robotModProjectile.TopIconActive &&
                    (robotModProjectile.LeftBracketActive || robotModProjectile.RightBracketActive) &&
                    !robotModProjectile.BottomBracketActive &&
                    robotModProjectile.LaserCooldown <= 0 &&
                    (robotModProjectile.RightIconCooldown <= GiantIbanRobotOfDoom.RightIconAttackTime ||
                     !robotModProjectile.RightIconActive)) // "Ranged" attack
                {
                    robotModProjectile.LaserCooldown = AndromedaDeathRay.TrueTimeLeft * 2;
                    if (player.Calamity().andromedaState == AndromedaPlayerState.SmallRobot)
                    {
                        robotModProjectile.LaserCooldown = AndromedaDeathRay.TrueTimeLeft + 1;
                    }
                }
            }
            return false;
        }
    }
}
